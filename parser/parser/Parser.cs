using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace parser
{
    class Parser
    {
        private readonly string B_OPEN = "(";
        private readonly string B_CLOSE = ")";
        private readonly string OF_LITERAL = @"([0-9]|\.)"; //characters in literals (does not check if a literal is wf)
        private readonly Regex LITERAL = new Regex(@"(([1-9][0-9]*)|0)(\.(([0-9]*[1-9]+)|0))*");
        // table for our operators
        private readonly Dictionary<char, IOperator> lookup;

        public Parser(List<IOperator> ops)
        {
            Dictionary<char, IOperator> builder = new Dictionary<char, IOperator>();
            for(int i = 0; i < ops.Count; i++)
            {
                IOperator op = ops[i];
                builder.Add(op.Symbol(), op);
            }

            lookup = builder;
        }

        private string Preprocess(string raw)
        {
            string noWhiteSpace = "";
            char[] rawChars = raw.ToCharArray();
            // remove whitespace and check all characters in our syntax
            for(int i = 0; i < rawChars.Length; i++)
            {
                char c = rawChars[i];
                Program.WriteLine("" + c);
                if (c != ' ')
                {
                    if (!lookup.ContainsKey(c) && !Regex.IsMatch("" + c, OF_LITERAL))
                    {
                        throw new Exception("There are illegal characters (besides literals and operators) in the expression.");
                    }
                    noWhiteSpace += c;
                }
            }

            return noWhiteSpace;
        }

        public double Parse(string raw)
        {
            string stmt = Preprocess(raw);

            Program.WriteLine(stmt);

            return 0.0;
        }
    }
}
