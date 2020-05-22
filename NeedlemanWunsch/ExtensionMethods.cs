/**
 * Author:    Mustafa Yumurtacı
 * Created:   23.05.2020
 **/

using System;

namespace NeedlemanWunsch
{
    public static class ExtensionMethods
    {
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}