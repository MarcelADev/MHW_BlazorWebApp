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

        #region Logs

        public static string LOG_WARNING = "WARNING: ";
        public static string LOG_UNAUTHORIZED = "WARNING: You were not authorized!";
        public static string LOG_UNEXPECTED_HTTP_STATUSCODE = "Received an unexpected HTTP Response Statuscode";

        public static string LOG_GET_ONE_SUCCESS = " was retrieved successfully";
        public static string LOG_GET_MANY_SUCCESS = " were retrieved successfully";

        public static string LOG_CREATE_SUCCESS = " was created successfully";
        public static string LOG_CREATE_FAILURE = " was NOT created";

        public static string LOG_UPDATE_SUCCESS = " was updated successfully";
        public static string LOG_UPDATE_FAILURE = " was NOT updated";

        public static string LOG_DELETE_SUCCESS = " was deleted successfully";
        public static string LOG_DELETE_FAILURE = " was NOT deleted";

        #endregion
    }
}
