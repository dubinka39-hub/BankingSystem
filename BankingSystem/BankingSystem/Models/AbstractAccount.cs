using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BankingSystem.Enums;

namespace BankingSystem.Models
{
    public abstract class AbstractAccount
    {
        public string Id { get; protected set; }
        public string OwnerName { get; protected set; }
        protected decimal Balance { get; set; }
        public AccountStatus Status { get; protected set; }

        protected AbstractAccount(string id, string ownerName, decimal initialBalance = 0)
        {
            Id = id;
            OwnerName = ownerName;
            Balance = initialBalance;
            Status = AccountStatus.Active;
        }

        public abstract void Deposit(decimal amount);
        public abstract void Withdraw(decimal amount);
        public abstract string GetAccountInfo();

        public override string ToString()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            return $"Тип: {GetType().Name}, Клиент: {OwnerName}, Номер: ****{lastFourDigits}, " +
                   $"Статус: {Status}, Баланс: {Balance}";
        }
    }
}
