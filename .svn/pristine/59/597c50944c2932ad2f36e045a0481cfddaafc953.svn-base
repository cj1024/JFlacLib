using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CJToolkit.Util
{
    public class ThreadUtil
    {
        /// <summary>
        /// 在制定时间过去之后执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="delay">要暂停的时间</param>
        /// <param name="useDispatcher">是否使用Dispatcher回调</param>
        public static void RunAfterDelay(Action action, int delay = 1000, bool useDispatcher = true)
        {
            var thread = new Thread(() =>
            {
                Thread.Sleep(delay);
                if (useDispatcher)
                    Deployment.Current.Dispatcher.BeginInvoke(action);
                else
                    action.Invoke();
            });
            thread.Start();
        }

        /// <summary>
        /// 尝试杀掉一个线程
        /// </summary>
        /// <param name="thread">要杀掉的线程</param>
        /// <returns>成功或失败</returns>
        public static bool TryAbortThread(Thread thread)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(thread.Abort);
            return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryStopTimer(Timer timer)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(timer.Dispose);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryStopTimer(DispatcherTimer timer)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(timer.Stop);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
