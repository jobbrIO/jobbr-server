using System.Collections.Generic;
using System.Linq;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.Scheduling
{
    internal static class MaxConcurrentJobRunPlaner
    {
        public static List<PlannedJobRun> GetPossiblePlannedJobRuns(IList<ScheduledPlanItem> currentPlan, IJobbrRepository repository)
        {
            var possibleRunsPerJob = GetPossibleRunsPerJob(currentPlan, repository);
            var scheduledPlanItems = GetOrderedScheduledPlanItems(currentPlan);
            var runningJobs = CreatePlannedJobRuns(scheduledPlanItems, possibleRunsPerJob);

            return runningJobs;
        }

        private static Dictionary<long, int> GetPossibleRunsPerJob(IEnumerable<ScheduledPlanItem> currentPlan,
            IJobbrRepository repository)
        {
            var allRunningJobIds = repository.GetRunningJobs().Select(j => j.Job.Id).ToList();
            var allPlannedJobIds = currentPlan.Select(c => c.JobId).ToList();
            var allJobIds = allPlannedJobIds.Union(allRunningJobIds).ToList();

            var possibleRunsPerJob = new Dictionary<long, int>();
            foreach (var jobId in allJobIds)
            {
                var currentRunningJobs = allRunningJobIds.Count(a => a == jobId);
                var maximumJobRuns = repository.GetJob(jobId).MaxConcurrentJobRuns;
                if (maximumJobRuns == 0)
                {
                    maximumJobRuns = int.MaxValue;
                }

                var possibleSlots = maximumJobRuns - currentRunningJobs;
                possibleRunsPerJob.Add(jobId, possibleSlots);
            }

            return possibleRunsPerJob;
        }

        private static Dictionary<long, List<ScheduledPlanItem>> GetOrderedScheduledPlanItems(
            IEnumerable<ScheduledPlanItem> currentPlan)
        {
            return (from item in currentPlan
                orderby item.PlannedStartDateTimeUtc
                group item by item.JobId
                into g
                select g).ToDictionary(k => k.Key, v => v.ToList());
        }

        private static List<PlannedJobRun> CreatePlannedJobRuns(Dictionary<long, List<ScheduledPlanItem>> scheduledPlanItems, IReadOnlyDictionary<long, int> possibleRunsPerJob)
        {
            var runningJobs = new List<PlannedJobRun>();
            foreach (var scheduledPlanItem in scheduledPlanItems)
            {
                for (var i = 0; i < possibleRunsPerJob[scheduledPlanItem.Key] && i < scheduledPlanItem.Value.Count; i++)
                {
                    var plannedJobRun = new PlannedJobRun
                    {
                        Id = scheduledPlanItem.Value[i].Id,
                        PlannedStartDateTimeUtc = scheduledPlanItem.Value[i].PlannedStartDateTimeUtc,
                    };
                    runningJobs.Add(plannedJobRun);
                }
            }

            return runningJobs;
        }
    }
}