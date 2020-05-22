/**
 * Author:    Mustafa Yumurtacı
 * Created:   23.05.2020
 **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeedlemanWunsch
{
    public class NeedlemanWunsch
    {
        private int Match { get; set; }
        private int Mismatch { get; set; }

        private int Gap { get; set; }

        // Row sequence
        private string FirstSequence { get; set; }

        // Column sequence
        private string SecondSequence { get; set; }
        private int[,] Matrix { get; set; }

        // All possible back traces
        private List<List<Trace>> BackTraces { get; set; }
        private List<AlignedSequencePair> AlignedSequencePairs { get; set; }

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
            AlignedSequencePairs = new List<AlignedSequencePair>();
        }

        /// <summary>
        /// Constructor for initializing a NeedlemanWunsch instance.
        /// Sets given values for alignment. Reads row sequence from seqS.txt, and col sequence from seqT.txt file.
        /// </summary>
        /// <param name="match">Match reward</param>
        /// <param name="mismatch">Mismatch penalty</param>
        /// <param name="gap">Gap penalty</param>
        public NeedlemanWunsch(int match, int mismatch, int gap)
        {
            Match = match;
            Mismatch = mismatch;
            Gap = gap;
            BackTraces = new List<List<Trace>>();
            AlignedSequencePairs = new List<AlignedSequencePair>();
            InitializeSequencesFromFile();
        }

        /// <summary>
        /// Read sequences first sequence from seqS.txt, second sequence from seqT.txt file and set them to instance along with Matrix.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        public void InitializeSequencesFromFile()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = appDirectory.Substring(0, appDirectory.IndexOf("/bin"));
            if (File.Exists(fullPath + "/seqS.txt") && File.Exists(fullPath + "/seqT.txt"))
            {
                FirstSequence = File.ReadAllLines(fullPath + "/seqS.txt")[1];
                SecondSequence = File.ReadAllLines(fullPath + "/seqT.txt")[1];
                Matrix = new int[FirstSequence.Length + 1, SecondSequence.Length + 1];
            }
            else
            {
                throw new FileNotFoundException("Both seqS.txt and seqT.txt files must be exists!");
            }
        }

        /// <summary>
        /// Fill in solution matrix based on match, mismatch and gap values
        /// </summary>
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

        /// <summary>
        /// Recursively trace back
        /// </summary>
        /// <param name="traces"></param>
        public void TraceBack(List<Trace> traces)
        {
            // Continue tracing until Matrix[1,1], Matrix[1,0] or Matrix[0,1]
            while (!((traces.Last().RowIndex == 1 && traces.Last().ColIndex == 1) ||
                     (traces.Last().RowIndex == 1 && traces.Last().ColIndex == 0) ||
                     (traces.Last().RowIndex == 0 && traces.Last().ColIndex == 1)))
            {
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

                // Set flags
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

                // Handle all possibilities, there might be alternative traces
                // If there such trace exists, handle it as different traceback recursively.
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
                    TraceBack(tempTrace);

                    //left condition
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

                    //diagonal condition
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
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

            traces.Add(new Trace {RowIndex = 0, ColIndex = 0});
            BackTraces.Add(traces);
        }


        /// <summary>
        /// Based on back traces, find all possible sequences.
        /// </summary>
        public void AlignSequences()
        {
            for (int i = 0; i < BackTraces.Count; i++)
            {
                string firstAlignedSequence = "";
                string secondAlignedSequence = "";
                for (int j = 0; j < BackTraces[i].Count - 1; j++)
                {
                    if (BackTraces[i][j].RowIndex - 1 == BackTraces[i][j + 1].RowIndex &&
                        BackTraces[i][j].ColIndex - 1 == BackTraces[i][j + 1].ColIndex)
                    {
                        firstAlignedSequence += FirstSequence[BackTraces[i][j].RowIndex - 1];
                        secondAlignedSequence += SecondSequence[BackTraces[i][j].ColIndex - 1];
                    }
                    else if (BackTraces[i][j].RowIndex - 1 == BackTraces[i][j + 1].RowIndex &&
                             BackTraces[i][j].ColIndex == BackTraces[i][j + 1].ColIndex)
                    {
                        firstAlignedSequence += FirstSequence[BackTraces[i][j].RowIndex - 1];
                        secondAlignedSequence += "-";
                    }
                    else
                    {
                        firstAlignedSequence += "-";
                        secondAlignedSequence += SecondSequence[BackTraces[i][j].ColIndex - 1];
                    }
                }

                AlignedSequencePairs.Add(new AlignedSequencePair
                {
                    FirstAlignedSequence = firstAlignedSequence.Reverse(),
                    SecondAlignedSequence = secondAlignedSequence.Reverse()
                });
            }
        }

        /// <summary>
        /// Print sequence alignment results along with other info obtained.
        /// </summary>
        public void PrintResults()
        {
            // Print solution matrix

            Console.WriteLine("Solution matrix:");

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

            // Print score
            Console.WriteLine("\nObtained score: " + Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1]);

            // Print back traces info
            Console.WriteLine("Number of backtraces: " + BackTraces.Count);

            // Print all possible alignments
            Console.WriteLine("\nAll possible alignments:");

            foreach (var pair in AlignedSequencePairs)
            {
                Console.WriteLine("----------");
                Console.WriteLine(pair.FirstAlignedSequence);
                Console.WriteLine(pair.SecondAlignedSequence);
            }

            Console.WriteLine("----------");
        }

        /// <summary>
        /// Run alignment process based on sequence info.
        /// </summary>
        public void Run()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            FillMatrix();
            var traces = new List<Trace>();
            traces.Add(new Trace
            {
                RowIndex = Matrix.GetLength(0) - 1,
                ColIndex = Matrix.GetLength(1) - 1
            });
            TraceBack(traces);
            AlignSequences();
            watch.Stop();

            PrintResults();
            Console.WriteLine("\nExecution time: " + watch.ElapsedMilliseconds + " milliseconds\n");
        }
    }


    public class AlignedSequencePair
    {
        public string FirstAlignedSequence { get; set; }
        public string SecondAlignedSequence { get; set; }
    }

    public class Trace
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
    }
}