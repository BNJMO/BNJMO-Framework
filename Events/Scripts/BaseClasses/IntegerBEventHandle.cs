using System;

namespace BNJMO
{
    [Serializable]
    public class IntegerBEventHandle : AbstractBEventHandle
    {
        public int Integer;

        public IntegerBEventHandle()
        {
            //BEHandleType = BEHandleType.TEST;
            //DebugMessage = "" + Integer;
        }

        public IntegerBEventHandle(int integer)
        {
            //BEHandleType = BEHandleType.TEST;
            Integer = integer;
            //DebugMessage = "" + Integer;
        }

        public override string GetLog()
        {
            return "Integer : " + Integer;
        }
    }
}
