using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace PcExplorer.Services.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static int[] CountAllAcessibleFiles(this DirectoryInfo di, Func<FileInfo, bool>[] predicates, CancellationToken token)
        {
            int[] count = new int[predicates.Length];
            var files = di.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            var filesCount = files.Count();
            for (int i = 0; i < filesCount; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return count;
                }
                try
                {
                    for (int j = 0; j < count.Length; j++)
                    {
                        if (predicates[j](files.ElementAt(i)))
                        {
                            count[j]++;
                        }
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }

            }

            var directories = di.EnumerateDirectories();
            var dirCount = directories.Count();

            for (int d = 0; d < dirCount; d++)
            {
                try
                {
                    var dir = directories.ElementAt(d);
                    var recursiveCount = CountAllAcessibleFiles(dir, predicates,token);
                    for(int c = 0; c < count.Length; c++)
                    {
                        count[c] += recursiveCount[c];
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }

            }



            return count;
        }
    }
}
