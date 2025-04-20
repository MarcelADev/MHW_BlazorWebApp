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

        #region HTTP Statuscodes

        public static int HTTP_STATUSCODE_CONFLICT = 409;

        #endregion
    }
}
