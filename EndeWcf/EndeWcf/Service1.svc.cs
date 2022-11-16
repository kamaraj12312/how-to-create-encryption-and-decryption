using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Security;

namespace EndeWcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {







        private static readonly UTF8Encoding Encoder = new UTF8Encoding();









        //  public string Password { get;  set; }

        //public string Encrypt(string str)
        //{
        //    string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
        //    byte[] byKey = { };
        //    byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
        //    byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    return Convert.ToBase64String(ms.ToArray());
        //}

        //public string Decrypt(string str)
        //{
        //    str = str.Replace(" ", "+");
        //    string DecryptKey = "2013;[pnuLIT)WebCodeExpert";
        //    byte[] byKey = { };
        //    byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
        //    byte[] inputByteArray = new byte[str.Length];

        //    byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        //    return encoding.GetString(ms.ToArray());
        //}








        public static string Encrypt(string unencrypted)
        {
            if (string.IsNullOrEmpty(unencrypted))
                return string.Empty;

            try
            {
                var encryptedBytes = MachineKey.Protect(Encoder.GetBytes(unencrypted));

                if (encryptedBytes != null && encryptedBytes.Length > 0)
                    return HttpServerUtility.UrlTokenEncode(encryptedBytes);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        public static string Decrypt(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return string.Empty;

            try
            {
                var bytes = HttpServerUtility.UrlTokenDecode(encrypted);
                if (bytes != null && bytes.Length > 0)
                {
                    var decryptedBytes = MachineKey.Unprotect(bytes);
                    if (decryptedBytes != null && decryptedBytes.Length > 0)
                        return Encoder.GetString(decryptedBytes);
                }

            }
            catch (Exception)
            {
                return string.Empty;
            }

            return string.Empty;
        }











































        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            string constr = ConfigurationManager.ConnectionStrings["TestDBEntities"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Username, Password FROM aesUSERS"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            users.Add(new UserModel
                            {
                                Username = sdr["Username"].ToString(),
                                EncryptedPassword = sdr["Password"].ToString(),
                                DecryptedPassword = Decrypt(sdr["Password"].ToString())
                            });
                        }
                    }
                    con.Close();
                }
            }
            return users;
        }

        public List<UserModel> GetUsersEncrypt(string userName, string password)
        {
            string constr = ConfigurationManager.ConnectionStrings["TestDBEntities"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO aesUSERS VALUES (@Username, @Password)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Username" , userName);
                     // cmd.Parameters.AddWithValue("@Password", con);

                    cmd.Parameters.AddWithValue("@Password", Encrypt(password));

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return GetUsersEncrypt(userName , password);
        }
    }
}
