/**
 * Author:    Mustafa Yumurtacı
 * Created:   23.05.2020
 **/

namespace NeedlemanWunsch
{
    class Program
    {
        static void Main(string[] args)
        {
            // Call from param
            //NeedlemanWunsch aligner = new NeedlemanWunsch(10, -2, -5, "ACGCTG", "CATGT");
            //aligner.Run();

            // Call from file
            NeedlemanWunsch aligner = new NeedlemanWunsch(5, -3, -5);
            aligner.Run();
        }
    }
}