using System;
using System.Security.Cryptography;

namespace RabbitCoin
{
    class Wallet
    {
        public String publicKey;
        public String privateKey;

        public Wallet(){
            HMACSHA256 hmac = new HMACSHA256();
            privateKey = Convert.ToBase64String(hmac.Key);
            publicKey = new Hash(privateKey).create();
        }
    }
}
