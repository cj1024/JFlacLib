using System;

namespace WPJFlacLib.JFlac.Metadata
{
    internal class Violation : Exception
    {
        public Violation()
        {
        }

        public Violation(String msg) : base(msg)
        {
        }

        public Violation(Exception err)
            : base("Violation", err)
        {
        }

        public Violation(String msg, Exception err) : base(msg, err)
        {
        }
    }
}