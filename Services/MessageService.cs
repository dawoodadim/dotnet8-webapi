namespace MiddlewareFilterDI.Services
{
    public class MessageService : IMessageService 
    {
        public string GetMessage()
        {
            return "Hello from MessageService (DI Working)";
        }
    }
}
