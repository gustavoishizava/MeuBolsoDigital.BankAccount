namespace MBD.BankAccounts.Domain.Resources
{
    public static class ResourceCodes
    {
        public static class Account
        {
            public const string DescriptionEmpty = "Account_Description_Empty";
            public const string DescriptionMaxLength = "Account_Description_MaxLength";
            public const string InitialValueMinValue = "Account_InitialBalance_MinValue";
            public const string DuplicateTransaction = "Account_Duplicate_Transaction";
        }
    }
}