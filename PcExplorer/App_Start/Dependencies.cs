using PcExplorer.Services;

namespace PcExplorer.App_Start
{
    public static class Dependencies
    {
        public static readonly IJobManager<int[]> JobManager = new JobManager<int[]>();
        public static readonly IFileSystemService FileSystemService = new FileSystemService();

    }
}