using System;
using System.Security.Principal;

namespace PrinterQueueWatch
{

    static class MCLApiUtilities
    {

        public static bool LoggedInAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            try
            {
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception e)
            {
                return false;
            }

        }


    }
}