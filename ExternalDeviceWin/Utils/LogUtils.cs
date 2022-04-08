namespace ExternalDeviceWin.Utils
{
    internal static class LogUtils
    {
        private static ILoggerFactory _loggerFactory { get; }
        static LogUtils()
        {
            _loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        }

        internal static ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();
        internal static ILogger CreateLogger(string name) => _loggerFactory.CreateLogger(name);
    }
}
