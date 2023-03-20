using System;
using System.Collections.Generic;
using System.Threading;

namespace Jobbr.Server.IntegrationTests
{
    public static class WaitFor
    {
        public static bool HasElements<T>(Func<IList<T>> func, int timeOut = 5000)
        {
            var timedOut = DateTime.UtcNow.AddMilliseconds(timeOut);

            while (DateTime.UtcNow < timedOut)
            {
                var retVal = func();

                if (retVal != null && retVal.Count > 0)
                {
                    return true;
                }

                Thread.Sleep(100);
            }

            return false;
        }

        public static bool HasZeroElements<T>(Func<IList<T>> func, int timeOut = 5000)
        {
            var timedOut = DateTime.UtcNow.AddMilliseconds(timeOut);

            while (DateTime.UtcNow < timedOut)
            {
                var retVal = func();

                if (retVal != null && retVal.Count == 0)
                {
                    return true;
                }

                Thread.Sleep(100);
            }

            return false;
        }

        public static bool MinElements<T>(Func<IList<T>> func, int min, int timeOut = 5000)
        {
            var timedOut = DateTime.UtcNow.AddMilliseconds(timeOut);

            while (DateTime.UtcNow < timedOut)
            {
                var retVal = func();

                if (retVal != null && retVal.Count >= min)
                {
                    return true;
                }

                Thread.Sleep(100);
            }

            return false;
        }
    }
}
