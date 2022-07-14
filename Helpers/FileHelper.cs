using System;
using System.IO;

namespace ACCess.Helpers
{
    public static class FileHelper
    {
        public static string GetDefaultDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Assetto Corsa Competizione", "Config");
        }
    }
}
