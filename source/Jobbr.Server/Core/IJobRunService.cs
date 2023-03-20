using System.Collections.Generic;
using System.IO;
using Jobbr.Server.Core.Models;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Interface for job run services.
    /// </summary>
    public interface IJobRunService
    {
        /// <summary>
        /// Update the progress of a job.
        /// </summary>
        /// <param name="jobRunId">ID for the job run.</param>
        /// <param name="progress">How far the job has progressed.</param>
        void UpdateProgress(long jobRunId, double progress);

        /// <summary>
        /// Update the state of a job.
        /// </summary>
        /// <param name="jobRunId">The ID of the job run.</param>
        /// <param name="state">The new state for the job.</param>
        void UpdateState(long jobRunId, JobRunStates state);

        /// <summary>
        /// Gets job artifacts.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <returns>A list of <see cref="JobArtefactModel"/>s. List is empty if none are found or an error is thrown in the process.</returns>
        List<JobArtefactModel> GetArtefactsByJobRunId(long jobRunId);

        /// <summary>
        /// Gets a <see cref="Stream"/> of artifacts for the job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <param name="filename">Target file to stream to.</param>
        /// <returns>An artifact <see cref="Stream"/> pointed towards the file. Null if none are found or error is thrown in the process.</returns>
        Stream GetArtefactAsStream(long jobRunId, string filename);

        /// <summary>
        /// Adds an artifact to a job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <param name="fileName">Filename of the file containing an artifact.</param>
        /// <param name="result">Result <see cref="Stream"/>.</param>
        void AddArtefact(long jobRunId, string fileName, Stream result);

        /// <summary>
        /// Updates the process ID of the job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <param name="processId">New process ID.</param>
        void UpdatePid(long jobRunId, int processId);

        /// <summary>
        /// Deletes a job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        void Delete(long jobRunId);
    }
}