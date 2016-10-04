using System.Collections.Generic;
using System.Threading.Tasks;
using PcExplorer.Services.Entities;
using System.IO;
using System;
using System.Linq;
using PcExplorer.Services.Extensions;
using System.Threading;

namespace PcExplorer.Services
{
    public class FileSystemService : IFileSystemService
    {

        public Task<int[]> CountFilesByPredicate(string path, Func<FileInfo, bool>[] predicates, CancellationTokenSource tokenSource)
        {
            var getFileCountTask = Task.Run(() =>
            {
                DirectoryInfo di = new DirectoryInfo(path);
                var countFiles = di.CountAllAcessibleFiles(predicates, tokenSource.Token);
               
                return countFiles;
            },tokenSource.Token);
          
            return getFileCountTask;
        }

        public async Task<DiskDrive[]> GetAllDrives()
        {
            var getAllDrivesTask = Task.Run(() =>
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                var diskDrives = new List<DiskDrive>();
                foreach (var drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        diskDrives.Add(new DiskDrive
                        {
                            Name = drive.Name,
                            Label = drive.VolumeLabel,
                            FileSystem = drive.DriveFormat,
                            AvailableSpace = drive.TotalFreeSpace,
                            TotalSize = drive.TotalSize
                        });
                    }
                }
                return diskDrives.ToArray();
            });

            var drives = await getAllDrivesTask;
            return drives;
        }

        public async Task<DirectoryContents> GetFileAndFolderList(string path)
        {
            var getFileListTask = Task.Run(() =>
            {
                DirectoryInfo di = new DirectoryInfo(path);

                var directoryContents = new DirectoryContents
                {
                    Directories = di.GetDirectories()
                                        .Select(d => d.Name)
                                        .ToArray(),
                    Files = di.GetFiles()
                                         .Select(f => f.Name)
                                         .ToArray()
                };
                return directoryContents;
            });
            var contents = await getFileListTask;
            return contents;
        }
    }
}

