using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DiaryJournal.Net
{
    public static class TRNGGenerator
    {
        public static byte[] GetBytes(int size)
        {
            bool[] bits = GetBits(size * 8);
            return BitsToByteArray(bits);
        }
        public static bool[] GetBits(int total)
        {
            Boolean[] bits = new Boolean[total];
            for (int i = 0; i < total; i++)
            {
                bits[i] = Generate_TRNG_BitBoolean();//Generate_TRNG_BitBoolean();
            }
            return bits;
        }

        public static byte[] BitsToByteArray(bool[] bits)
        {
            BitArray a = new BitArray(bits);
            byte[] bytes = new byte[a.Length / 8];
            a.CopyTo(bytes, 0);
            return bytes;
        }

        public static bool Generate_TRNG_BitBoolean()
        {
            var gen1 = 0;
            var gen2 = 0;
            Task.Run(() =>
            {
                while (gen1 < 1 || gen2 < 1)
                    Interlocked.Increment(ref gen1);
            });
            while (gen1 < 1 || gen2 < 1)
                Interlocked.Increment(ref gen2);
            return (gen1 + gen2) % 2 == 0;
        }

    }
}
