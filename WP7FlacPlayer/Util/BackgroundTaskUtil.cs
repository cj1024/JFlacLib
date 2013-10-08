using System;
using System.ComponentModel;

namespace WP7FlacPlayer.Util
{
    public class BackgroundTaskUtil
    {
        private static BackgroundWorker _backgroundWorker;

        /// <summary>
        /// 辅助开始事件，比如可用于计数器和定时器
        /// </summary>
        public static EventHandler<EventArgs> BackgroundTaskStarted;

        /// <summary>
        /// 辅助完成事件
        /// </summary>
        public static EventHandler<EventArgs> BackgroundTaskCompleted;

        public static void RunBackgroundTaskSingleton<T>(Func<T> taskFunc, Action<T> completionAction)
        {
            CancelAsync();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                e.Result = taskFunc();
            };
            _backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                var backgroundTaskFinishedHandler = BackgroundTaskCompleted;
                if (null != backgroundTaskFinishedHandler)
                {
                    backgroundTaskFinishedHandler.Invoke(null, EventArgs.Empty);
                }
                BackgroundTaskCompleted = null;
                if (!_backgroundWorker.WorkerSupportsCancellation)
                {
                    completionAction((T)e.Result);
                }
            };
            var backgroundTaskStartedHandler = BackgroundTaskStarted;
            if (null != backgroundTaskStartedHandler)
            {
                backgroundTaskStartedHandler.Invoke(null, EventArgs.Empty);
            }
            _backgroundWorker.RunWorkerAsync();
        }

        public static void CancelAsync()
        {
            if (_backgroundWorker != null && _backgroundWorker.IsBusy)
            {
                _backgroundWorker.WorkerSupportsCancellation = true;
                _backgroundWorker.CancelAsync();
            }
        }
    }
}
