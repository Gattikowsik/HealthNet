using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HealthNet.Utility;

public class EmailHelper
{
    public static (bool IsValid, string Message) ValidateEmail(string email)
    {
        //validating the email
        if(string.IsNullOrWhiteSpace(email))
            return (false, "Email address cannot be empty.");
        //maximum length for email
        if(email.Length > 254)
            return (false, "Email is too long.");
         try
            {
                //basic format verification
                var addr = new MailAddress(email);
                if (addr.Address != email)
                    return (false, "Invalid email format.");
                // Regex patternfor the presence of an '@' and a dot
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(email, pattern))
                    return (false, "Email must have a valid domain.");

                return (true, "Email is valid.");
            }
            catch (FormatException)
            {
                // Handle cases where MailAddress constructor fails
                return (false, "The email format is incorrect.");
            }
    }
}
