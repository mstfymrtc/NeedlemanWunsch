using System;

namespace NeedlemanWunsch
{
    class Program
    {
        static void Main(string[] args)
        {
            NeedlemanWunsch aligner = new NeedlemanWunsch(10, -2, -5, "CATTCAC", "CTCGCAGC");
            aligner.Run();
        }
    }
}