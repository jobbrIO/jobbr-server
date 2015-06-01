using System.IO;
using Demo.Common;

namespace Demo.MyJobs
{
    public class UserSpecificJob
    {
        private readonly IUserResolver userResolver;

        public UserSpecificJob(IUserResolver userResolver)
        {
            this.userResolver = userResolver;
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            File.WriteAllText("userinfo.txt", "Got the user '" + this.userResolver.GetUserName() + "!'");
        }
    }
}
