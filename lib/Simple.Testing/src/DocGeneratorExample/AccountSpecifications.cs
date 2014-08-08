using System;
using System.Collections.Generic;
using System.Linq;
using Simple.Testing.Framework;

namespace DocGeneratorExample
{
    public class AccountSpecifications
    {
        public Specification when_constructing_an_account = new ConstructorSpecification<Account>()
        {
            When = () => new Account("Jane Smith", 17),
            Expect =
                {
                    account => account.AccountHolderName == "Jane Smith",
                    account => account.UniqueIdentifier == 17,
                    account => account.CurrentBalance == new Money(0m),
                    account => account.Transactions.Count() == 0
                }
        };
        public Specification when_depositing_to_a_new_account = new ActionSpecification<Account>()
        {
            Before = () =>SystemTime.Set(new DateTime(2011,1,1)),
            On = () => new Account("Joe User", 14),
            When = account => account.Deposit(new Money(50)),
            Expect = 
                {
                    account => account.CurrentBalance == new Money(50),
                    account => account.Transactions.Count() == 1,
                    account => account.Transactions.First().Amount == new Money(50),
                    account => account.Transactions.First().Type == TransactionType.Deposit,
                    account => account.Transactions.First().Timestamp == new DateTime(2011,1,1),
                },
            Finally = SystemTime.Clear
        };
        public Specification when_withdrawing_to_overdraw_an_account = new FailingSpecification<Account, CannotOverdrawAccountException>()
        {
            On = () => new Account("Joe User", 14),
            When = account => account.Withdraw(new Money(50)),
            Expect = 
                {
                    exception => exception.Message == "The operation would overdraw the account"
                }
        };
        public Specification when_witdrawing_from_account_with_sufficient_funds = new ActionSpecification<Account>()
        {
            Before = () => SystemTime.Set(new DateTime(2011, 1, 1)),
            On = () => new Account("Joe User", 14, new Money(100)),
            When = account => account.Withdraw(new Money(50)),
            Expect = 
                {
                    account => account.CurrentBalance == new Money(50),
                    account => account.Transactions.Count() == 1,
                    account => account.Transactions.First().Amount == new Money(-5),
                    account => account.Transactions.First().Type == TransactionType.Deposit,
                },
            Finally = SystemTime.Clear
        };
    }

     class Account
    {
        public string AccountHolderName { get; private set; }
        public int UniqueIdentifier { get; private set; }
        public Money CurrentBalance { get { return _balance;  } }
        private Money _balance = 0.Dollars();
       
        private List<Transaction> _transactions = new List<Transaction>();
        public IEnumerable<Transaction> Transactions { get { return _transactions; } }

        public Account(string accountHolder, int uniqueIdentifier) : this(accountHolder, uniqueIdentifier, new Money(0))
        {
        }

        public Account(string accountHolder, int uniqueIdentifier, Money startingBalance)
        {
            AccountHolderName = accountHolder;
            UniqueIdentifier = uniqueIdentifier;
            _balance = startingBalance;
        }

        public override string ToString()
        {
            var ret = String.Format("Account: {0} owned by {1} with a balance {2}\n", UniqueIdentifier, AccountHolderName,
                                    CurrentBalance);
            if (_transactions.Count == 0) return ret + "\tWith no previous transactions.";
            ret = _transactions.Aggregate(ret, (current, t) => current + ("\t" + t.ToString() + "\n"));
            return ret;
        }

         public void Deposit(Money amount)
         {
             _balance += amount;
             _transactions.Add(new Transaction(amount, TransactionType.Deposit));
         }

         public void Withdraw(Money amount)
         {
             if(_balance - amount < new Money(0))
             {
                 throw new CannotOverdrawAccountException("The operation would overdraw the account");
             }
             _balance -= amount;
             _transactions.Add(new Transaction(new Money(0) - amount, TransactionType.Deposit));
         }
    }

    internal class CannotOverdrawAccountException : Exception
    {
        public CannotOverdrawAccountException(string message) : base(message)
        {
            
        }
    }

    internal class Transaction
    {
        public Money Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Transaction(Money amount, TransactionType type)
        {
            Amount = amount;
            Type = type;
            Timestamp = SystemTime.GetTime();
        }

        public override string ToString()
        {
            return Type + " for " + Amount + " at " + Timestamp;
        }
    }

    internal static class SystemTime
    {
        private static DateTime setTime = DateTime.MinValue;

        public static void Clear()
        {
            setTime = DateTime.MinValue;
        }

        public static void Set(DateTime toSet)
        {
            setTime = toSet;
        }
        public static DateTime GetTime()
        {
            if (setTime == DateTime.MinValue) return DateTime.Now;
            return setTime;
        }
    }

    struct Money
    {
        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public Money(decimal amount)
        {
            _amount = amount;
        }

        public override string  ToString()
        {
 	         return "$" + Amount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Amount.Equals(((Money) obj).Amount);
        }

        public static bool operator ==(Money c1, Money c2)
        {
            return c1._amount == c2.Amount;
        }

        public static bool operator !=(Money c1, Money c2)
        {
            return !(c1 == c2);
        }

        public static Money operator +(Money c1, Money c2)
        {
            return new Money(c1._amount + c2._amount);
        }

        public static bool operator <(Money c1, Money c2)
        {
            return c1._amount < c2._amount;
        }

        public static bool operator >(Money c1, Money c2)
        {
            return c1._amount > c2._amount;
        }

        public static Money operator -(Money c1, Money c2)
        {
            return new Money(c1._amount - c2._amount);
        }
        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }
    }

    static class Extensions
    {
        public static Money Dollars(this decimal amount)
        {
            return new Money(amount);
        }

        public static Money Dollars(this int amount)
        {
            return new Money(amount);
        }
    }

    enum TransactionType
    {
        Withdrawl,
        Deposit
    }
}
