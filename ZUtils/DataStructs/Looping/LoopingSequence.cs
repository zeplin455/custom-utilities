using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUtils.DataStructs.Looping
{
    public class LoopingSequence<T> where T : IComparable
    {
        public class SequencePart
        {
            public int SequencePosition { get; private set; }
            public T SequenceValue { get; private set; }

            public SequencePart Next { get; set; } = null;
            public SequencePart Previous { get; set; } = null;
            public SequencePart(int _Position, T _Value)
            {
                SequencePosition = _Position;
                SequenceValue = _Value;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0)
                {
                    int modIndex = index % Indexer.Count;
                    return Indexer[modIndex].SequenceValue;
                }else
                {
                    int modIndex = Math.Abs(index % Indexer.Count);
                    return Indexer[modIndex].SequenceValue;
                }
            }
        }

        private SequencePart[] Parts { get; set; }
        private SortedDictionary<int, SequencePart> Indexer { get; set; } = new SortedDictionary<int, SequencePart>();

        public LoopingSequence(T[] _Sequence)
        {
            if (_Sequence == null || _Sequence.Length < 1)
            {
                throw new Exception("A sequence must contain something");
            }

            SequencePart[] temp = new SequencePart[_Sequence.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = new SequencePart(i, _Sequence[i]);
                Indexer.Add(i, temp[i]);
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (i == 0)
                {
                    temp[i].Previous = temp[temp.Length - 1];
                    temp[i].Next = temp[i + 1];
                }
                else if (i == temp.Length - 1)
                {
                    temp[i].Previous = temp[i - 1];
                    temp[i].Next = temp[0];
                }
                else
                {
                    temp[i].Previous = temp[i - 1];
                    temp[i].Next = temp[i + 1];
                }
            }

            Parts = temp.OrderBy(x => x.SequenceValue).ThenBy(x => x.SequencePosition).ToArray();
        }

        public List<SequenceMatchScore> CompareSequence(LoopingSequence<T> _OtherSeqence, IFuzzyComparer<T> _Comparer, out int _ThisBestStartIndex, out int _BestMatchScore, out int _BestOtherMatchIndex, int _CandidateConsiderationLength = 1, int _MinScore = -1)
        {
            List<SequenceMatchScore> Scores = new List<SequenceMatchScore>();

            Parallel.For(0, Parts.Length, (s) =>
            {
                SequencePart Subject = this.Parts[s];
                int CandidateGatherPoint = -1;

                int LowerBound = 0;
                int UpperBound = _OtherSeqence.Parts.Length - 1;

                //Do binary search for start point
                while (LowerBound <= UpperBound)
                {
                    int Midpoint = (LowerBound + UpperBound) / 2;

                    FuzzyCompareResult Result = _Comparer.CompareItems(Subject.SequenceValue, _OtherSeqence.Parts[Midpoint].SequenceValue);

                    if (Result == FuzzyCompareResult.ExactMatch || Result == FuzzyCompareResult.GreaterThanWithinMargins || Result == FuzzyCompareResult.SmallerThanWithinMargins || Result == FuzzyCompareResult.NotEqualUndefinedWithinMargins)
                    {
                        CandidateGatherPoint = Midpoint;
                        break;
                    }
                    else if (Result == FuzzyCompareResult.SmallerThan)
                    {
                        UpperBound = Midpoint - 1;
                    }
                    else
                    {
                        LowerBound = Midpoint + 1;
                    }
                }

                if (CandidateGatherPoint != -1)
                {
                    List<SequencePart> Candidates = new List<SequencePart>();

                    int InitialCorrect = CheckSequence(_Comparer, Subject, _OtherSeqence.Parts[CandidateGatherPoint], _CandidateConsiderationLength);

                    if (InitialCorrect >= _CandidateConsiderationLength)
                    {
                        Candidates.Add(_OtherSeqence.Parts[CandidateGatherPoint]);
                    }

                    bool FoundEnd = false;
                    int SearchIndex = CandidateGatherPoint + 1;

                    //Check above starting point for matches
                    while (!FoundEnd)
                    {
                        if (SearchIndex >= _OtherSeqence.Parts.Length)
                        {
                            FoundEnd = true;
                            break;
                        }

                        FuzzyCompareResult CompareResult = _Comparer.CompareItems(Subject.SequenceValue, _OtherSeqence.Parts[SearchIndex].SequenceValue);

                        if (CompareResult == FuzzyCompareResult.ExactMatch || CompareResult == FuzzyCompareResult.GreaterThanWithinMargins || CompareResult == FuzzyCompareResult.SmallerThanWithinMargins || CompareResult == FuzzyCompareResult.NotEqualUndefinedWithinMargins)
                        {
                            int CorrectCount = CheckSequence(_Comparer, Subject, _OtherSeqence.Parts[SearchIndex], _CandidateConsiderationLength);

                            if (_CandidateConsiderationLength > 0 && _CandidateConsiderationLength <= CorrectCount)
                            {
                                Candidates.Add(_OtherSeqence.Parts[SearchIndex]);
                            }
                        }
                        else
                        {
                            FoundEnd = true;
                            break;
                        }

                        SearchIndex++;
                    }

                    FoundEnd = false;
                    SearchIndex = CandidateGatherPoint - 1;

                    //Check below starting point for matches
                    while (!FoundEnd)
                    {
                        if (SearchIndex < 0)
                        {
                            FoundEnd = true;
                            break;
                        }

                        FuzzyCompareResult CompareResult = _Comparer.CompareItems(Subject.SequenceValue, _OtherSeqence.Parts[SearchIndex].SequenceValue);

                        if (CompareResult == FuzzyCompareResult.ExactMatch || CompareResult == FuzzyCompareResult.GreaterThanWithinMargins || CompareResult == FuzzyCompareResult.SmallerThanWithinMargins || CompareResult == FuzzyCompareResult.NotEqualUndefinedWithinMargins)
                        {
                            int CorrectCount = CheckSequence(_Comparer, Subject, _OtherSeqence.Parts[SearchIndex], _CandidateConsiderationLength);

                            if (_CandidateConsiderationLength > 0 && _CandidateConsiderationLength <= CorrectCount)
                            {
                                Candidates.Add(_OtherSeqence.Parts[SearchIndex]);
                            }
                        }
                        else
                        {
                            FoundEnd = true;
                            break;
                        }

                        SearchIndex--;
                    }

                    Parallel.For(0, Candidates.Count, (i) =>
                    {
                        SequencePart Target = Candidates[i];

                        int CorrectCount = CheckSequence(_Comparer, Subject, Target, -1, _CandidateConsiderationLength);


                        if ((_MinScore == -1) || (_MinScore >= 0 && CorrectCount >= _MinScore))
                        {
                            SequenceMatchScore score = new SequenceMatchScore();
                            score.ThisBestIndex = Subject.SequencePosition;
                            score.BestMatchLengthOther = CorrectCount;
                            score.BestMatchIndexOther = Candidates[i].SequencePosition;

                            lock (Scores)
                            {
                                Scores.Add(score);
                            }
                        }
                    });
                }
            });

            int Best = -1;
            int BestIndex = -1;
            int ThisBestIndex = -1;

            for (int i = 0; i < Scores.Count; ++i)
            {
                if (Scores[i].BestMatchLengthOther > Best)
                {
                    Best = Scores[i].BestMatchLengthOther;
                    BestIndex = Scores[i].BestMatchIndexOther;
                    ThisBestIndex = Scores[i].ThisBestIndex;
                }
            }

            _BestMatchScore = Best;
            _BestOtherMatchIndex = BestIndex;
            _ThisBestStartIndex = ThisBestIndex;

            return Scores;
        }

        private static int CheckSequence(IFuzzyComparer<T> _Comparer, SequencePart _Subject, SequencePart _Target, int _LimitCheck = -1, int _SkipItems = 0)
        {
            int SubjectStart = _Subject.SequencePosition;
            SequencePart NextItem = _Subject.Next;
            SequencePart NextComparisonTarget = _Target.Next;
            int CorrectCount = 1;

            for (int i = 0; CorrectCount < _SkipItems; ++i)
            {
                NextItem = NextItem.Next;
                NextComparisonTarget = NextComparisonTarget.Next;
                CorrectCount++;
            }


            while (NextItem.SequencePosition != SubjectStart)
            {
                FuzzyCompareResult CompareResult = _Comparer.CompareItems(NextItem.SequenceValue, NextComparisonTarget.SequenceValue);

                if (CompareResult == FuzzyCompareResult.ExactMatch || CompareResult == FuzzyCompareResult.GreaterThanWithinMargins || CompareResult == FuzzyCompareResult.SmallerThanWithinMargins || CompareResult == FuzzyCompareResult.NotEqualUndefinedWithinMargins)
                {
                    CorrectCount++;

                    if (_LimitCheck > 0 && CorrectCount >= _LimitCheck)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

                NextItem = NextItem.Next;
                NextComparisonTarget = NextComparisonTarget.Next;
            }

            return CorrectCount;
        }
    }
}
