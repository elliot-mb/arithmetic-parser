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
        private readonly char B_OPEN = '(';
        private readonly char B_CLOSE = ')';
        private readonly string OF_LITERAL = @"([0-9]|\.)"; //characters in literals (does not check if a literal is wf)
        private readonly Regex LITERAL = new Regex(@"(([1-9][0-9]*)|0)(\.(([0-9]*[1-9]+)|0))*");
        private readonly Regex LITERAL_PART = new Regex(@"[0-9]|\."); //for a single char of a literal
        private readonly int OP_NOT_FOUND = -1;
        private enum Segment{
            None,
            Literal,
            Operator,
            Bracket
        };

        // table for our operators
        private readonly Dictionary<char, IOperator> opLookup;

        public Parser(List<IOperator> ops)
        {
            Dictionary<char, IOperator> builder = new Dictionary<char, IOperator>();
            for(int i = 0; i < ops.Count; i++)
            {
                IOperator op = ops[i];
                builder.Add(op.Symbol(), op);
            }

            opLookup = builder;
        }

        private bool IsValidChar(char c)
        {
            return opLookup.ContainsKey(c) || Regex.IsMatch("" + c, OF_LITERAL) || c == B_OPEN || c == B_CLOSE;
        }

        //removes whitespace, checks characters are all valid, and adds brackets to represent operator binding strength (TODO)
        private string Preprocess(string raw)
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
                    if (!IsValidChar(c))
                    {
                        throw new Exception("Expression appears to contain illegal character(s) including '"+c+"' in the expression '" + raw + "'.");
                    }
                    noWhiteSpace += c;
                }
            }

            if(bracketDepth != 0)
            {
                throw new Exception("Expression appears to contain orphaned brackets, please ensure your expression is well-formed.");
            }

            //split the input into in-order literals and operator streaks for more granular processing 
            List<string> destructured = new List<string>();
            string currentSegment = "";
            Segment currentID = Segment.None;
            for(int i = 0; i < noWhiteSpace.Length; i++)
            {
                char c = noWhiteSpace[i];
                if(LITERAL_PART.IsMatch("" + c)) //is part of a literal
                {
                    if (currentID != Segment.Literal)
                    {
                        destructured.Add(currentSegment);
                        currentID = Segment.Literal;
                        currentSegment = "";
                    }
                    currentSegment += c;
                }
                else if (c == B_OPEN || c == B_CLOSE) // each and every bracket is its own segment 
                { 
                    destructured.Add(currentSegment);
                    currentID = Segment.Bracket;
                    currentSegment = "";
                    currentSegment += c;
                }
                else //if current character is an operator (or sequence of in the case of negatives etc.)
                {
                    if (currentID != Segment.Operator)
                    {
                        destructured.Add(currentSegment);
                        currentID = Segment.Operator; 
                        currentSegment = "";
                    }
                    currentSegment += c;
                }
            }

            if(currentSegment.Length != 0)
                destructured.Add(currentSegment);
            destructured.RemoveAt(0);
            
            Program.WriteLine("preprocessed: " + noWhiteSpace);
            Program.WriteLine("segmented   : " + string.Join(", ", destructured));

            return noWhiteSpace;
        }

        private int FindWeakOp(string stmt) //if they are all the same we left-associate 
        {
            int result = OP_NOT_FOUND;
            int bracketDepth = 0;
            int weakest = int.MaxValue;
            for(int i = 0; i < stmt.Length; i++)
            {
                char c = stmt[i];
                if (c == B_OPEN)
                    bracketDepth++;
                if (c == B_CLOSE)
                    bracketDepth--;

                if (opLookup.ContainsKey(c))
                {
                    IOperator op = opLookup[c];
                    //multiplies binding strength of brackets so they remain together until processed individually 
                    int s = op.Strength() + (bracketDepth * Operators.MAX_OP_STRENGTH); 
                    if (s <= weakest)
                    {
                        weakest = s;
                        result = i;
                    }
                }
            }

            return result;
        }

        private void SplitStmt(string stmt, int n, out string stmt1, out string stmt2)
        {
            if(n > stmt.Length || n < 0)
            {
                throw new Exception("Cannot split '" + stmt + "' on index '" + n + "'.");
            }
            stmt1 = stmt.Remove(n, stmt.Length - n);
            stmt2 = stmt.Substring(n + 1);

            return;
        }

        //evaluates statment stmt
        private double Eval(string stmt)
        {
            if (stmt.Length == 0) throw new Exception("Cannot evaluate empty statements");

            int weakOp = FindWeakOp(stmt);
            if (weakOp == OP_NOT_FOUND)
            {
                string literal = LITERAL.Match(stmt).Value;
                //Program.WriteLine("LITERAL" + literal);
                double val;
                bool parseSucceeds = Double.TryParse(literal, out val); //pass-by-ref
                if (!parseSucceeds) throw new Exception("Cannot evaluate double literal '" + stmt + "' or '" + literal + "', it may be incorrectly formatted.");
                return val;
            }
            
            string stmt1, stmt2;
            SplitStmt(stmt, weakOp, out stmt1, out stmt2);
            IOperator op = opLookup[stmt[weakOp]]; //what we apply to the results of the two statements 
            Program.WriteLine(stmt);
            Program.WriteLine("^".PadLeft(weakOp + 1));

            return op.Do(Eval(stmt1), Eval(stmt2));

        }

        public double Parse(string raw)
        {
            Preprocessor p = new Preprocessor(opLookup);
            Program.WriteLine(p.ToStatement(p.Clean(raw, opLookup)).ToString());

            Program.WriteLine("//////////////////////////");

            string stmt = Preprocess(raw);

            return Eval(stmt);
        }
    }
}
