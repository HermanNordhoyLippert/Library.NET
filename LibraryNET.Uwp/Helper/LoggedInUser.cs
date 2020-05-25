using LibraryNET.Model;

namespace LibraryNET.Helper
{
    public static class LoggedInUser
    {
        // Different solution then using local storage. but only works if the application is run localy on users machin. Would not work on a server.
        public static User CurrentLoggedInUser { get; set; } = null;
    }
}
