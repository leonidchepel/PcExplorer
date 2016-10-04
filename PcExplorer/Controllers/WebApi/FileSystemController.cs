using PcExplorer.App_Start;
using PcExplorer.Models;
using PcExplorer.Services;
using PcExplorer.Services.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PcExplorer.Controllers.WebApi
{
    [RoutePrefix("api/filesystem")]
    public class FileSystemController : ApiController
    {
        IJobManager<int[]> _filesCountJobManager;
        IFileSystemService _fileSystemService;

        public FileSystemController(IJobManager<int[]> filesCountJobManager, IFileSystemService fileSystemService)
        {
            _filesCountJobManager = filesCountJobManager;
            _fileSystemService = fileSystemService;
        }


        public FileSystemController():this(Dependencies.JobManager, Dependencies.FileSystemService)
        {

        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ListFilesAndFolders()

        {
            var path = HttpUtility.UrlDecode(Url.Request.RequestUri.Query)
                                                .Replace("?", string.Empty);
            if (path == string.Empty)
            {
                var drives = await _fileSystemService.GetAllDrives();
                var drivesAsFolders = convertToDirectoryContents(drives);
                return Ok(drivesAsFolders);
            }
            var filesAndFoldersTask = await _fileSystemService.GetFileAndFolderList(path);
            return Ok(filesAndFoldersTask);
        }

        [HttpPost]
        [Route("count")]
        public IHttpActionResult PostCountFilesInFolder(CountFilesSizeFilterDto[] filter)
        {
            var path = HttpUtility.UrlDecode(Url.Request.RequestUri.Query)
                                                .Replace("?", string.Empty);
            Func<FileInfo, bool>[] predicates = filter.Select(f =>
            {
                Func<FileInfo, bool> predicate = p =>
                    {
                        if (f.ToBytes <= 0)
                        {
                            return p.Length >= f.FromBytes;
                        }
                        return p.Length >= f.FromBytes && p.Length <= f.ToBytes;
                    };
                    return predicate;
            }).ToArray();

            var cancelationToken = new CancellationTokenSource();
            var job = _fileSystemService.CountFilesByPredicate(path, predicates, cancelationToken);
            var jobId = _filesCountJobManager.PostJob(job,cancelationToken);
            return Ok(jobId);
        }

        [HttpGet]
        [Route("count/status/{jobId}")]
        public IHttpActionResult GetJobStatus(int jobId)
        {
            var status = _filesCountJobManager.GetJobStatus(jobId);
            var statusObj = new { status = status.ToString() };
            return Ok(statusObj);
        }



        [HttpDelete]
        [Route("count/{jobId}")]
        public IHttpActionResult CancelJob(int jobId)
        {
            _filesCountJobManager.CancelJob(jobId);
            return Ok();
        }

        [HttpGet]
        [Route("count/{jobId}")]
        public IHttpActionResult GetJobResult(int jobId)
        {
            var result = _filesCountJobManager.GetJobResult(jobId);
            return Ok(result);
        }


        private DirectoryContents convertToDirectoryContents(DiskDrive[] drives)
        {
            return new DirectoryContents
            {
                Directories = drives.Select(d => d.Name).ToArray()
            };
        }

    }
}
