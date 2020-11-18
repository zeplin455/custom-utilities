using System;
using System.Collections.Generic;
using System.Text;

namespace ZUtils.DataStructs.Looping.FuzzyComparers
{
    class ByteComparer : IFuzzyComparer<byte>
    {
        private byte Range;
        public ByteComparer(byte InRange)
        {
            Range = InRange;
        }

        public FuzzyCompareResult CompareItems(byte subject, byte comparedTo)
        {
            int rangeCheck = Math.Abs(subject - comparedTo);

            if (rangeCheck <= Range)
            {
                if (subject == comparedTo)
                {
                    return FuzzyCompareResult.ExactMatch;
                }

                if (subject < comparedTo)
                {
                    return FuzzyCompareResult.SmallerThanWithinMargins;
                }
                else
                {
                    return FuzzyCompareResult.GreaterThanWithinMargins;
                }
            }
            else
            {
                if (subject < comparedTo)
                {
                    return FuzzyCompareResult.SmallerThan;
                }
                else
                {
                    return FuzzyCompareResult.GreaterThan;
                }
            }
        }
    }
}
