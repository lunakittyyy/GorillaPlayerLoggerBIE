using System;
using System.Collections.Generic;
using System.Text;

namespace GorillaPlayerLoggerBIE.Utils
{
    public class StringTool
    {
        public static string NormalizeName(string text)
        {
            text = new string(Array.FindAll(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
            if (text.Length > 12)
            {
                text = text.Substring(0, 10);
            }
            text = text.ToUpper();
            return text;
        }
    }
}
