using System;

namespace RabbitCoin
{
    class BlockChain
    {
        public Block[] chain = new Block[1];
        public int minPendingTransactions = 1;
        public int difficulty = 4;
        public decimal miningReward = 100;
        public Transaction[] pendingTransactions = new Transaction[0];

        public BlockChain()
        {
            chain[0] = createGenesisBlock();
        }

        public Block createGenesisBlock()
        {
            return new Block(new Transaction[]{new Transaction(null, "6871bf25e4a062d9db9e26bc625ee671f3ee12457a07441a13c7381ab5866747", 1000000000, "hVT1/FUDlU8WOvM0bGreF+c4QNlRH7QMgakRYJSK7Kd+T/gIBr/bVwE1t+Ri8zQqeS0foqunrZaMX8XcquCiJQ==")}, "0");
        }

        public Block getLatestBlock()
        {
            return chain[chain.Length-1];
        }

        public void minePendingTransactions(String miningRewardAddress)
        {
            if(pendingTransactions.Length >= minPendingTransactions){
                Block block = new Block(pendingTransactions);
                block.previousHash = getLatestBlock().hash;
                block.mineBlock(difficulty);

                Array.Resize(ref chain, chain.Length + 1);
                chain[chain.Length - 1] = block;

                Array.Resize(ref pendingTransactions, 1);
                pendingTransactions[0] = new Transaction(null, miningRewardAddress, miningReward, null);
            }else{
                Console.WriteLine("Pending transactions don't exists. You need to wait for new transaction to be added...");
            }
        }

        public void addTransaction(Transaction transaction){
            if(transaction.fromAddress == null || transaction.fromAddress.Length < 64){
                Console.WriteLine("Transaction must have valid from address!");
                return;
            }
            if(transaction.toAddress == null || transaction.toAddress.Length < 64){
                Console.WriteLine("Transaction must have valid payee address!");
                return;
            }
            if(!transaction.isValid()){
                Console.WriteLine("Can't add invalid transaction to a blockchain!");
                return;
            }
            if(transaction.amount <= 0){
                Console.WriteLine("You need to send at least few Rabbit Coinse!");
                return;
            }
            if(getBalanceOfAddress(transaction.fromAddress) < transaction.amount){
                Console.WriteLine("You don't have enough Rabbit Coins to make this transaction!");
                return;
            }
            Array.Resize(ref pendingTransactions, pendingTransactions.Length + 1);
            pendingTransactions[pendingTransactions.Length - 1] = transaction;
        }

        public decimal getBalanceOfAddress(String address){
            decimal balance = 0;
            foreach(Block block in chain){
                foreach(Transaction transaction in block.transactions){
                    if(transaction.fromAddress == address) balance -= transaction.amount;
                    if(transaction.toAddress == address) balance += transaction.amount;
                }
            }
            return balance;
        }

        public bool isChainValid()
        {
            for(int i = 1; i < chain.Length; i++){
                Block currentBlock = chain[i];
                Block previousBlock = chain[i-1];

                if(!currentBlock.hasValidTransaction()) return false;
                if(currentBlock.hash != currentBlock.calculateHash()) return false;
                if(currentBlock.previousHash != previousBlock.hash) return false;
            }
            return true;
        }
    }
}
