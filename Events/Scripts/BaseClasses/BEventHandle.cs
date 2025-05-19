using System;

namespace BNJMO
{
    /// <summary>
    /// A generic event handle with no parameter
    /// </summary>
    [Serializable]
    public class BEventHandle : AbstractBEventHandle
    {
        public override string GetLog()
        {
            return "";
        }
    }

    /// <summary>
    /// A generic event handle with 1 parameter
    /// </summary>
    [Serializable]
    public class BEventHandle<A> : AbstractBEventHandle
    {
        public A Arg1 { get; set; }

        public BEventHandle()
        {
        }

        public BEventHandle(A arg1)
        {
            Arg1 = arg1;
        }

        public override string GetLog()
        {
            return Arg1.ToString();
        }
    }

    /// <summary>
    /// A generic event handle with 2 parameters
    /// </summary>
    [Serializable]
    public class BEventHandle<A, B> : AbstractBEventHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }

        public BEventHandle()
        {
        }

        public BEventHandle(A arg1, B arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override string GetLog()
        {
            return Arg1 + " | " + Arg2;
        }
    }

    /// <summary>
    /// A generic event handle with 3 parameters
    /// </summary>
    [Serializable]
    public class BEventHandle<A, B, C> : AbstractBEventHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }
        public C Arg3 { get; set; }

        public BEventHandle()
        {
        }

        public BEventHandle(A arg1, B arg2, C arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        public override string GetLog()
        {
            return Arg1 + " | " + Arg2 + " | " + Arg3;
        }
    }

    /// <summary>
    /// A generic event handle with 4 parameters
    /// </summary>
    [Serializable]
    public class BEventHandle<A, B, C, D> : AbstractBEventHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }
        public C Arg3 { get; set; }
        public D Arg4 { get; set; }

        public BEventHandle()
        {
        }

        public BEventHandle(A arg1, B arg2, C arg3, D arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        public override string GetLog()
        {
            return Arg1 + " | " + Arg2 + " | " + Arg3 + " | " + Arg4;
        }
    }

    /// <summary>
    /// A generic event handle with 5 parameters
    /// </summary>
    [Serializable]
    public class BEventHandle<A, B, C, D, E> : AbstractBEventHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }
        public C Arg3 { get; set; }
        public D Arg4 { get; set; }
        public E Arg5 { get; set; }

        public BEventHandle()
        {
        }

        public BEventHandle(A arg1, B arg2, C arg3, D arg4, E arg5)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        public override string GetLog()
        {
            return Arg1 + " | " + Arg2 + " | " + Arg3 + " | " + Arg4 + " | " + Arg5;
        }
    }
}
