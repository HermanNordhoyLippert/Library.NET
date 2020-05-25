using System;

namespace LibraryNET.Helper
{
    public class InputValidator
    {
        // Thought I had to validate more userinput and api data so created this class, 
        //but ended up just needing a string validator for Google's api because some books does not have alot of data.
        public string StringValidator(string s)
        {
            if(String.IsNullOrWhiteSpace(s))
            {
                return "Data not found";
            }
            else
            {
                return s;
            }
        }
    }
}
