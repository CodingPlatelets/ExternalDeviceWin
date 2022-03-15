namespace ExternalDeviceWin.Utils
{
    internal static class LogUtils
    {
        private static ILoggerFactory LoggerFactory { get; }
        static LogUtils()
        {
            LoggerFactory = new LoggerFactory();
        }
        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        internal static ILogger CreateLogger(string name) => LoggerFactory.CreateLogger(name);
    }
}
