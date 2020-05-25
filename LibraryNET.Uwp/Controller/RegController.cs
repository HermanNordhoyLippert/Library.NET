using System.Threading.Tasks;
using LibraryNET.Model;
using System;

namespace LibraryNET.Controller
{
    public class RegController
    {
        /// <summary>
        /// Registereds the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>returns true if successful and false if not</returns>
        public async Task<bool> RegisteredUser(string username, string password)
        {
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrWhiteSpace(username) && !String.IsNullOrEmpty(password) && !String.IsNullOrWhiteSpace(password))
            {
                User newUser = new User();
                newUser.Username = username;
                newUser.Password = password;

                var listOfUsers = await DonauApi.ApiHelper.GetUser();

                if (!listOfUsers.Exists(x => x.Username == username))
                {
                    await DonauApi.ApiHelper.PostUser(newUser);
                    return true;
                }
                else
                {
                    return false;
                }
                //foreach (var item in await DonauApi.ApiHelper.GetUser())
                //{
                //    if(item.Username != newUser.Username)
                //    {
                //        await DonauApi.ApiHelper.PostUser(newUser);
                //        return true;
                //    }
                //}
                //return false;
            }
            else
            {
                return false;
            }
        }
    }
}
