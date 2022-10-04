namespace MBD.BankAccounts.API.Workers
{
    public static class RabbitMqConstants
    {
        public const string TRANSACTION_PAID = "TOPIC/transactions.updated.paid";
        public const string TRANSACTION_UNDO_PAYMENT = "TOPIC/transactions.updated.undo_payment";
        public const string TRANSACTION_VALUE_CHANGED = "TOPIC/transactions.updated.value_changed";
    }
}