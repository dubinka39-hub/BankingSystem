using BankingSystem.Models;
using BankingSystem.Enums;
using BankingSystem.Services;
using BankingSystem.Exceptions;
using System;

namespace BankingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== МОДУЛЬНАЯ БАНКОВСКАЯ СИСТЕМА ===\n");

            // Запуск полного тестирования
            BankAccountTester.RunTests();

            // Дополнительные интерактивные примеры
            RunAdditionalExamples();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void RunAdditionalExamples()
        {
            Console.WriteLine("\n\n=== ДОПОЛНИТЕЛЬНЫЕ ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ ===\n");

            // Пример 1: Сравнение всех типов счетов
            Console.WriteLine("1. СРАВНЕНИЕ ВСЕХ ТИПОВ СЧЕТОВ:");
            Console.WriteLine("--------------------------------");

            var baseAcc = new BankAccount("Базовый Клиент", Currency.RUB, 5000);
            var savingAcc = new SavingAccount("Сберегатель Клиент", Currency.USD, 6.0m, 1000, 5000);
            var premiumAcc = new PremiumAccount("Премиум Клиент", Currency.EUR, 10000, 20000, 30, 5000);
            var investAcc = new InvestmentAccount("Инвестор Клиент", Currency.USD, 20000);

            Console.WriteLine(baseAcc);
            Console.WriteLine(savingAcc);
            Console.WriteLine(premiumAcc);
            Console.WriteLine(investAcc);

            // Пример 2: Демонстрация специфических операций
            Console.WriteLine("\n2. СПЕЦИФИЧЕСКИЕ ОПЕРАЦИИ:");
            Console.WriteLine("--------------------------------");

            // Сберегательный счет - начисление процентов
            Console.WriteLine("\n>>> Сберегательный счет:");
            Console.WriteLine($"До начисления: {savingAcc.GetBalance()}");
            savingAcc.ApplyMonthlyInterest();
            Console.WriteLine($"После начисления: {savingAcc.GetBalance()}");

            // Премиум счет - использование овердрафта
            Console.WriteLine("\n>>> Премиум счет (овердрафт):");
            premiumAcc.Withdraw(6000); // Уходит в минус
            Console.WriteLine(premiumAcc);

            // Инвестиционный счет - работа с портфелем
            Console.WriteLine("\n>>> Инвестиционный счет:");
            investAcc.TransferToInvestment(15000);

            var etf = new InvestmentAsset
            {
                Name = "SPY",
                Type = "ETF",
                Quantity = 20,
                CurrentPrice = 450,
                AnnualYieldPercent = 10.0m
            };
            var crypto = new InvestmentAsset
            {
                Name = "BTC",
                Type = "Crypto",
                Quantity = 0.5m,
                CurrentPrice = 50000,
                AnnualYieldPercent = 25.0m
            };

            investAcc.BuyAsset(etf);
            investAcc.BuyAsset(crypto);

            Console.WriteLine(investAcc.GetPortfolioReport());
            Console.WriteLine($"Общая проектированная доходность: {investAcc.ProjectYearlyGrowth():F2} USD");

            // Пример 3: Обработка ошибок
            Console.WriteLine("\n3. ОБРАБОТКА ОШИБОК:");
            Console.WriteLine("--------------------------------");

            // Попытка создать сберегательный счет с нарушением мин. баланса
            try
            {
                var invalidSaving = new SavingAccount("Ошибка", Currency.RUB, 5, 1000, 500);
            }
            catch (InvalidOperationError ex)
            {
                Console.WriteLine($"Ошибка создания: {ex.Message}");
            }

            // Попытка превысить овердрафт
            try
            {
                var testPremium = new PremiumAccount("Тест", Currency.RUB, 1000, 5000, 10, 500);
                testPremium.Withdraw(2000); // Превышает овердрафт
            }
            catch (InsufficientFundsError ex)
            {
                Console.WriteLine($"Ошибка овердрафта: {ex.Message}");
            }

            Console.WriteLine("\n=== ПРИМЕРЫ ЗАВЕРШЕНЫ ===");
        }
    }
}