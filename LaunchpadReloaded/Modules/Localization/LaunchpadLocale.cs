using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using BepInEx.Configuration;
using BepInEx.Logging;
using MiraAPI.Roles;
using Reactor.Localization;

namespace LaunchpadReloaded.Modules.Localization;

public static class LaunchpadLocale
{
    private const string DefaultLocaleCode = "en_US";
    private static readonly Dictionary<string, Dictionary<string, string>> Translations =
        new(System.StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> EmbeddedLocaleFiles =
        new(System.StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> TmpTextList = new()
    {
        { "[nl]", "\n" },
        { "[and]", "&" },
    };

    private static bool _initialized;
    private static ManualLogSource? _logger;
    private static ConfigEntry<string>? _selectedLocaleConfig;

    public static void Initialize(ConfigFile config)
    {
        if (_initialized)
        {
            return;
        }

        _logger ??= BepInEx.Logging.Logger.CreateLogSource("LaunchpadLocale");

        DiscoverEmbeddedLocales();

        _selectedLocaleConfig ??= config.Bind(
            "Localization",
            "Locale",
            DefaultLocaleCode,
            BuildLocaleConfigDescription());

        LoadEmbeddedLocales();
        ValidateConfiguredLocale();
        _initialized = true;
    }

    public static bool IsMissingString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ||
               value.Equals("STRMISS", System.StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("STRMISS_", System.StringComparison.OrdinalIgnoreCase);
    }

    public static string ResolveRoleDisplayName(RoleBehaviour role)
    {
        if (role is ICustomRole customRole && !IsMissingString(customRole.RoleName))
        {
            return customRole.RoleName;
        }

        if (!IsMissingString(role.NiceName))
        {
            return role.NiceName;
        }

        if (role.IsDead)
        {
            return GetParsed("RoleGhostName", "Ghost");
        }

        return role.GetType().Name.Replace("Role", string.Empty);
    }

    public static string GetParsed(string name, string? defaultValue = null,
        Dictionary<string, string>? parseList = null)
    {
        return GetParsedForLocale(GetSelectedLocaleCode(), name, defaultValue, parseList);
    }

    public static string GetParsed(SupportedLangs language, string name, string? defaultValue = null,
        Dictionary<string, string>? parseList = null)
    {
        return GetParsedForLocale(GetSelectedLocaleCode(), name, defaultValue, parseList);
    }

    public static string GetParsedForLocale(string localeCode, string name, string? defaultValue = null,
        Dictionary<string, string>? parseList = null)
    {
        var text = defaultValue ?? "STRMISS_" + name;
        var normalizedLocaleCode = NormalizeLocaleCode(localeCode) ?? DefaultLocaleCode;

        if (Translations.TryGetValue(DefaultLocaleCode, out var englishTranslations) &&
            englishTranslations.TryGetValue(name, out var englishText))
        {
            text = englishText;
        }

        if (!normalizedLocaleCode.Equals(DefaultLocaleCode, System.StringComparison.OrdinalIgnoreCase) &&
            Translations.TryGetValue(normalizedLocaleCode, out var localeTranslations) &&
            localeTranslations.TryGetValue(name, out var localeText))
        {
            text = localeText;
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

    private static string GetSelectedLocaleCode()
    {
        var configuredLocaleCode = NormalizeLocaleCode(_selectedLocaleConfig?.Value);
        if (configuredLocaleCode != null && EmbeddedLocaleFiles.ContainsKey(configuredLocaleCode))
        {
            return configuredLocaleCode;
        }

        return DefaultLocaleCode;
    }

    private static void DiscoverEmbeddedLocales()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith("LaunchpadReloaded.Resources.Locale.", System.StringComparison.Ordinal) &&
                           name.EndsWith(".xml", System.StringComparison.OrdinalIgnoreCase));

        EmbeddedLocaleFiles.Clear();

        foreach (var resourceName in resourceNames)
        {
            var fileName = resourceName["LaunchpadReloaded.Resources.Locale.".Length..];
            var localeCode = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(localeCode))
            {
                continue;
            }

            EmbeddedLocaleFiles[localeCode] = fileName;
        }
    }

    private static void LoadEmbeddedLocales()
    {
        if (EmbeddedLocaleFiles.Count == 0)
        {
            _logger?.LogError("No embedded locale XML files were found.");
            return;
        }

        if (EmbeddedLocaleFiles.TryGetValue(DefaultLocaleCode, out var defaultLocaleFile))
        {
            LoadEmbeddedLocale(DefaultLocaleCode, defaultLocaleFile);
        }

        foreach (var (localeCode, fileName) in EmbeddedLocaleFiles)
        {
            if (localeCode.Equals(DefaultLocaleCode, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            LoadEmbeddedLocale(localeCode, fileName);
        }
    }

    private static void LoadEmbeddedLocale(string localeCode, string fileName)
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

        Translations[localeCode] = new Dictionary<string, string>();
        ParseXml(xmlContent, Translations[localeCode]);
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

            target[nameAttr] = node.InnerText ?? string.Empty;
        }
    }

    private static void ValidateConfiguredLocale()
    {
        var configuredLocaleCode = NormalizeLocaleCode(_selectedLocaleConfig?.Value);
        if (configuredLocaleCode != null && EmbeddedLocaleFiles.ContainsKey(configuredLocaleCode))
        {
            return;
        }

        if (_selectedLocaleConfig == null)
        {
            return;
        }

        _logger?.LogWarning(
            $"Unknown Launchpad locale '{_selectedLocaleConfig.Value}'. Falling back to {DefaultLocaleCode}.");
        _selectedLocaleConfig.Value = DefaultLocaleCode;
    }

    private static string BuildLocaleConfigDescription()
    {
        var availableLocales = EmbeddedLocaleFiles.Count == 0
            ? DefaultLocaleCode
            : string.Join(", ", EmbeddedLocaleFiles.Keys
                .OrderBy(static code => code)
                .Select(code => $"{code} ({GetLocaleDisplayName(code)})"));

        return $"Locale XML to use for Launchpad strings. Available locales: {availableLocales}";
    }

    private static string GetLocaleDisplayName(string localeCode)
    {
        var normalizedLocaleCode = NormalizeLocaleCode(localeCode) ?? localeCode;
        var languagePrefix = normalizedLocaleCode.Split('_')[0];

        try
        {
            return CultureInfo.GetCultureInfo(languagePrefix).NativeName;
        }
        catch (CultureNotFoundException)
        {
            return languagePrefix;
        }
    }

    private static string? NormalizeLocaleCode(string? localeCode)
    {
        if (string.IsNullOrWhiteSpace(localeCode))
        {
            return null;
        }

        return localeCode.Trim().Replace('-', '_');
    }
}
