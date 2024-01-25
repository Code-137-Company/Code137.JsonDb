using System.Security.Cryptography;

namespace Code137.JsonDb.Security
{
    public class Cryptograph
    {
        private readonly string _password;

        public Cryptograph(string password)
        {
            _password = password;
        }

        public string Encrypt(string content)
        {
            using (Aes alg = Aes.Create())
            {
                alg.Key = new Rfc2898DeriveBytes(_password, new byte[16]).GetBytes(16);
                alg.IV = new byte[16];

                ICryptoTransform encryptor = alg.CreateEncryptor(alg.Key, alg.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(content);
                        }
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string content)
        {
            using (Aes alg = Aes.Create())
            {
                alg.Key = new Rfc2898DeriveBytes(_password, new byte[16]).GetBytes(16);
                alg.IV = new byte[16];

                ICryptoTransform decryptor = alg.CreateDecryptor(alg.Key, alg.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(content)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
