using System;

namespace ULIB
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UlibExceptions : Exception
    {
        protected UlibExceptions()
        {
        }

        protected UlibExceptions(string message)
            : base(message)
        {

        }

        protected UlibExceptions(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
