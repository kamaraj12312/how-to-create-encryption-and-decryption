namespace EndeWcf
{
    public class UserModel
    {


        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string DecryptedPassword { get; set; }
    }
}