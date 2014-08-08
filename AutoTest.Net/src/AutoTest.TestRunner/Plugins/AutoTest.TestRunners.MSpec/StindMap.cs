using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace LinFu.DynamicProxy
{
    public class StindMap : Dictionary<string, OpCode>
    {
        public StindMap()
        {
            var stindMap = this;
            stindMap["Bool&"] = OpCodes.Stind_I1;
            stindMap["Int8&"] = OpCodes.Stind_I1;
            stindMap["Uint8&"] = OpCodes.Stind_I1;

            stindMap["Int16&"] = OpCodes.Stind_I2;
            stindMap["Uint16&"] = OpCodes.Stind_I2;

            stindMap["Uint32&"] = OpCodes.Stind_I4;
            stindMap["Int32&"] = OpCodes.Stind_I4;

            stindMap["IntPtr"] = OpCodes.Stind_I4;
            stindMap["Uint64&"] = OpCodes.Stind_I8;
            stindMap["Int64&"] = OpCodes.Stind_I8;
            stindMap["Float32&"] = OpCodes.Stind_R4;
            stindMap["Float64&"] = OpCodes.Stind_R8;
        }
    }
}
