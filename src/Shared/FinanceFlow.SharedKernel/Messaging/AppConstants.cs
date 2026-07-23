namespace FinanceFlow.SharedKernel.Messaging;

public static class AppConstants
{
    public static class RabbitMq
    {
        public const string ExchangeName = "financeflow-exchange";

        public static class RoutingKeys
        {
            public const string UserCreated = "user.created";
            public const string UserUpdated = "user.updated";
            public const string TransactionCreated = "transaction.created";
        }
    }
}
