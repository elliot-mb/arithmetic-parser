using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace parser
{
    class Preprocessor
    {
        private readonly char B_OPEN = '(';
        private readonly char B_CLOSE = ')';
        private readonly Regex LITERAL_PART = new Regex(@"[0-9]|\."); //for a single char of a literal

        public Preprocessor() { }

        private bool IsValidChar(char c, Dictionary<char, IOperator> opLookup)
        {
            return opLookup.ContainsKey(c) || LITERAL_PART.IsMatch("" + c) || c == B_OPEN || c == B_CLOSE;
        }

        public string Clean(string raw, Dictionary<char, IOperator> opLookup)
        {
            string noWhiteSpace = "";
            int bracketDepth = 0;
            // remove whitespace and check all characters in our syntax
            for (int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];
                if (c == B_OPEN)
                    bracketDepth++;
                if (c == B_CLOSE)
                    bracketDepth--;

                if (c != ' ')
                {
                    if (!IsValidChar(c, opLookup))
                    {
                        throw new Exception("Expression appears to contain illegal character(s) including '" + c + "' in the expression '" + raw + "'.");
                    }
                    noWhiteSpace += c;
                }
            }

            if (bracketDepth != 0)
            {
                throw new Exception("Expression appears to contain orphaned brackets, please ensure your expression is well-formed.");
            }

            return noWhiteSpace;
        }

        
    }
}
