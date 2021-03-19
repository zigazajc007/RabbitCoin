using System;
using System.Text;
using System.Security.Cryptography;

namespace RabbitCoin
{
    class Hash
    {
        String text;

        public Hash(String text)
        {
            if(text != null){
                this.text = text;
            }else{
                this.text = "0000000000";
            }
        }

        public String create()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(this.text));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
