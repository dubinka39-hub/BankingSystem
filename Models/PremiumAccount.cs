using BankingSystem.Enums;
using BankingSystem.Exceptions;
using System;

namespace BankingSystem.Models
{
    public class PremiumAccount : BankAccount
    {
        public decimal OverdraftLimit { get; private set; }
        public decimal WithdrawalLimit { get; private set; }
        public decimal FixedCommission { get; private set; }
        private decimal _todayWithdrawn = 0;
        private DateTime _lastWithdrawalDate = DateTime.MinValue;

        public PremiumAccount(string ownerName, Currency currency, decimal overdraftLimit = 10000,
            decimal withdrawalLimit = 50000, decimal fixedCommission = 100, decimal initialBalance = 0)
            : base(ownerName, currency, initialBalance)
        {
            OverdraftLimit = overdraftLimit;
            WithdrawalLimit = withdrawalLimit;
            FixedCommission = fixedCommission;
        }

        public PremiumAccount(string id, string ownerName, Currency currency, decimal overdraftLimit = 10000,
            decimal withdrawalLimit = 50000, decimal fixedCommission = 100, decimal initialBalance = 0)
            : base(id, ownerName, currency, initialBalance)
        {
            OverdraftLimit = overdraftLimit;
            WithdrawalLimit = withdrawalLimit;
            FixedCommission = fixedCommission;
        }

        private void ResetDailyLimitIfNeeded()
        {
            if (_lastWithdrawalDate.Date != DateTime.Now.Date)
            {
                _todayWithdrawn = 0;
                _lastWithdrawalDate = DateTime.Now;
            }
        }

        public override void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            ResetDailyLimitIfNeeded();

            if (_todayWithdrawn + amount > WithdrawalLimit)
                throw new InvalidOperationError($"Превышен дневной лимит снятия: {WithdrawalLimit} {Currency}");

            decimal totalAmount = amount + FixedCommission;
            decimal currentBalance = GetBalance();
            decimal availableFunds = currentBalance + OverdraftLimit;

            if (totalAmount > availableFunds)
                throw new InsufficientFundsError();

            // Напрямую уменьшаем баланс
            Balance -= totalAmount;
            _todayWithdrawn += amount;

            Console.WriteLine($"Счет {Id}: успешно снято {amount} {Currency} (комиссия {FixedCommission} {Currency}). Новый баланс: {Balance} {Currency}");

            if (Balance < 0)
                Console.WriteLine($"Счет {Id}: использован овердрафт. Доступно: {GetAvailableOverdraft()} {Currency}");
        }

        public decimal GetAvailableOverdraft()
        {
            decimal balance = GetBalance();
            if (balance >= 0)
                return OverdraftLimit;
            else
                return OverdraftLimit - Math.Abs(balance);
        }

        public override string GetAccountInfo()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            decimal balance = GetBalance();

            return $"Информация о премиум счете:\n" +
                   $"Тип счета: {GetType().Name}\n" +
                   $"Владелец: {OwnerName}\n" +
                   $"Номер счета: ****{lastFourDigits}\n" +
                   $"Полный номер: {Id}\n" +
                   $"Статус: {Status}\n" +
                   $"Баланс: {balance} {Currency}\n" +
                   $"Лимит овердрафта: {OverdraftLimit} {Currency}\n" +
                   $"Доступно овердрафта: {GetAvailableOverdraft()} {Currency}\n" +
                   $"Дневной лимит снятия: {WithdrawalLimit} {Currency}\n" +
                   $"Снято сегодня: {_todayWithdrawn} {Currency}\n" +
                   $"Фиксированная комиссия: {FixedCommission} {Currency}\n" +
                   $"Валюта: {Currency}";
        }

        public override string ToString()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            decimal balance = GetBalance();
            string overdraftInfo = balance < 0 ? $" (ОВЕРДРАФТ: {Math.Abs(balance)})" : "";

            return $"{GetType().Name} - {OwnerName} (****{lastFourDigits}) | " +
                   $"Статус: {Status} | Баланс: {balance} {Currency}{overdraftInfo} | " +
                   $"Лимит: {WithdrawalLimit} | Комиссия: {FixedCommission}";
        }
    }
}