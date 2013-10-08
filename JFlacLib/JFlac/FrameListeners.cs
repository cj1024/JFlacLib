using System.Collections.Generic;

namespace JFlacLib.JFlac
{
    public class FrameListeners : IFrameListener
    {
        private readonly HashSet<IFrameListener> _frameListeners = new HashSet<IFrameListener>();

        public void AddFrameListener(IFrameListener listener)
        {
            lock (_frameListeners)
            {
                if (!_frameListeners.Contains(listener))
                _frameListeners.Add(listener);
            }
        }
        
        public void RemoveFrameListener(IFrameListener listener)
        {
            lock (_frameListeners)
            {
                if(_frameListeners.Contains(listener))
                    _frameListeners.Remove(listener);
            }
        }

        public void ProcessMetadata(Metadata.Metadata metadata)
        {
            lock (_frameListeners)
            {
                var it = _frameListeners.GetEnumerator();
                while (it.MoveNext())
                {
                    var listener = it.Current;
                    if (listener != null) listener.ProcessMetadata(metadata);
                }
            }
        }

        public void ProcessFrame(ref Frame.Frame frame)
        {
            lock (_frameListeners)
            {
                var it = _frameListeners.GetEnumerator();
                while (it.MoveNext())
                {
                    var listener = it.Current;
                    if (listener != null) listener.ProcessFrame(ref frame);
                }
            }
        }

        public void ProcessError(string msg)
        {
            lock (_frameListeners)
            {
                var it = _frameListeners.GetEnumerator();
                while (it.MoveNext())
                {
                    var listener = it.Current;
                    if (listener != null) listener.ProcessError(msg);
                }
            }
        }
    }
}
