using System;
using Simple.Testing.Framework;

namespace Simple.Testing.Example
{
    /*
     * In an action specification the SUT is returned from the On(). The SUT is then given
     * as a parameter to the When and the Expects. It is expected that the when() will
     * issue some behavior that mutates the SUT which is then sent to the expectations.
     */
    public class ActionSpecifications
    {
        public Specification when_withdrawing_money_from_empty_account = new ActionSpecification<Depositor>
        {
            On = () => new Depositor(13),
            When = depositor => depositor.Withdraw(50.00m),
            Expect =
                                   {
                                       depositor => depositor.Balance > 0.01m,
                                       depositor => depositor.AccountIsOpen,
                                       depositor => depositor.Balance < .50m * GetOverallCount() / 17
                                   },
        };

        private static decimal GetOverallCount()
        {
            return 12;
        }
    }


    public class Depositor
    {
        private readonly int _depositorId;
        public readonly decimal Balance = 50.00m;
        public readonly bool AccountIsOpen = true;
        public void Withdraw(decimal amount)
        {
        }
        public Depositor(int depositorId)
        {
            _depositorId = depositorId;
        }
    }
}
