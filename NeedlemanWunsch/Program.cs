using System;

namespace NeedlemanWunsch
{
    class Program
    {
        static void Main(string[] args)
        {
            NeedlemanWunsch aligner = new NeedlemanWunsch(2, -1, -1, "ACGCTG", "CATGT");
            aligner.Run();
        }
    }
}