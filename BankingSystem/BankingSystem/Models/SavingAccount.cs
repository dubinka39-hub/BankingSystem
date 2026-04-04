using BankingSystem.Enums;
using BankingSystem.Exceptions;
using System;

namespace BankingSystem.Models
{
    public class SavingAccount : BankAccount
    {
        public decimal MinBalance { get; private set; }
        public decimal MonthlyInterestRate { get; private set; }

        public SavingAccount(string ownerName, Currency currency, decimal monthlyInterestRate,
            decimal minBalance = 0, decimal initialBalance = 0)
            : base(ownerName, currency, initialBalance)
        {
            if (monthlyInterestRate < 0)
                throw new InvalidOperationError("Процентная ставка не может быть отрицательной");
            if (minBalance < 0)
                throw new InvalidOperationError("Минимальный баланс не может быть отрицательным");
            if (initialBalance < minBalance)
                throw new InvalidOperationError($"Начальный баланс не может быть меньше минимального ({minBalance})");

            MinBalance = minBalance;
            MonthlyInterestRate = monthlyInterestRate;
        }

        public SavingAccount(string id, string ownerName, Currency currency, decimal monthlyInterestRate,
            decimal minBalance = 0, decimal initialBalance = 0)
            : base(id, ownerName, currency, initialBalance)
        {
            if (monthlyInterestRate < 0)
                throw new InvalidOperationError("Процентная ставка не может быть отрицательной");
            if (minBalance < 0)
                throw new InvalidOperationError("Минимальный баланс не может быть отрицательным");
            if (initialBalance < minBalance)
                throw new InvalidOperationError($"Начальный баланс не может быть меньше минимального ({minBalance})");

            MinBalance = minBalance;
            MonthlyInterestRate = monthlyInterestRate;
        }

        public override void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            decimal newBalance = GetBalance() - amount;
            if (newBalance < MinBalance)
                throw new InvalidOperationError($"Снятие невозможно: баланс не может быть меньше минимального ({MinBalance} {Currency})");

            // Напрямую уменьшаем баланс, минуя проверку базового класса
            Balance -= amount;
            Console.WriteLine($"Счет {Id}: успешно снято {amount} {Currency}. Новый баланс: {Balance} {Currency}");
        }

        public decimal CalculateMonthlyProfit()
        {
            decimal balance = GetBalance();
            return balance * (MonthlyInterestRate / 100);
        }

        public void ApplyMonthlyInterest()
        {
            ValidateAccountStatusForOperation();
            decimal profit = CalculateMonthlyProfit();
            if (profit > 0)
            {
                // ИСПРАВЛЕНО: напрямую увеличиваем баланс, не вызывая base.Deposit
                Balance += profit;
                Console.WriteLine($"Счет {Id}: начислены проценты {profit:F2} {Currency} (ставка {MonthlyInterestRate}%). Новый баланс: {Balance} {Currency}");
            }
        }

        public override string GetAccountInfo()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            decimal projectedProfit = CalculateMonthlyProfit();

            return $"Информация о сберегательном счете:\n" +
                   $"Тип счета: {GetType().Name}\n" +
                   $"Владелец: {OwnerName}\n" +
                   $"Номер счета: ****{lastFourDigits}\n" +
                   $"Полный номер: {Id}\n" +
                   $"Статус: {Status}\n" +
                   $"Баланс: {GetBalance()} {Currency}\n" +
                   $"Минимальный баланс: {MinBalance} {Currency}\n" +
                   $"Месячная ставка: {MonthlyInterestRate}%\n" +
                   $"Проектированная прибыль за месяц: {projectedProfit:F2} {Currency}\n" +
                   $"Валюта: {Currency}";
        }

        public override string ToString()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            return $"{GetType().Name} - {OwnerName} (****{lastFourDigits}) | " +
                   $"Статус: {Status} | Баланс: {GetBalance()} {Currency} | " +
                   $"Мин. баланс: {MinBalance} | Ставка: {MonthlyInterestRate}%";
        }
    }
}