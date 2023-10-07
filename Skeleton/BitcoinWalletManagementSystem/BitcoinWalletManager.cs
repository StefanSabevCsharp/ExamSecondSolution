using System;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinWalletManagementSystem
{
    public class BitcoinWalletManager : IBitcoinWalletManager
    {
        private Dictionary<string, User> users = new Dictionary<string, User>();
        private Dictionary<string, Wallet> wallets = new Dictionary<string, Wallet>();

        private Dictionary<User,List<Wallet>> userWallets = new Dictionary<User, List<Wallet>>();
        private Dictionary<User, HashSet<Transaction>> transactionsByUserId = new Dictionary<User, HashSet<Transaction>>();

        public void CreateUser(User user)
        {
           
                this.users.Add(user.Id, user);
            
        }

        public void CreateWallet(Wallet wallet)
        {
                this.wallets.Add(wallet.Id, wallet);

            User currentUser = new User() { Id = wallet.UserId };
           
                
            
            if(!this.userWallets.ContainsKey(currentUser))
            {
                this.userWallets.Add(currentUser, new List<Wallet>());
            }
            this.userWallets[currentUser].Add(wallet);
             
            
            
        }

        public bool ContainsUser(User user)
        {
            return this.users.ContainsKey(user.Id);
        }

        public bool ContainsWallet(Wallet wallet)
        {
            return this.wallets.ContainsKey(wallet.Id);
        }

        public IEnumerable<Wallet> GetWalletsByUser(string userId)
        {
           return this.userWallets[this.users[userId]];
        }

        public void PerformTransaction(Transaction transaction)
        {

            string firstWalletID = transaction.SenderWalletId;
            string secondWalletID = transaction.ReceiverWalletId;
            Wallet firstWallet = this.wallets[firstWalletID];
            Wallet receiverWallet = this.wallets[secondWalletID];

            User firstUser = this.users[firstWallet.UserId];
            User secondUser = this.users[receiverWallet.UserId];

            

                firstWallet.Balance -= transaction.Amount;
                receiverWallet.Balance += transaction.Amount;


            if (!transactionsByUserId.ContainsKey(firstUser))
            {
                transactionsByUserId[firstUser] = new HashSet<Transaction>();
            }

            if (!transactionsByUserId.ContainsKey(secondUser))
            {
                transactionsByUserId[secondUser] = new HashSet<Transaction>();
            }

            transactionsByUserId[firstUser].Add(transaction);
            transactionsByUserId[secondUser].Add(transaction);

        }

        public IEnumerable<Transaction> GetTransactionsByUser(string userId)
        {
           
            return this.transactionsByUserId[this.users[userId]];
        }

        public IEnumerable<Wallet> GetWalletsSortedByBalanceDescending()
        {
            return this.wallets.Values.OrderByDescending(x => x.Balance);
        }

        public IEnumerable<User> GetUsersSortedByBalanceDescending()
        {
            return this.users.Values.OrderByDescending(x => this.userWallets[x].Sum(y => y.Balance));
        }

        public IEnumerable<User> GetUsersByTransactionCount()
        {
            return this.transactionsByUserId.Keys.OrderByDescending(x => this.transactionsByUserId[x].Count);
        }
    }
}