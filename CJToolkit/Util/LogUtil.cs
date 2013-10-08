using System;

namespace CJToolkit.Util
{
    public class LogUtil
    {
        private static DateTime _start, _end;

        public static void Log(string realmessage)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0:yyyyMMdd-hh:mm}:{1}", DateTime.Now, realmessage));
        }
        
        public static void Start()
        {
            _start = DateTime.Now;
        }

        public static void Stop()
        {
            _end = DateTime.Now;
        }

        public static void ShowTimeSpan()
        {
            System.Diagnostics.Debug.WriteLine(string.Format("共用时{0}", (_end - _start).Milliseconds));
        }

    }
}
