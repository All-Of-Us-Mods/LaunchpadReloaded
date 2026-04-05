using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BepInEx.Logging;
using Reactor.Localization;
using UnityEngine;

namespace LaunchpadReloaded.Modules.Localization;

public static class LaunchpadLocale
{
    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> Translations = new();
    private static readonly Dictionary<string, string> TmpTextList = new()
    {
        { "<nl>", "\n" },
        { "<and>", "&" },
    };

    private static bool _initialized;
    private static ManualLogSource? _logger;

    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        _logger ??= BepInEx.Logging.Logger.CreateLogSource("LaunchpadLocale");

        LoadEmbeddedLocale(SupportedLangs.English, "en_US.xml");
    }

    public static string GetParsed(string name, string? defaultValue = null,
        Dictionary<string, string>? parseList = null)
    {
        var currentLanguage =
            TranslationController.InstanceExists
                ? TranslationController.Instance.currentLanguage.languageID
                : SupportedLangs.English;
        return GetParsed(currentLanguage, name, defaultValue, parseList);
    }

    public static string GetParsed(SupportedLangs language, string name, string? defaultValue = null,
        Dictionary<string, string>? parseList = null)
    {
        var text = defaultValue ?? "STRMISS_" + name;

        if (Translations.TryGetValue(SupportedLangs.English, out var translationsEng) &&
            translationsEng.TryGetValue(name, out var translationEng))
        {
            text = translationEng;
        }

        if (language is not SupportedLangs.English &&
            Translations.TryGetValue(language, out var translations) &&
            translations.TryGetValue(name, out var translation))
        {
            text = translation;
        }

        text = Regex.Replace(text, @"\%([^%]+)\%", @"<$1>");
        if (text.Contains("\\<"))
        {
            text = text.Replace("\\<", "<");
        }

        if (text.Contains("\\>"))
        {
            text = text.Replace("\\>", ">");
        }

        foreach (var tmpText in TmpTextList.Where(x => text.Contains(x.Key)))
        {
            text = text.Replace(tmpText.Key, tmpText.Value);
        }

        if (parseList != null)
        {
            foreach (var tmpText in parseList.Where(x => text.Contains(x.Key)))
            {
                text = text.Replace(tmpText.Key, tmpText.Value);
            }
        }

        return text;
    }

    private static void LoadEmbeddedLocale(SupportedLangs language, string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"LaunchpadReloaded.Resources.Locale.{fileName}";

        using var resourceStream = assembly.GetManifestResourceStream(resourceName);
        if (resourceStream == null)
        {
            _logger?.LogError($"Missing embedded locale resource: {resourceName}");
            return;
        }

        using var reader = new StreamReader(resourceStream);
        var xmlContent = reader.ReadToEnd();

        if (!Translations.ContainsKey(language))
        {
            Translations[language] = new Dictionary<string, string>();
        }

        ParseXml(xmlContent, Translations[language]);
    }

    private static void ParseXml(string xmlContent, Dictionary<string, string> target)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        var root = xmlDoc.DocumentElement;
        if (root == null || root.Name != "resources")
        {
            _logger?.LogError("Locale XML root node must be <resources>.");
            return;
        }

        var stringNodes = root.SelectNodes("string");
        if (stringNodes == null)
        {
            return;
        }

        foreach (XmlNode node in stringNodes)
        {
            var nameAttr = node.Attributes?["name"]?.Value;
            if (string.IsNullOrWhiteSpace(nameAttr))
            {
                continue;
            }

            var value = node.InnerText ?? string.Empty;
            target[nameAttr] = value;
        }
    }
}
