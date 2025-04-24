using System;

namespace BNJMO
{
    [Serializable]
    public class IntegerBEHandle : AbstractBEHandle
    {
        public int Integer;

        public IntegerBEHandle()
        {
            //BEHandleType = BEHandleType.TEST;
            //DebugMessage = "" + Integer;
        }

        public IntegerBEHandle(int integer)
        {
            //BEHandleType = BEHandleType.TEST;
            Integer = integer;
            //DebugMessage = "" + Integer;
        }

        public override string GetDebugMessage()
        {
            return "Integer : " + Integer;
        }
    }
}
