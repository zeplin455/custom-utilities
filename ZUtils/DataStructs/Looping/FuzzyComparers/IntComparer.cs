using System;
using System.Collections.Generic;
using System.Text;

namespace ZUtils.DataStructs.Looping.FuzzyComparers
{
    public class IntComparer : IFuzzyComparer<int>
    {
        private int Range;
        public IntComparer(int InRange)
        {
            Range = InRange;
        }

        public FuzzyCompareResult CompareItems(int subject, int comparedTo)
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
                } else
                {
                    return FuzzyCompareResult.GreaterThanWithinMargins;
                }
            }else
            {
                if(subject < comparedTo)
                {
                    return FuzzyCompareResult.SmallerThan;
                }else
                {
                    return FuzzyCompareResult.GreaterThan;
                }
            }
        }
    }
}
