using BankingSystem.Enums;
using BankingSystem.Exceptions;
using BankingSystem.Models;
using System;

namespace BankingSystem.Services
{
    public static class BankAccountTester
    {
        public static void RunTests()
        {
            Console.WriteLine("=== ТЕСТИРОВАНИЕ БАНКОВСКОЙ СИСТЕМЫ ===\n");

            TestBaseAccount();
            TestSavingAccount();
            TestPremiumAccount();
            TestInvestmentAccount();

            Console.WriteLine("\n=== ВСЕ ТЕСТЫ ЗАВЕРШЕНЫ ===");
        }

        static void TestBaseAccount()
        {
            Console.WriteLine("\n>>> ТЕСТИРОВАНИЕ БАЗОВОГО СЧЕТА <<<\n");

            try
            {
                var account = new BankAccount("Иван Иванов", Currency.RUB, 1000);
                Console.WriteLine(account.GetAccountInfo());
                Console.WriteLine();

                account.Deposit(500);
                account.Withdraw(300);
                account.FreezeAccount();

                try { account.Withdraw(100); }
                catch (AccountFrozenError ex) { Console.WriteLine($"Ожидаемая ошибка: {ex.Message}"); }

                account.UnfreezeAccount();
                account.Withdraw(100);
                Console.WriteLine($"Итоговый баланс: {account.GetBalance()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void TestSavingAccount()
        {
            Console.WriteLine("\n>>> ТЕСТИРОВАНИЕ СБЕРЕГАТЕЛЬНОГО СЧЕТА <<<\n");

            try
            {
                // Создание с минимальным балансом 500 и ставкой 5.5%
                var saving = new SavingAccount("Петр Сидоров", Currency.USD, 5.5m, 500, 1000);
                Console.WriteLine(saving.GetAccountInfo());
                Console.WriteLine();

                // Попытка снять больше чем позволяет мин. баланс
                Console.WriteLine("Попытка снять 600 (мин. баланс 500):");
                try { saving.Withdraw(600); }
                catch (InvalidOperationError ex) { Console.WriteLine($"Ожидаемая ошибка: {ex.Message}"); }

                // Успешное снятие
                Console.WriteLine("\nСнятие 400 (должно работать):");
                saving.Withdraw(400);
                Console.WriteLine($"Баланс: {saving.GetBalance()}, Минимум: {saving.MinBalance}");

                // Расчет прибыли
                Console.WriteLine($"\nПрибыль за месяц: {saving.CalculateMonthlyProfit():F2} {saving.Currency}");

                // Начисление процентов
                saving.ApplyMonthlyInterest();
                Console.WriteLine($"Баланс после начисления: {saving.GetBalance()}");

                Console.WriteLine("\n" + saving.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void TestPremiumAccount()
        {
            Console.WriteLine("\n>>> ТЕСТИРОВАНИЕ ПРЕМИУМ СЧЕТА <<<\n");

            try
            {
                // Создание с овердрафтом 5000, лимитом 10000, комиссией 50
                var premium = new PremiumAccount("Олег Премьер", Currency.EUR, 5000, 10000, 50, 1000);
                Console.WriteLine(premium.GetAccountInfo());
                Console.WriteLine();

                // Обычное снятие
                Console.WriteLine("Снятие 800:");
                premium.Withdraw(800);
                Console.WriteLine();

                // Снятие с переходом в овердрафт
                Console.WriteLine("Снятие 500 (должно уйти в овердрафт):");
                premium.Withdraw(500);
                Console.WriteLine($"Доступно овердрафта: {premium.GetAvailableOverdraft()}");
                Console.WriteLine();

                // Попытка превысить овердрафт
                Console.WriteLine("Попытка снять 5000 (превышение овердрафта):");
                try { premium.Withdraw(5000); }
                catch (InsufficientFundsError ex) { Console.WriteLine($"Ожидаемая ошибка: {ex.Message}"); }

                // Пополнение для выхода из овердрафта
                Console.WriteLine("\nПополнение на 2000:");
                premium.Deposit(2000);
                Console.WriteLine(premium.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void TestInvestmentAccount()
        {
            Console.WriteLine("\n>>> ТЕСТИРОВАНИЕ ИНВЕСТИЦИОННОГО СЧЕТА <<<\n");

            try
            {
                var invest = new InvestmentAccount("Анна Инвестор", Currency.USD, 10000);
                Console.WriteLine(invest.GetAccountInfo());
                Console.WriteLine();

                // Перевод на инвестиционный баланс
                Console.WriteLine("Перевод 5000 на инвестиционный баланс:");
                invest.TransferToInvestment(5000);
                Console.WriteLine($"Основной баланс: {invest.GetBalance()}, Инвестиционный: {invest.InvestmentBalance}");
                Console.WriteLine();

                // Покупка активов
                Console.WriteLine("Покупка активов:");
                var appleStock = new InvestmentAsset
                {
                    Name = "AAPL",
                    Type = "Stocks",
                    Quantity = 10,
                    CurrentPrice = 150,
                    AnnualYieldPercent = 8.5m
                };
                invest.BuyAsset(appleStock);

                var bond = new InvestmentAsset
                {
                    Name = "US Treasury 10Y",
                    Type = "Bonds",
                    Quantity = 5,
                    CurrentPrice = 1000,
                    AnnualYieldPercent = 4.2m
                };
                invest.BuyAsset(bond);

                Console.WriteLine(invest.GetPortfolioReport());
                Console.WriteLine();

                // Расчет годовой доходности
                Console.WriteLine($"Проектированная годовая доходность: {invest.ProjectYearlyGrowth():F2} {invest.Currency}");
                Console.WriteLine($"Стоимость портфеля через год: {invest.ProjectPortfolioValueInYear():F2} {invest.Currency}");
                Console.WriteLine();

                // Продажа части актива
                Console.WriteLine("Продажа 5 акций AAPL:");
                invest.SellAsset("AAPL", 5);
                Console.WriteLine(invest.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}