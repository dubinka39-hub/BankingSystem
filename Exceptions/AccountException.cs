using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Exceptions
{
    public abstract class AccountException : Exception
    {
        protected AccountException(string message) : base(message) { }
    }
    public class AccountClosedError : AccountException
    {
        public AccountClosedError() : base("Операция невозможна: счет закрыт") { }
    }
    public class AccountFrozenError : AccountException
    {
        public AccountFrozenError() : base("Операция невозможна: счет заморожен") { }
    }
    public class InsufficientFundsError : AccountException
    {
        public InsufficientFundsError() : base("Недостаточно средств на счете") { }
    }
    public class InvalidOperationError : AccountException
    {
        public InvalidOperationError(string message) : base($"Некорректная операция: {message}") { }
    }
}
