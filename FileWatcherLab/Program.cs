using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatcherLab
{
    class Program
    {
        private static object _lock = new object();

        static void Main(string[] args)
        {
            #region 2.監控WatchFolder, 有新增檔案就處理
            //2.監控WatchFolder
            var watchFile = "D:\\Watcher";
            //設定針測的檔案類型，如不指定可設定*.*
            var watch = new FileSystemWatcher(watchFile, "*.*");
            //是否連子資料夾都要偵測
            watch.IncludeSubdirectories = true;
            //設定監控的變更類型
            watch.NotifyFilter = NotifyFilters.FileName;
            //設定緩衝區大小為8M
            watch.InternalBufferSize = 8 * 1024 * 1024;
            //開啟監聽
            watch.EnableRaisingEvents = true;

            Console.WriteLine($"File  Watching..... Start  [{DateTime.Now}]");

            //新增時觸發事件
            watch.Created += (
                (object sender, FileSystemEventArgs e) =>
                {
                    if (e.ChangeType == WatcherChangeTypes.Created)
                    {
                        while (!FileIsReady(e.FullPath))
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("waiting 1 sec");
                        }

                        var des = $"D:\\Temp\\{e.Name}";
                        File.Move(e.FullPath, des);
                        Console.WriteLine($"move to {des} ok ...");
                    }
                });
            #endregion

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
            Console.WriteLine($"File  Watching..... End  [{DateTime.Now}");
        }

        public static bool FileIsReady(string path)
        {
            //One exception per file rather than several like in the polling pattern
            try
            {
                //If we can't open the file, it's still copying
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
