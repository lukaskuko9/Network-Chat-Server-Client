using Server.Logger;

namespace Server
{
    public static class Global
    {
        public static ILogger logger = new ConsoleLogger();
    }
}
