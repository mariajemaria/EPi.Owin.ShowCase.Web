using System;
using System.Configuration;
using System.Linq;

namespace EPi.Owin.Settings
{
    public static class SecurityAppSettings
    {
        public static string AdminRole { get; private set; }
        public static string[] AdminUsers { get; private set; }
        public static string PrefixForAllowedRoles { get; private set; }
        public static bool AllowRolesCreation { get; private set; }
        public static string DbNameForContext { get; private set; }

        static SecurityAppSettings()
        {
            AdminRole = ConfigurationManager.AppSettings["AdminRole"];
            AdminUsers = ConfigurationManager.AppSettings["AdminUsers"].Split(',').Select(i => i.Trim()).ToArray();
            PrefixForAllowedRoles = ConfigurationManager.AppSettings["PrefixForAllowedRoles"];
            AllowRolesCreation = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowRolesCreation"]);
            DbNameForContext = ConfigurationManager.AppSettings["DbNameForContext"];
        }
    }
}
