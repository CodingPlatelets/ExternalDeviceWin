using ExternalDeviceWin.Entites;
using System;

namespace ExternalDeviceWin.Utils
{
    public static class FileUtils
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

        public static string saveFile(Stream fs, string? fileName, FileTypes type)
        {
            if (type != FileTypes.PDF)
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
                _logger.LogInformation("{File Name} is saved", fileName);
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
            return new($"{DateTime.UtcNow}__"+Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static FileTypes GetStreamExtension(this Stream ms)
        {
            using var msStream = new MemoryStream();
            ms.CopyTo(msStream);
            msStream.Seek(0, SeekOrigin.Begin);
            var bytes = msStream.ToArray();
            msStream.Close();
            if (bytes.Length < 1)
            {
                return FileTypes.Unknown;
            }

            var fileFlag = bytes[0].ToString() + bytes[1];

            switch (fileFlag)
            {
                case " 255216 ":
                    return FileTypes.JPG;

                case " 4946 ":
                case " 104116":
                    return FileTypes.TXT;

                case "3780":
                    return FileTypes.PDF;

                case " 7173 ":
                    return FileTypes.GIF;

                case " 6677 ":
                    return FileTypes.BMP;

                case " 13780 ":
                    return FileTypes.PNG;
                default:
                    return FileTypes.Unknown;
            }
        }
    }

    // public static FileTypes GetStreamExtension(Stream ms)
    // {
    //     using var msStream = new MemoryStream();
    //     ms.CopyTo(msStream);
    //     msStream.Seek(0, SeekOrigin.Begin);
    //     var bytes = msStream.ToArray();
    //     msStream.Close();
    //     if (bytes.Length < 1)
    //     {
    //         return FileTypes.Unknown;
    //     }
    //
    //     string fileFlag = bytes[0].ToString() + bytes[1].ToString();
    //
    //     switch (fileFlag)
    //     {
    //         case " 255216 ":
    //             return FileTypes.JPG;
    //
    //         case " 4946 ":
    //         case " 104116":
    //             return FileTypes.TXT;
    //
    //         case "3780":
    //             return FileTypes.PDF;
    //
    //         case " 7173 ":
    //             return FileTypes.GIF;
    //
    //         case " 6677 ":
    //             return FileTypes.BMP;
    //
    //         case " 13780 ":
    //             return FileTypes.PNG;
    //         default:
    //             return FileTypes.Unknown;
    //     }
    // }
}
