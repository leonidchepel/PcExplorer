using PcExplorer.Services.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PcExplorer.Services
{
    public interface IFileSystemService
    {
        Task<DiskDrive[]> GetAllDrives();
        Task<DirectoryContents> GetFileAndFolderList(string path);
        Task<int[]> CountFilesByPredicate(string path, Func<FileInfo, bool>[] predicates, CancellationTokenSource tokenSource);
    }
}
