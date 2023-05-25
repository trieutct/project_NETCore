using System.Text;

namespace Sport_Shop.Models
{
    public class Encrypt
    {
        public static string Key = "adef@sdsj@";
        public static string ConvertToEncrypt(string password)
        {
            byte[] a = ASCIIEncoding.UTF8.GetBytes(password);
            string EncryptPassword = Convert.ToBase64String(a);
            return EncryptPassword;
        }
    }
}
