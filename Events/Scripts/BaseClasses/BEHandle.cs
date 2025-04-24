using System;

namespace BNJMO
{
    /// <summary>
    /// A generic event handle with no parameter
    /// </summary>
    [Serializable]
    public class BEHandle : AbstractBEHandle
    {
        public BEHandle() : base()
        {
        }

        public override string GetDebugMessage()
        {
            return "";
        }
    }

    /// <summary>
    /// A generic event handle with 1 parameter
    /// </summary>
    [Serializable]
    public class BEHandle<A> : AbstractBEHandle
    {
        public A Arg1 { get; set; }

        public BEHandle() : base()
        {
        }

        public BEHandle(A arg1) : base()
        {
            Arg1 = arg1;
        }

        public override string GetDebugMessage()
        {
            return Arg1.ToString();
        }
    }

    /// <summary>
    /// A generic event handle with 2 parameters
    /// </summary>
    [Serializable]
    public class BEHandle<A, B> : AbstractBEHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }

        public BEHandle() : base()
        {
        }

        public BEHandle(A arg1, B arg2) : base()
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override string GetDebugMessage()
        {
            return Arg1.ToString() + " | " + Arg2.ToString();
        }
    }

    /// <summary>
    /// A generic event handle with 3 parameters
    /// </summary>
    [Serializable]
    public class BEHandle<A, B, C> : AbstractBEHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }
        public C Arg3 { get; set; }

        public BEHandle() : base()
        {
        }

        public BEHandle(A arg1, B arg2, C arg3) : base()
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        public override string GetDebugMessage()
        {
            return Arg1.ToString() + " | " + Arg2.ToString() + " | " + Arg3.ToString();
        }
    }

    /// <summary>
    /// A generic event handle with 4 parameters
    /// </summary>
    [Serializable]
    public class BEHandle<A, B, C, D> : AbstractBEHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }
        public C Arg3 { get; set; }
        public D Arg4 { get; set; }

        public BEHandle() : base()
        {
        }

        public BEHandle(A arg1, B arg2, C arg3, D arg4) : base()
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        public override string GetDebugMessage()
        {
            return Arg1.ToString() + " | " + Arg2.ToString() + " | " + Arg3.ToString() + " | " + Arg4.ToString();
        }
    }

    /// <summary>
    /// A generic event handle with 5 parameters
    /// </summary>
    [Serializable]
    public class BEHandle<A, B, C, D, E> : AbstractBEHandle
    {
        public A Arg1 { get; set; }
        public B Arg2 { get; set; }
        public C Arg3 { get; set; }
        public D Arg4 { get; set; }
        public E Arg5 { get; set; }

        public BEHandle() : base()
        {
        }

        public BEHandle(A arg1, B arg2, C arg3, D arg4, E arg5) : base()
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        public override string GetDebugMessage()
        {
            return Arg1.ToString() + " | " + Arg2.ToString() + " | " + Arg3.ToString() + " | " + Arg4.ToString() + " | " + Arg5.ToString();
        }
    }
}
