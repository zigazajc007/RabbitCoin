using System;

namespace RabbitCoin
{
    class Block
    {
        public long timestamp;
        public Transaction[] transactions;
        public String previousHash;
        public String hash;
        public int nonce = 0;

        public Block(Transaction[] transactions, String previousHash = "")
        {
            this.timestamp = DateTime.Now.ToFileTimeUtc();
            this.transactions = transactions;
            this.previousHash = previousHash;
            this.hash = calculateHash();
        }

        public String calculateHash()
        {
            String trans = "";
            foreach(Transaction transaction in transactions) trans += transaction;
            return new Hash(previousHash + timestamp + trans + nonce).create();
        }

        public void mineBlock(int difficulty)
        {
            Console.WriteLine("Mining block...");
            while(hash.Substring(0, difficulty) != new String('0', difficulty)){
                nonce++;
                hash = calculateHash();
            }
            Console.WriteLine("Block mined: " + hash);
        }

        public bool hasValidTransaction(){
            foreach(Transaction trans in transactions){
                if(!trans.isValid()) return false;
            }
            return true;
        }
    }
}
