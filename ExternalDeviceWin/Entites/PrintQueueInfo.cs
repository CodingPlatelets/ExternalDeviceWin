namespace ExternalDeviceWin.Entites
{
    /// <summary>
    /// Console.WriteLine((from lll in l select new { lll.Name, lll.FullName,lll.IsBusy,lll.IsDirect,lll.IsInError }));
    /// 
    /// </summary>
    public class PrintQueueInfo
    {
        public string Name { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public bool IsBusy { get; set; }
        public bool IsInError { get; set; }
        public bool IsOffLine   { get; set; }
        public bool IsOutOfPapaer { get; set; }
        public bool IsPaperJammed { get; set; }
        public bool IsPaused { get; set; }
        public string ErrMsg { get; set; } = string.Empty;

        public PrintQueueInfo(string name,string fullName,bool isAvailable,
                            bool isBusy, bool isInError,bool isOffLine,bool isOutOfPaper,bool isPaperJammed,bool isPaused,string msg)
        {
            Name = name;
            FullName = fullName;
            IsAvailable = isAvailable;
            IsBusy = isBusy;
            IsInError = isInError;
            IsOffLine = isOffLine;
            IsOutOfPapaer = isOutOfPaper;
            IsPaperJammed = isPaperJammed;
            IsPaused = isPaused;
            ErrMsg = msg;
        }
        public PrintQueueInfo()
        {

        }
    }
}
