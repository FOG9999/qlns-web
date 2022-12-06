using System;
using System.ComponentModel;

namespace VIETTEL.Helpers
{
    public static class ErrorHelpers
    {
        #region sql
        public static bool ExceptionContainsErrorCode(Exception ex, int ErrorCode)
        {
            Win32Exception winEx = ex as Win32Exception;
            if (winEx != null && ErrorCode == winEx.ErrorCode)
                return true;
            if (ex.InnerException != null)
                return ExceptionContainsErrorCode(ex.InnerException, ErrorCode);
            return false;
        }
        #endregion sql
    }
}
