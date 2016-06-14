using System;
using System.Security.Principal;

namespace GodlikeStickCreator.Utilities
{
    internal static class User
    {
        private static bool? _isAdministrator;

        public static bool IsAdministrator => _isAdministrator ?? (_isAdministrator = IsUserAdministrator()).Value;

        private static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                var user = WindowsIdentity.GetCurrent();
                if (user == null)
                    return false;

                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }

            return isAdmin;
        }
    }
}