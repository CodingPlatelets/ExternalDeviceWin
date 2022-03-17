using ExternalDeviceWin.Entites;
using System;

namespace ExternalDeviceWin.Utils
{
    public class FileUtils
    {
        public static readonly string FileFolder = null!;
        private static readonly Random random = null!;
        private static readonly string folderName = null!;
        private static readonly ILogger _logger;

        static FileUtils()
        {
            _logger = LogUtils.CreateLogger(nameof(FileUtils));
            random = new Random();
            folderName = @"\tempFiles";
            FileFolder = Directory.GetCurrentDirectory() + folderName;
        }

        public static string saveFile(Stream fs,string? fileName,FileTypes type)
        {
            if(type != FileTypes.PDF)
            {
                _logger.LogWarning("Currently, we only support pdf file");
                return string.Empty;
            }
            return savePdf(fs, fileName);
        }
        private static string savePdf(Stream fs, string? fileName)
        {
            if (!Directory.Exists(FileFolder))
            {
                Directory.CreateDirectory(FileFolder);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = RandomString();
            }
            using var fileStream = File.Create(FileFolder + $@"\{fileName}.pdf");
            if (fs.CanSeek && fileStream.CanRead && fileStream.CanWrite)
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.CopyTo(fileStream);
                fileStream.Close();
                return FileFolder + $@"\{fileName}.pdf";
            }
            return string.Empty;
        }

        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool DeleteFile(string path)
        {
            if (File.Exists(path)) { File.Delete(path); return true; }
            else { return false; }
        }
        
    }
}
