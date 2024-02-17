using System;
using System.Diagnostics;
using System.IO;

namespace Project1
{
    internal class Program
    {
        // credit to stack overflow for image extensions: https://stackoverflow.com/questions/670546/determine-if-file-is-an-image
        // author: bendewey
        // date: Mar 22, 2009 at 4:23
        public static readonly List<string> ImageExtensions = new List<string>
        {
            ".JPG",
            ".JPEG",
            ".JPE",
            ".BMP",
            ".GIF",
            ".PNG"
        };

        static void Main(string[] args)
        {
            Counter counter = new Counter();

            try
            {
                if (args.Contains("-b"))
                {
                    Console.WriteLine("-b is Both");
                }
                else if (args.Contains("-d"))
                {
                    Console.WriteLine("-d is Parallel");
                }
                else if (args.Contains("-s"))
                {
                    string path = args.Length >= 3 ? args[2] : "C:\\";
                    counter.Reset();
                    Console.WriteLine("Directory `{0}`", path);

                    var stopwatch = Stopwatch.StartNew();
                    SingleDFS(path, counter);
                    string ts = stopwatch.Elapsed.TotalSeconds.ToString();

                    Console.WriteLine("Sequential Calculated in: {0}s", ts);
                    counter.Print();
                }
                else
                {
                    Console.WriteLine(
                        "Usage: du [-s] [-d] [-b] <path>\n"
                            + "Summarize disk usage of the set of FILEs, recursively for directories.\n\n"
                            + "You MUST specify one of the parameters, -s, -d, or -b\n"
                            + "-s       Run in single threaded mode\n"
                            + "-d       Run in parallel mode (uses all available processors)\n"
                            + "-b       Run in both single threaded and parallel mode.\n"
                            + "         Runs parallel follow by sequential mode\n"
                    );
                }
                ;
            }
            catch
            {
                Console.WriteLine(
                    "Usage: du [-s] [-d] [-b] <path>\n"
                        + "Summarize disk usage of the set of FILEs, recursively for directories.\n\n"
                        + "You MUST specify one of the parameters, -s, -d, or -b\n"
                        + "-s       Run in single threaded mode\n"
                        + "-d       Run in parallel mode (uses all available processors)\n"
                        + "-b       Run in both single threaded and parallel mode.\n"
                        + "         Runs parallel follow by sequential mode\n"
                );
            }
            ;
        }

        static void ParallelDFS(string dir, Counter counter)
        {
            try
            {
                // for each obj in directory
                //      obj == folder
                //           call SingleDFS()
                //           call Counter Class to update folder count
                //      obj == file
                //           if image
                //              call Counter to update image_count, image_bytes
                //           call Counter class to update file_count, total
                //
            }
            catch
            {
                Console.WriteLine(
                    "Usage: du [-s] [-d] [-b] <path>\n"
                        + "Summarize disk usage of the set of FILEs, recursively for directories.\n\n"
                        + "You MUST specify one of the parameters, -s, -d, or -b\n"
                        + "-s       Run in single threaded mode\n"
                        + "-d       Run in parallel mode (uses all available processors)\n"
                        + "-b       Run in both single threaded and parallel mode.\n"
                        + "         Runs parallel follow by sequential mode\n"
                );
            }
            ;
        }

        static void SingleDFS(string dir, Counter counter)
        {
            try
            {
                foreach (string fi in Directory.GetFileSystemEntries(dir))
                {
                    if (Directory.Exists(fi))
                    {
                        counter.UpdateFolderCount(1);
                        SingleDFS(fi, counter);
                    }
                    if (File.Exists(fi))
                    {
                        FileInfo fileInfo = new FileInfo(fi);
                        if (ImageExtensions.Contains(Path.GetExtension(fi).ToUpper()))
                        {
                            counter.UpdateImageCount(1);
                            counter.UpdateImageByteCount(fileInfo.Length);
                        }
                        counter.UpdateTotalBytesCount(fileInfo.Length);
                        counter.UpdateFileCount(1);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                counter.UpdateUnauthorizedCount(1);
            }
            catch
            {
                Console.WriteLine(
                    "Usage: du [-s] [-d] [-b] <path>\n"
                        + "Summarize disk usage of the set of FILEs, recursively for directories.\n\n"
                        + "You MUST specify one of the parameters, -s, -d, or -b\n"
                        + "-s       Run in single threaded mode\n"
                        + "-d       Run in parallel mode (uses all available processors)\n"
                        + "-b       Run in both single threaded and parallel mode.\n"
                        + "         Runs parallel follow by sequential mode\n"
                );
            }
            ;
        }
    }

    internal class Counter
    {
        private int folder_count = 0;
        private int file_count = 0;
        private long total_bytes = 0;
        private long image_bytes = 0;
        private int image_count = 0;
        private int unauthorized_count = 0;

        public void Reset()
        {
            folder_count = 0;
            file_count = 0;
            total_bytes = 0;
            image_bytes = 0;
            image_count = 0;
            unauthorized_count = 0;
        }

        public void Print()
        {
            Console.WriteLine(
                "{0} folders, {1} files, {2} bytes",
                folder_count.ToString("N0"),
                file_count.ToString("N0"),
                total_bytes.ToString("N0")
            );
            GetImagePrintText();
        }

        private void GetImagePrintText()
        {
            if (image_count == 0)
            {
                Console.WriteLine("No image files found in the directory");
            }
            else
            {
                Console.WriteLine(
                    "{0} image files, {1} bytes",
                    image_count.ToString("N0"),
                    image_bytes.ToString("N0")
                );
            }
            ;
        }

        public void UpdateFolderCount(int count)
        {
            folder_count += count;
        }

        public void UpdateFileCount(int count)
        {
            file_count += count;
        }

        public void UpdateTotalBytesCount(long count)
        {
            total_bytes += count;
        }

        public void UpdateImageCount(int count)
        {
            image_count += count;
        }

        public void UpdateImageByteCount(long count)
        {
            image_bytes += count;
        }

        public void UpdateUnauthorizedCount(int count)
        {
            unauthorized_count += count;
        }
    }
}
