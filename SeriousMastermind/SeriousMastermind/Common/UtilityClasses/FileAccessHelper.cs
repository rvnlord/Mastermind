using System;
using System.IO;
using Android.App;

namespace SeriousMastermind.Common.UtilityClasses
{
    public class FileAccessHelper
    {
        public static string GetLocalFilePath(string filename)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbPath = Path.Combine(path, filename);

            CopyDatabaseIfNotExists(filename, dbPath);

            return dbPath;
        }

        private static void CopyDatabaseIfNotExists(string filename, string dbPath)
        {
            if (File.Exists(dbPath))
                return;

            using (var br = new BinaryReader(Application.Context.Assets.Open(filename)))
            {
                using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                {
                    var buffer = new byte[2048];
                    int length;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                        bw.Write(buffer, 0, length);
                }
            }
        }
    }
}