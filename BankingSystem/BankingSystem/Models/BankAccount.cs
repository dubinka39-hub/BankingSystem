using BankingSystem.Enums;
using BankingSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Models
{
    public class BankAccount : AbstractAccount
    {
        private static readonly Random _random = new Random();
        private const int SHORT_ID_LENGTH = 8;

        public Currency Currency { get; private set; }

        public BankAccount(string ownerName, Currency currency, decimal initialBalance = 0)
            : this(GenerateShortId(), ownerName, currency, initialBalance)
        {
        }

        public BankAccount(string id, string ownerName, Currency currency, decimal initialBalance = 0)
            : base(ValidateId(id), ValidateOwnerName(ownerName), ValidateInitialBalance(initialBalance))
        {
            Currency = currency;
        }

        private static string GenerateShortId()
        {
            const string chars = "0123456789";
            var result = new StringBuilder(SHORT_ID_LENGTH);

            for (int i = 0; i < SHORT_ID_LENGTH; i++)
            {
                result.Append(chars[_random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        private static string ValidateId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationError("ID счета не может быть пустым");
            return id;
        }

        private static string ValidateOwnerName(string ownerName)
        {
            if (string.IsNullOrWhiteSpace(ownerName))
                throw new InvalidOperationError("Имя владельца не может быть пустым");
            if (ownerName.Length < 2)
                throw new InvalidOperationError("Имя владельца слишком короткое");
            return ownerName.Trim();
        }

        private static decimal ValidateInitialBalance(decimal balance)
        {
            if (balance < 0)
                throw new InvalidOperationError("Начальный баланс не может быть отрицательным");
            return balance;
        }

        // ИЗМЕНЕНО: protected вместо private
        protected void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationError("Сумма должна быть положительной");
        }

        // ИЗМЕНЕНО: protected вместо private
        protected void ValidateAccountStatusForOperation()
        {
            switch (Status)
            {
                case AccountStatus.Frozen:
                    throw new AccountFrozenError();
                case AccountStatus.Closed:
                    throw new AccountClosedError();
            }
        }

        public override void Deposit(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            Balance += amount;
            Console.WriteLine($"Счет {Id}: успешно пополнен на {amount} {Currency}. Новый баланс: {Balance} {Currency}");
        }

        public override void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            if (Balance < amount)
                throw new InsufficientFundsError();

            Balance -= amount;
            Console.WriteLine($"Счет {Id}: успешно снято {amount} {Currency}. Новый баланс: {Balance} {Currency}");
        }

        public void FreezeAccount()
        {
            if (Status == AccountStatus.Closed)
                throw new InvalidOperationError("Нельзя заморозить закрытый счет");

            Status = AccountStatus.Frozen;
            Console.WriteLine($"Счет {Id} заморожен");
        }

        public void UnfreezeAccount()
        {
            if (Status == AccountStatus.Frozen)
            {
                Status = AccountStatus.Active;
                Console.WriteLine($"Счет {Id} разморожен");
            }
        }

        public void CloseAccount()
        {
            if (Balance != 0)
                throw new InvalidOperationError("Нельзя закрыть счет с ненулевым балансом");

            Status = AccountStatus.Closed;
            Console.WriteLine($"Счет {Id} закрыт");
        }

        public override string GetAccountInfo()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;

            return $"Информация о счете:\n" +
                   $"Тип счета: {GetType().Name}\n" +
                   $"Владелец: {OwnerName}\n" +
                   $"Номер счета: ****{lastFourDigits}\n" +
                   $"Полный номер: {Id}\n" +
                   $"Статус: {Status}\n" +
                   $"Баланс: {Balance} {Currency}\n" +
                   $"Валюта: {Currency}";
        }

        public override string ToString()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            return $"{GetType().Name} - {OwnerName} (****{lastFourDigits}) | Статус: {Status} | Баланс: {Balance} {Currency}";
        }

        public decimal GetBalance() => Balance;
    }
}