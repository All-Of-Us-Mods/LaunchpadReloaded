//
// Kino/Bloom v2 - Bloom filter for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using LaunchpadReloaded.Features;
using MiraAPI.LocalSettings;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public class Bloom(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    public void SetBloomByMap()
    {
        if (ShipStatus.Instance == null)
        {
            return;
        }

        // Use custom bloom settings if enabled.
        if (LocalSettingsTabSingleton<LaunchpadSettings>.Instance.CustomBloomSettings.Value)
        {
            ThresholdLinear = LocalSettingsTabSingleton<LaunchpadSettings>.Instance.BloomSlider.Value;
            return;
        }

        var multiplier = 1.0f;
        if (Application.platform == RuntimePlatform.Android)
        {
            multiplier = 1.45f;
        }

        ThresholdLinear = multiplier *
                          (ShipStatus.Instance.TryCast<AirshipStatus>() ||
                           ShipStatus.Instance.Type is ShipStatus.MapType.Hq or ShipStatus.MapType.Fungle
                              ? 1.3f : 0.95f);
    }

    #region Public Properties

    /// Prefilter threshold (gamma-encoded)
    /// Filters out pixels under this level of brightness.
    public float ThresholdGamma
    {
        get => Mathf.Max(_threshold, 0);
        set => _threshold = value;
    }

    /// Prefilter threshold (linearly-encoded)
    /// Filters out pixels under this level of brightness.
    public float ThresholdLinear
    {
        get => GammaToLinear(ThresholdGamma);
        set => _threshold = LinearToGamma(value);
    }

    private float _threshold = LinearToGamma(0.95f);

    /// Soft-knee coefficient
    /// Makes transition between under/over-threshold gradual.
    public float SoftKnee { get; set; } = 0.5f;

    /// Bloom radius
    /// Changes extent of veiling effects in a screen
    /// resolution-independent fashion.
    public float Radius { get; set; } = 8f;

    /// Bloom intensity
    /// Blend factor of the result image.
    public float Intensity
    {
        get => Mathf.Max(_intensity, 0);
        set => _intensity = value;
    }

    private float _intensity = 1;

    /// High quality mode
    /// Controls filter quality and buffer resolution.
    public bool HighQuality { get; set; } = true;

    /// Anti-flicker filter
    /// Reduces flashing noise with an additional filter.
    public bool AntiFlicker { get; set; } = true;

    #endregion

    #region Private Members

    private readonly Shader _shader = LaunchpadAssets.BloomShader.LoadAsset();

    private Material _material = null!;

    private const int KMaxIterations = 16;
    private readonly RenderTexture[] _blurBuffer1 = new RenderTexture[KMaxIterations];
    private readonly RenderTexture[] _blurBuffer2 = new RenderTexture[KMaxIterations];

    private static readonly int Threshold = Shader.PropertyToID("_Threshold");
    private static readonly int Curve = Shader.PropertyToID("_Curve");
    private static readonly int PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
    private static readonly int SampleScale = Shader.PropertyToID("_SampleScale");
    private static readonly int Intensity1 = Shader.PropertyToID("_Intensity");
    private static readonly int BaseTex = Shader.PropertyToID("_BaseTex");

    private static float LinearToGamma(float x)
    {
        return Mathf.LinearToGammaSpace(x);
    }

    private static float GammaToLinear(float x)
    {
        return Mathf.GammaToLinearSpace(x);
    }

    #endregion

    #region MonoBehaviour Functions

    private void OnEnable()
    {
        _material = new Material(_shader)
        {
            hideFlags = HideFlags.DontSave
        };
    }

    private void OnDisable()
    {
        DestroyImmediate(_material);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var useRGBM = Application.isMobilePlatform;

        // source texture size
        var tw = source.width;
        var th = source.height;

        // halve the texture size for the low quality mode
        if (!HighQuality)
        {
            tw /= 2;
            th /= 2;
        }

        // blur buffer format
        var rtFormat = useRGBM ?
            RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;

        // determine the iteration count
        var logh = Mathf.Log(th, 2) + Radius - 8;
        var loghI = (int)logh;
        var iterations = Mathf.Clamp(loghI, 1, KMaxIterations);

        // update the shader properties
        var lthresh = ThresholdLinear;
        _material.SetFloat(Threshold, lthresh);

        var knee = lthresh * SoftKnee + 1e-5f;
        var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
        _material.SetVector(Curve, curve);

        var pfo = !HighQuality && AntiFlicker;
        _material.SetFloat(PrefilterOffs, pfo ? -0.5f : 0.0f);

        _material.SetFloat(SampleScale, 0.5f + logh - loghI);
        _material.SetFloat(Intensity1, Intensity);

        // prefilter pass
        var prefiltered = RenderTexture.GetTemporary(tw, th, 0, rtFormat);
        var pass = AntiFlicker ? 1 : 0;
        Graphics.Blit(source, prefiltered, _material, pass);

        // construct a mip pyramid
        var last = prefiltered;
        for (var level = 0; level < iterations; level++)
        {
            _blurBuffer1[level] = RenderTexture.GetTemporary(
                last.width / 2, last.height / 2, 0, rtFormat
            );

            pass = level == 0 ? AntiFlicker ? 3 : 2 : 4;
            Graphics.Blit(last, _blurBuffer1[level], _material, pass);

            last = _blurBuffer1[level];
        }

        // upsample and combine loop
        for (var level = iterations - 2; level >= 0; level--)
        {
            var basetex = _blurBuffer1[level];
            _material.SetTexture(BaseTex, basetex);

            _blurBuffer2[level] = RenderTexture.GetTemporary(
                basetex.width, basetex.height, 0, rtFormat
            );

            pass = HighQuality ? 6 : 5;
            Graphics.Blit(last, _blurBuffer2[level], _material, pass);
            last = _blurBuffer2[level];
        }

        // finish process
        _material.SetTexture(BaseTex, source);
        pass = HighQuality ? 8 : 7;
        Graphics.Blit(last, destination, _material, pass);

        // release the temporary buffers
        for (var i = 0; i < KMaxIterations; i++)
        {
            if (_blurBuffer1[i] != null)
            {
                RenderTexture.ReleaseTemporary(_blurBuffer1[i]);
            }

            if (_blurBuffer2[i] != null)
            {
                RenderTexture.ReleaseTemporary(_blurBuffer2[i]);
            }

            _blurBuffer1[i] = null!;
            _blurBuffer2[i] = null!;
        }

        RenderTexture.ReleaseTemporary(prefiltered);
    }

    #endregion
}