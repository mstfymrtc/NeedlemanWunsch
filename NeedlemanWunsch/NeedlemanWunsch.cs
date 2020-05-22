using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NeedlemanWunsch
{
    public class NeedlemanWunsch
    {
        private int Match { get; set; }
        private int Mismatch { get; set; }
        private int Gap { get; set; }
        private string FirstSequence { get; set; } //row sequence
        private string SecondSequence { get; set; } //col sequence
        private int[,] Matrix { get; set; }

        //where int[,] is a trace, BackTraces is list of trace
        private List<List<Trace>> BackTraces { get; set; }

        /// <summary>
        /// Constructor for initializing a NeedlemanWunsch instance.
        /// Sets given values for alignment.
        /// </summary>
        /// <param name="match">Match reward</param>
        /// <param name="mismatch">Mismatch penalty</param>
        /// <param name="gap">Gap penalty</param>
        /// <param name="firstSequence">First sequence</param>
        /// <param name="secondSequence">Second sequence</param>
        public NeedlemanWunsch(int match, int mismatch, int gap, string firstSequence, string secondSequence)
        {
            Match = match;
            Mismatch = mismatch;
            Gap = gap;
            FirstSequence = firstSequence;
            SecondSequence = secondSequence;
            Matrix = new int[firstSequence.Length + 1, secondSequence.Length + 1];
            BackTraces = new List<List<Trace>>();
        }

        /// <summary>
        /// Constructor for initializing a NeedlemanWunsch instance.
        /// Sets given values for alignment. Reads sequences from seqS.txt and seqT.txt files.
        /// </summary>
        /// <param name="match">Match reward</param>
        /// <param name="mismatch">Mismatch penalty</param>
        /// <param name="gap">Gap penalty</param>
        public NeedlemanWunsch(int match, int mismatch, int gap)
        {
            Match = match;
            Mismatch = mismatch;
            Gap = gap;
//            FirstSequence = firstSequence;
//            SecondSequence = secondSequence;
//            Matrix = new int[firstSequence.Length + 1, secondSequence.Length + 1];
        }

        public void FillMatrix()
        {
            // Fill in first row and first col with gap values
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                Matrix[i, 0] = i * Gap;
            }

            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                Matrix[0, j] = j * Gap;
            }

            // Traverse matrix and calculate all values

            for (int i = 1; i < Matrix.GetLength(0); i++)
            {
                for (int j = 1; j < Matrix.GetLength(1); j++)
                {
                    int topValue = Matrix[i - 1, j] + Gap;
                    int leftValue = Matrix[i, j - 1] + Gap;
                    int diagonalValue = Matrix[i - 1, j - 1] +
                                        (FirstSequence[i - 1] == SecondSequence[j - 1] ? Match : Mismatch);
                    Matrix[i, j] = Math.Max(Math.Max(topValue, leftValue), diagonalValue);
                }
            }
        }

        public void TraceBack(List<Trace> traces)
        {
            //continue tracing until matrix[1,1]
            while (traces.Last().RowIndex != 0 && traces.Last().ColIndex != 0)
            {
                Console.WriteLine(traces.Last().RowIndex); //last trace's row
                Console.WriteLine(traces.Last().ColIndex); //last trace's col

                bool isSourceTop = false;
                bool isSourceLeft = false;
                bool isSourceDiagonal = false;

                int topValue = Matrix[traces.Last().RowIndex - 1, traces.Last().ColIndex] + Gap;
                int leftValue = Matrix[traces.Last().RowIndex, traces.Last().ColIndex - 1] + Gap;
                int diagonalValue = Matrix[traces.Last().RowIndex - 1, traces.Last().ColIndex - 1] +
                                    (FirstSequence[traces.Last().RowIndex - 1] ==
                                     SecondSequence[traces.Last().ColIndex - 1]
                                        ? Match
                                        : Mismatch);

                //set flags
                if (topValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                {
                    isSourceTop = true;
                }

                if (leftValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                {
                    isSourceLeft = true;
                }

                if (diagonalValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                {
                    isSourceDiagonal = true;
                }

                //handle all possibilities, there might be alternative traces
                if (isSourceTop && isSourceLeft && isSourceDiagonal)
                {
                    var tempTrace = new List<Trace>(traces);
                    //top condition
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                    TraceBack(tempTrace); //recursive call

                    //left condition
                    tempTrace = new List<Trace>(traces);
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                    TraceBack(tempTrace);
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceTop && isSourceLeft)
                {
                    var tempTrace = new List<Trace>(traces);
                    //top condition
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                    TraceBack(tempTrace); //left condition
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceTop && isSourceDiagonal)
                {
                    var tempTrace = new List<Trace>(traces);
                    //top condition
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                    TraceBack(tempTrace);
                    //diagonal condition
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceLeft && isSourceDiagonal)
                {
                    var tempTrace = new List<Trace>(traces);
                    //left condition
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                    TraceBack(tempTrace);
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                    //diagonal condition
                }
                else if (isSourceTop)
                {
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                }
                else if (isSourceLeft)
                {
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else
                {
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
            }

            BackTraces.Add(traces);
        }

        public void PrintMatrix()
        {
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                if (i == 0)
                {
                    Console.Write("\t\t");
                    Console.ForegroundColor = ConsoleColor.Green;

                    for (int j = 0; j < Matrix.GetLength(1); j++)
                    {
                        if (j != 0)
                        {
                            Console.Write(SecondSequence[j - 1] + "\t");
                        }
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }

                if (i != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(FirstSequence[i - 1]);
                    Console.ResetColor();
                }

                Console.Write("\t");


                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Console.Write(Matrix[i, j] + "\t");
                }

                Console.WriteLine();
            }
        }

        public void Run()
        {
            FillMatrix();
            var traces = new List<Trace>();
            traces.Add(new Trace
            {
                RowIndex = Matrix.GetLength(0) - 1,
                ColIndex = Matrix.GetLength(1) - 1
            });
            TraceBack(traces);
            PrintMatrix();
        }
    }

    public class Trace
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
    }
}