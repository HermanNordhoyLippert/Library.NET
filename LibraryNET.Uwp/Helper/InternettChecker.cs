using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LibraryNET.Helper
{
    public class InternettChecker
    {
        /// <summary>
        /// Checks for internet connection.
        /// </summary>
        /// <returns>true if machine is connected to the internett and false if not</returns>
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://www.hiof.no/"))
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
