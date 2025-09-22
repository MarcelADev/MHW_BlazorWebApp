using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library
{
    public static class StaticVariables
    {
        public static int ORDER_DEFAULT_VALUE = 9999;

        #region Roles

        public static string USER_ROLE = "User";
        public static string ADMIN_ROLE = "Admin";
        public static string MASTER_ROLE = "Master";

        #endregion

        #region Armor Types
        public enum ArmorType
        {
            Head = 0,
            Chest = 1,
            Legs = 2
        }
        #endregion

        #region Password & Username Requirements & Banned Names

        public static Dictionary<string, object> PASSWORD_REQUIREMENTS = new()
        {
            {"RequireDigit", true },
            {"RequireLowercase", true },
            {"RequireUppercase", true },
            {"RequireNonAlphanumeric", true },
            {"RequiredLength", 10 },
            {"RequiredUniqueChars", 10 },
        };

        public static char[] SPECIAL_CHARACTERS = new char[]
        {
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+',
            '[', ']', '{', '}', ';', ':', '\'', '"', ',', '<', '.', '>', '/', '?', '\\', '|', '`', '~'
        };

        public static string ALLOWED_USERNAME_CHARACTERS = "abcdefghijklmnopqrstuvwxyz" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "0123456789" + " ";

        public static string[] BANNED_NAMES = new string[]
        {
            "master",
            "admin",
            "administrator",
            "root",
            "system",
            "moderator",
            "mod",
            "owner",
            "staff",
            "abbasstinkt",
            "polestinkt",
            "marcelstinkt"
        };

        #endregion

        #region Alert Messages

        public static string ALERT_MESSAGE_UNEXPECTED_SERVER_RESPONSE = "Unexpected server response. Contact support";

        #endregion
    }
}
