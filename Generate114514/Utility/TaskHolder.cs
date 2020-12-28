using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generate114514.Utility
{
    public static class TaskHolder
    {
        public static Task<string> task;
        public static CancellationTokenSource cts;
        public static void CancelTasks()
        {
            if (task != null && task.Status == TaskStatus.Running)
            {
                cts.Cancel();
            }
        }
    }
}
