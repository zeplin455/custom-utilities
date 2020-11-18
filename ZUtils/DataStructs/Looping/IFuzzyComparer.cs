using System;
using System.Collections.Generic;
using System.Text;

namespace ZUtils.DataStructs.Looping
{
    public enum FuzzyCompareResult
    {
        ExactMatch,
        GreaterThan,
        SmallerThan,
        NotEqualUndefined,
        GreaterThanWithinMargins,
        SmallerThanWithinMargins,
        NotEqualUndefinedWithinMargins
    }
    public interface IFuzzyComparer<T>
    {
        FuzzyCompareResult CompareItems(T subject, T comparedTo);
    }
}
