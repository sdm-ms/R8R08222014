using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Nonmodel_Code
{
    public class GuidManipulation
    {
        public static Guid ChangeBit(Guid originalGuid, int bitIndex, bool isSet)
        {
            // see http://stackoverflow.com/questions/21408109/set-specific-bit-in-byte-array
            Byte[] bytes = originalGuid.ToByteArray();
            int byteIndex = bitIndex / 8;
            int bitInByteIndex = bitIndex % 8; 
            byte mask = (byte)(1 << bitInByteIndex);
            // bool isSet = (bytes[byteIndex] & mask) != 0;
            // set to 1
            if (isSet)
                bytes[byteIndex] |= mask;
            else
                bytes[byteIndex] &= (byte) ~mask;
            return new Guid(bytes);
        }

        public static Guid NewGuidWithNewBitSet()
        {
            return ChangeBit(Guid.NewGuid(), 0, true);
        }

        public static bool IsBitSet(Guid guid, int bitIndex)
        {
            Byte[] bytes = guid.ToByteArray();
            int byteIndex = bitIndex / 8;
            int bitInByteIndex = bitIndex % 8;
            byte mask = (byte)(1 << bitInByteIndex);
            bool isSet = (bytes[byteIndex] & mask) != 0;
            return isSet;
        }
    }
}
