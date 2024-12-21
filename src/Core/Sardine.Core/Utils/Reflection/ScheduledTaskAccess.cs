using System.Reflection;

namespace Sardine.Core.Utils.Reflection
{
    // https://stackoverflow.com/questions/29549854/how-can-i-monitor-the-task-queues-in-the-net-taskschedulers-across-appdomain
    public static class ScheduledTaskAccess
    {
        public static Task[] GetScheduledTasksForDebugger(this TaskScheduler ts)
        {
            ArgumentNullException.ThrowIfNull(ts);

            var mi = ts.GetType().GetMethod("GetScheduledTasksForDebugger", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (mi == null)
                return [];

            return (Task[])mi.Invoke(ts, [])!;
        }
        public static TaskScheduler[] GetTaskSchedulersForDebugger()
        {
            var mi = typeof(TaskScheduler).GetMethod("GetTaskSchedulersForDebugger", BindingFlags.NonPublic | BindingFlags.Static);

            if (mi == null)
                return [];

            return (TaskScheduler[])mi.Invoke(null, [])!;
        }
    }
}
