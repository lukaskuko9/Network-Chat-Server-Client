namespace ChatApp.Logger
{
    public static class Logger
    {
        public static ILogger Instance = new ConsoleLogger();
    }
}
