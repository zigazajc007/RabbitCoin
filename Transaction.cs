using System;

namespace RabbitCoin
{
    class Transaction
    {
        public String fromAddress;
        public String toAddress;
        public decimal amount;
        public String signature;

        String privateKey;

        public Transaction(String fromAddress, String toAddress, decimal amount, String privateKey)
        {
            this.fromAddress = fromAddress;
            this.toAddress = toAddress;
            this.amount = amount;
            this.privateKey = privateKey;

            signTransaction();
        }

        public String calculateHash(){
            return new Hash(fromAddress + toAddress + amount).create();
        }

        public void signTransaction(){
            if(fromAddress == null){
                signature = new Hash("0000000000" + calculateHash() + "0000000000").create();
                return;
            }
            if(fromAddress != new Hash(privateKey).create()){
                Console.WriteLine("Private key is not correct!");
                return;
            }
            signature = new Hash(fromAddress + calculateHash() + privateKey).create();
        }

        public bool isValid(){
            if(fromAddress == null) return true;
            if(signature == null || signature.Length > 64) return false;
            return true;
        }

        public override String ToString(){
            return "From Address: " + fromAddress + "\nTo Address: " + toAddress + "\nAmount: " + amount;
        }
    }
}
