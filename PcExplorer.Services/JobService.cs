using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PcExplorer.Services.Enums;
using System.Threading;

namespace PcExplorer.Services
{
    public class JobManager<T> : IJobManager<T> where T : class
    {
        private Dictionary<int, TaskToken> _jobs;
        private class TaskToken
        {
            public Task<T> Task { get; set; }
            public CancellationTokenSource TokenSource { get; set; }
            public T Result { get; set; }
        }

        public JobManager()
        {
            _jobs = new Dictionary<int, TaskToken>();
            
        }

        public void CancelJob(int jobId)
        {
            var job = getJobByIdInternal(jobId);
            _jobs.Remove(jobId);
            if (!job.TokenSource.IsCancellationRequested)
            {
                job.TokenSource.Cancel();
            }
        }

        public T GetJobResult(int jobId)
        {
            var job = getJobByIdInternal(jobId);
            return job.Result;
        }

        public JobStatus GetJobStatus(int jobId)
        {
            var job = getJobByIdInternal(jobId);
            if (job.Task.IsCompleted)
            {
                job.Result = job.Result == null ? job.Task.Result:null;
                return JobStatus.Finished;
            }
            return JobStatus.Running;
        }


        public int PostJob(Task<T> job, CancellationTokenSource tokenSource)
        {
            int jobId = _jobs.Keys.Count == 0 ? 0 : _jobs.Keys.Max() + 1;
            var taskToken = new TaskToken
            {
                Task = job,
                TokenSource = tokenSource,
            };
            _jobs.Add(jobId, taskToken);
            return jobId;
        }



        private TaskToken getJobByIdInternal(int jobId)
        {
            TaskToken job;
            var idExists = _jobs.TryGetValue(jobId, out job);
            if (idExists)
            {
                return job;
            }
            throw new ArgumentOutOfRangeException("No such jobId");
        }

    }
}
