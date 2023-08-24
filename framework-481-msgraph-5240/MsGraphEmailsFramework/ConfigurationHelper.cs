using System.Configuration;
using System.Diagnostics;

namespace MsGraphEmailsFramework
{
    public static class ConfigurationHelper
    {
        public static bool GetBool(string configurationKey, bool defaultValue = false)
        {
            var appSetting = ConfigurationManager.AppSettings[configurationKey];
            if (string.IsNullOrEmpty(appSetting))
            {
                Trace.TraceWarning($"Found no value for [{configurationKey}]; returning default [{defaultValue}].");
                return defaultValue;
            }

            if (bool.TryParse(appSetting, out var parsedValue))
            {
                return parsedValue;
            }

            Trace.TraceWarning($"Could not parse AppSetting [{configurationKey}] into a boolean, returning default [{defaultValue}].");
            return defaultValue;
        }
    }
}
