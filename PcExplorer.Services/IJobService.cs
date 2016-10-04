using PcExplorer.Services.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace PcExplorer.Services
{
    public interface IJobManager<T> where T:class
    {
        T GetJobResult(int jobId);
        int PostJob(Task<T> job, CancellationTokenSource tokenSource);
        void CancelJob(int jobId);
        JobStatus GetJobStatus(int jobId);
    }
}
