using System.ComponentModel;

namespace MBD.BankAccounts.Domain.Enumerations
{
    public enum AccountType
    {
        [Description("Conta Corrente")]
        CheckingAccount,

        [Description("Dinheiro")]
        Money,

        [Description("Conta poupança")]
        SavingsAccount,

        [Description("Investimento")]
        Investment,

        [Description("Outros")]
        Others
    }
}