using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EPi.Owin.Setup.Settings
{
    public static class SecuritySetupAppSettings
    {
        public const string EPiLog4NetFileName = "EPiServerLog.config";

        public static int ValidateIdentityInterval { get; private set; }
        public static int LogoutIdleTimeSpan { get; private set; }
        public static string AllUsersRole { get; private set; }
        public static List<string> AllowedDomains { get; private set; }

        public static string GoogleAuthenticationClientId { get; private set; }
        public static string GoogleAuthenticationClientSecret { get; private set; }

        static SecuritySetupAppSettings()
        {
            LogoutIdleTimeSpan = Convert.ToInt32(ConfigurationManager.AppSettings["LogoutIdleTimeSpan"] ?? "20");
            ValidateIdentityInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ValidateIdentityInterval"] ?? "15");

            AllUsersRole = ConfigurationManager.AppSettings["AllUsersRole"];
            AllowedDomains = ConfigurationManager.AppSettings["AllowedDomains"].Split(',').Select(i => i.Trim()).ToList();

            GoogleAuthenticationClientId = ConfigurationManager.AppSettings["GoogleAuthenticationClientId"];
            GoogleAuthenticationClientSecret = ConfigurationManager.AppSettings["GoogleAuthenticationClientSecret"];
        }
    }
}
