using BankingSystem.Enums;
using BankingSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankingSystem.Models
{
    public class InvestmentAsset
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal AnnualYieldPercent { get; set; }

        public decimal GetValue() => Quantity * CurrentPrice;
        public decimal GetYearlyIncome() => GetValue() * (AnnualYieldPercent / 100);

        public override string ToString()
        {
            return $"{Name} ({Type}): {Quantity} шт. по {CurrentPrice} = {GetValue()} | Доходность: {AnnualYieldPercent}%";
        }
    }

    public class InvestmentAccount : BankAccount
    {
        public List<InvestmentAsset> Portfolio { get; private set; }
        public decimal InvestmentBalance { get; private set; }
        public decimal TotalPortfolioValue => Portfolio.Sum(a => a.GetValue());

        public InvestmentAccount(string ownerName, Currency currency, decimal initialBalance = 0)
            : base(ownerName, currency, initialBalance)
        {
            Portfolio = new List<InvestmentAsset>();
            InvestmentBalance = 0;
        }

        public InvestmentAccount(string id, string ownerName, Currency currency, decimal initialBalance = 0)
            : base(id, ownerName, currency, initialBalance)
        {
            Portfolio = new List<InvestmentAsset>();
            InvestmentBalance = 0;
        }

        public void TransferToInvestment(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            if (GetBalance() < amount)
                throw new InsufficientFundsError();

            // Напрямую работаем с балансом
            Balance -= amount;
            InvestmentBalance += amount;
            Console.WriteLine($"Счет {Id}: переведено {amount} {Currency} на инвестиционный баланс. Основной баланс: {Balance} {Currency}");
        }

        public void TransferFromInvestment(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            if (InvestmentBalance < amount)
                throw new InvalidOperationError("Недостаточно средств на инвестиционном балансе");

            InvestmentBalance -= amount;
            Balance += amount;
            Console.WriteLine($"Счет {Id}: выведено {amount} {Currency} с инвестиционного баланса. Основной баланс: {Balance} {Currency}");
        }

        public void BuyAsset(InvestmentAsset asset)
        {
            ValidateAccountStatusForOperation();

            decimal cost = asset.GetValue();
            if (InvestmentBalance < cost)
                throw new InvalidOperationError($"Недостаточно средств для покупки. Требуется: {cost} {Currency}, доступно: {InvestmentBalance} {Currency}");

            InvestmentBalance -= cost;
            Portfolio.Add(asset);
            Console.WriteLine($"Счет {Id}: куплен актив {asset.Name} на сумму {cost} {Currency}");
        }

        public void SellAsset(string assetName, decimal quantity)
        {
            ValidateAccountStatusForOperation();

            var asset = Portfolio.FirstOrDefault(a => a.Name == assetName);
            if (asset == null)
                throw new InvalidOperationError($"Актив {assetName} не найден в портфеле");

            if (asset.Quantity < quantity)
                throw new InvalidOperationError($"Недостаточно актива для продажи. Доступно: {asset.Quantity}");

            decimal saleValue = quantity * asset.CurrentPrice;
            InvestmentBalance += saleValue;
            asset.Quantity -= quantity;

            if (asset.Quantity == 0)
                Portfolio.Remove(asset);

            Console.WriteLine($"Счет {Id}: продано {quantity} {assetName} на сумму {saleValue} {Currency}");
        }

        public void UpdateAssetPrice(string assetName, decimal newPrice)
        {
            var asset = Portfolio.FirstOrDefault(a => a.Name == assetName);
            if (asset != null)
            {
                asset.CurrentPrice = newPrice;
                Console.WriteLine($"Обновлена цена {assetName}: {newPrice} {Currency}");
            }
        }

        public decimal ProjectYearlyGrowth()
        {
            return Portfolio.Sum(a => a.GetYearlyIncome());
        }

        public decimal ProjectPortfolioValueInYear()
        {
            return TotalPortfolioValue + ProjectYearlyGrowth();
        }

        public override void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccountStatusForOperation();

            if (GetBalance() < amount)
                throw new InsufficientFundsError();

            // Напрямую уменьшаем баланс
            Balance -= amount;
            Console.WriteLine($"Счет {Id}: успешно снято {amount} {Currency}. Новый баланс: {Balance} {Currency}");
        }

        public string GetPortfolioReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== ИНВЕСТИЦИОННЫЙ ПОРТФЕЛЬ ===");

            if (Portfolio.Count == 0)
            {
                sb.AppendLine("Портфель пуст");
            }
            else
            {
                foreach (var asset in Portfolio)
                {
                    sb.AppendLine($"  • {asset}");
                }
                sb.AppendLine($"\nОбщая стоимость портфеля: {TotalPortfolioValue} {Currency}");
                sb.AppendLine($"Проектированная годовая доходность: {ProjectYearlyGrowth()} {Currency}");
            }

            sb.AppendLine($"Свободный инвестиционный баланс: {InvestmentBalance} {Currency}");
            return sb.ToString();
        }

        public override string GetAccountInfo()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            decimal mainBalance = GetBalance();
            decimal projectedGrowth = ProjectYearlyGrowth();

            var sb = new StringBuilder();
            sb.AppendLine($"Информация об инвестиционном счете:");
            sb.AppendLine($"Тип счета: {GetType().Name}");
            sb.AppendLine($"Владелец: {OwnerName}");
            sb.AppendLine($"Номер счета: ****{lastFourDigits}");
            sb.AppendLine($"Полный номер: {Id}");
            sb.AppendLine($"Статус: {Status}");
            sb.AppendLine($"Основной баланс: {mainBalance} {Currency}");
            sb.AppendLine($"Инвестиционный баланс: {InvestmentBalance} {Currency}");
            sb.AppendLine($"Общая стоимость активов: {TotalPortfolioValue} {Currency}");
            sb.AppendLine($"Общие активы: {mainBalance + InvestmentBalance + TotalPortfolioValue} {Currency}");
            sb.AppendLine($"Количество активов в портфеле: {Portfolio.Count}");
            sb.AppendLine($"Проектированная годовая доходность: {projectedGrowth:F2} {Currency}");
            sb.AppendLine($"Валюта: {Currency}");
            sb.AppendLine("\n" + GetPortfolioReport());

            return sb.ToString();
        }

        public override string ToString()
        {
            string lastFourDigits = Id.Length >= 4 ? Id.Substring(Id.Length - 4) : Id;
            return $"{GetType().Name} - {OwnerName} (****{lastFourDigits}) | " +
                   $"Статус: {Status} | Основной: {GetBalance()} {Currency} | " +
                   $"Инвестиции: {InvestmentBalance} | Активы: {TotalPortfolioValue} ({Portfolio.Count} шт.)";
        }
    }
}