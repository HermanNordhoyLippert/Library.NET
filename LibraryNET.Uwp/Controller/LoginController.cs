using System.Threading.Tasks;
using LibraryNET.Helper;

namespace LibraryNET.Controller
{
    public class LoginController
    {
        /// <summary>
        /// Logins the request asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>returns true if successful and false if not</returns>
        public async Task<bool> LoginRequestAsync(string username, string password)
        {
            foreach (var item in await DonauApi.ApiHelper.GetUser())
            {
                if(item.Username.Equals(username) && item.Password.Equals(password))
                {
                    LoggedInUser.CurrentLoggedInUser = item;
                    return true;
                }
            }
            return false;
        }
    }
}
