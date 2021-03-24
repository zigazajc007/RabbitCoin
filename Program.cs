using System;
using System.IO;
using System.Threading;

namespace RabbitCoin
{
    class Program
    {
        static BlockChain rabbitCoin;
        static Wallet wallet;
        static int page = 0;

        static void Main(string[] args)
        {
            rabbitCoin = new BlockChain();
            importBlockChain(false);

            while(true){
                showOptions();
                ConsoleKeyInfo input = Console.ReadKey();
                switch(input.KeyChar){
                    case 'w':
                        generateWallet();
                    break;
                    case 'b':
                        showBalance();
                    break;
                    case 'c':
                        createTransaction();
                    break;
                    case 'm':
                        mineTransactions();
                    break;
                    case 'd':
                        displayBlockChain();
                    break;
                    case 'g':
                        giveRC();
                    break;
                    case 's':
                        saveBlockChain(true);
                    break;
                    case 'i':
                        importBlockChain(true);
                    break;
                    case 'q':
                        saveBlockChain(false);
                        Environment.Exit(0);
                    break;
                }

            }
        }

        static void generateWallet()
        {
            page = 1;
            showOptions();
            Console.WriteLine("\n\tWallet:");
            wallet = new Wallet();
            Console.WriteLine("\n\t  Public key: " + wallet.publicKey);
            Console.WriteLine("\t  Private key: " + wallet.privateKey);
            ConsoleKeyInfo input = Console.ReadKey();
            if(input.KeyChar == 's') saveWallet(wallet.publicKey, wallet.privateKey);
            page = 0;
        }

        static void saveWallet(String publicKey, String privateKey){
            Directory.CreateDirectory("Wallets");
            for(int i = 1; i < int.MaxValue; i++){
                if(!File.Exists("Wallets/"+i+".txt")){
                    File.WriteAllText("Wallets/"+i+".txt", "public key: " + publicKey + "\nprivate key: " + privateKey);
                    break;
                }
            }
        }

        static void saveBlockChain(bool specific_location){
            page = 3;
            Directory.CreateDirectory("BlockChain");
            String file_name = "BlockChain";
            if(specific_location){
                showOptions();
                Console.Write("\nProvide name of the file (without extension): ");
                file_name = Console.ReadLine();
            }
            String blockchain = "";
            foreach(Block block in rabbitCoin.chain){
                blockchain += block.timestamp + ";";
                blockchain += block.nonce + ";";
                blockchain += block.previousHash + ";";
                blockchain += block.hash + ";";
                foreach(Transaction transaction in block.transactions){
                    blockchain += transaction.fromAddress + ";";
                    blockchain += transaction.toAddress + ";";
                    blockchain += transaction.amount + ";";
                    blockchain += transaction.signature + ";";
                }
                blockchain += "\n";
            }
            File.WriteAllText("BlockChain/" + file_name + ".txt", blockchain);
            if(specific_location){
                Console.WriteLine("BlockChain is saved to " + Directory.GetCurrentDirectory() + "/BlockChain/" + file_name + ".txt");
                Console.ReadKey();
            }
            page = 0;
        }

        static void importBlockChain(bool specific_location){
            page = 3;
            String file_name = "BlockChain";
            if(specific_location){
                showOptions();
                Console.Write("\nProvide name of the file (without extension): ");
                file_name = Console.ReadLine();
            }
            if(File.Exists("BlockChain/" + file_name + ".txt")){
                String blockchain = File.ReadAllText("BlockChain/" + file_name + ".txt");
                String[] blocks = blockchain.Split('\n');
                Array.Resize(ref rabbitCoin.chain, blocks.Length-1);
                Block block = new Block(new Transaction[0]);
                for(int i = 0; i < blocks.Length-1; i++){
                    String[] data = blocks[i].Split(';');
                    Transaction[] transactions = new Transaction[(data.Length-4)/4];
                    block.timestamp = Convert.ToInt64(data[0]);
                    block.nonce = Convert.ToInt32(data[1]);
                    block.previousHash = data[2];
                    block.hash = data[3];
                    int index = 0;
                    for(int j = 4; j < data.Length-1; j += 4){
                        transactions[index] = new Transaction(data[j], data[j+1], Convert.ToInt64(data[j+2]), data[j+3]);
                        index++;
                    }
                    block.transactions = transactions;
                    rabbitCoin.chain[i] = block;
                }
            }else{
                if(specific_location)
                    Console.WriteLine("BlockChain in " + Directory.GetCurrentDirectory() + "/BlockChain/" + file_name + ".txt does not exists!");
            }
            if(specific_location) Console.ReadKey();
            page = 0;
        }

        static void createTransaction(){
            showOptions();
            Console.Write("\nPlease enter your address: ");
            String fromAddress = Console.ReadLine();
            Console.Write("Please enter private key for your address: ");
            String privateKey = Console.ReadLine();
            Console.Write("Please enter payee address: ");
            String toAddress = Console.ReadLine();
            Console.Write("Please enter amount of Rabbit Coins: ");
            decimal amount = 0;
            while(amount <= 0){
                try{
                    amount = Convert.ToDecimal(Console.ReadLine());
                }catch{
                    Console.Write("Please enter amount of Rabbit Coins: ");
                }
            }
            rabbitCoin.addTransaction(new Transaction(fromAddress, toAddress, amount, privateKey));
            Console.ReadKey();
        }

        static void giveRC(){
            page = 3;
            showOptions();
            Console.Write("\nPlease enter your address: ");
            String toAddress = Console.ReadLine();
            Console.Write("Please enter amount of Rabbit Coins: ");
            decimal amount = 0;
            while(amount <= 0){
                try{
                    amount = Convert.ToDecimal(Console.ReadLine());
                }catch{
                    Console.Write("Please enter amount of Rabbit Coins: ");
                }
            }
            rabbitCoin.addTransaction(new Transaction("6871bf25e4a062d9db9e26bc625ee671f3ee12457a07441a13c7381ab5866747", toAddress, amount, "hVT1/FUDlU8WOvM0bGreF+c4QNlRH7QMgakRYJSK7Kd+T/gIBr/bVwE1t+Ri8zQqeS0foqunrZaMX8XcquCiJQ=="));
            Console.ReadKey();
            page = 0;
        }

        static void mineTransactions(){
            page = 3;
            showOptions();
            Console.Write("\nPlease enter your address (Mining rewards will be send to this address): ");
            String toAddress = Console.ReadLine();
            Console.WriteLine();
            while(!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)){
                rabbitCoin.minePendingTransactions(toAddress);
                Thread.Sleep(500);
            }
            page = 0;
        }

        static void showBalance()
        {
            page = 2;
            showOptions();
            ConsoleKeyInfo input = Console.ReadKey();
            if(input.KeyChar == 'u'){
                page = 3;
                showOptions();
                if(wallet != null){
                    Console.WriteLine("\n\tBalance of '" + wallet.publicKey + "' address is " + rabbitCoin.getBalanceOfAddress(wallet.publicKey) + " RC");
                }else{
                    Console.WriteLine("\n\tYou did not create any wallet.");
                }
                Console.ReadKey();
            }else if(input.KeyChar == 'm'){
                page = 3;
                showOptions();
                Console.Write("\n\tPlease enter public key: ");
                String address = Console.ReadLine();
                Console.WriteLine("\n\tBalance of '" + address + "' address is " + rabbitCoin.getBalanceOfAddress(address) + " RC");
                Console.ReadKey();
            }
            page = 0;
        }

        static void showOptions(){
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n██████╗  █████╗ ██████╗ ██████╗ ██╗████████╗     ██████╗ ██████╗ ██╗███╗   ██╗");
            Console.WriteLine("██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║╚══██╔══╝    ██╔════╝██╔═══██╗██║████╗  ██║");
            Console.WriteLine("██████╔╝███████║██████╔╝██████╔╝██║   ██║       ██║     ██║   ██║██║██╔██╗ ██║");
            Console.WriteLine("██╔══██╗██╔══██║██╔══██╗██╔══██╗██║   ██║       ██║     ██║   ██║██║██║╚██╗██║");
            Console.WriteLine("██║  ██║██║  ██║██████╔╝██████╔╝██║   ██║       ╚██████╗╚██████╔╝██║██║ ╚████║");
            Console.WriteLine("╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚═════╝ ╚═╝   ╚═╝        ╚═════╝ ╚═════╝ ╚═╝╚═╝  ╚═══╝\n");
            Console.WriteLine("Options:");
            switch(page){
                case 1:
                    Console.WriteLine("\ts - Save wallet to file");
                    Console.WriteLine("\tq - Back");
                    break;
                case 2:
                    Console.WriteLine("\tu - Use last created wallet");
                    Console.WriteLine("\tm - Manually enter wallet");
                    Console.WriteLine("\tq - Back");
                    break;
                case 3:
                    Console.WriteLine("\tq - Back");
                    break;
                default:
                    Console.WriteLine("\tw - Create new wallet");
                    Console.WriteLine("\tb - Show balance of specific wallet");
                    Console.WriteLine("\tc - Create new transaction");
                    Console.WriteLine("\tm - Mine pending transactions");
                    Console.WriteLine("\td - Display BlockChain");
                    Console.WriteLine("\tg - Give RC (DEVELOPER)");
                    Console.WriteLine("\ts - Save BlockChain");
                    Console.WriteLine("\ti - Import BlockChain");
                    Console.WriteLine("\tq - Quit");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }

        static void displayBlockChain()
        {
            page = 3;
            showOptions();
            foreach(Block block in rabbitCoin.chain){
                Console.WriteLine("_______________________________________________________________________________");
                Console.WriteLine("TimeStamp: " + block.timestamp);
                Console.WriteLine("Transactions: ");
                foreach(Transaction transaction in block.transactions){
                    Console.WriteLine("\t__________________________________________________");
                    Console.WriteLine("\tFrom Address: " + transaction.fromAddress);
                    Console.WriteLine("\tTo Address: " + transaction.toAddress);
                    Console.WriteLine("\tAmount: " + transaction.amount);
                    Console.WriteLine("\tSignature: " + transaction.signature);
                    Console.WriteLine("\t__________________________________________________");
                }
                Console.WriteLine("\nNonce: " + block.nonce);
                Console.WriteLine("Previous Hash: " + block.previousHash);
                Console.WriteLine("Hash: " + block.hash);
                Console.WriteLine("_______________________________________________________________________________");
            }
            Console.ReadKey();
            page = 0;
        }
    }
}
