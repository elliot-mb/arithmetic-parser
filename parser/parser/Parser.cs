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
        private readonly int OP_NOT_FOUND = -1;
        private readonly int SUB_STMT_NOT_FOUND = -1;
        private readonly int B_WRAPPED = 2; //the amount of extra characters in '(stmt)' versus 'stmt'

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

            /*
            string bracketed = raw;

            //for adding binding strength brackets
            int bracketTally = 0; //<=> we need to close brackets before the end of the expression 
            int bracketDepth = 0; //the CURRENT depth of nesting in the original statement (contributes directly to binding strength)
            int lastOp = OP_NOT_FOUND;
            int lastStrength = 0;
            int inserts = 0;
            for (int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];
                if (c == B_OPEN)
                    bracketDepth++;
                if (c == B_CLOSE)
                    bracketDepth--;

                if (opLookup.ContainsKey(c))
                {
                    IOperator op = opLookup[c];
                    int currentStrength = op.Strength() + (bracketDepth * Operators.MAX_OP_STRENGTH);

                    if (lastOp != OP_NOT_FOUND)
                    {
                        IOperator prevOp = opLookup[raw[lastOp]];
 
                        if (currentStrength > lastStrength)
                        {
                            bracketed = bracketed.Insert(lastOp + 1 + inserts, "" + B_OPEN);
                            bracketTally++;
                            inserts++;
                        }
                        else if(currentStrength < lastStrength)
                        {
                            bracketed = bracketed.Insert(i + inserts, "" + B_CLOSE);
                            bracketTally--;
                            inserts++;
                        }
                    }

                    lastOp = i;
                    lastStrength = currentStrength;
                    //Program.WriteLine("depth:" + bracketDepth + ";" +lastOp.ToString() + ";" + bracketed);
                }
            }
            //to close of all remaining open-brackets
            for(int i = 0; i < bracketTally; i++)
                bracketed += B_CLOSE;

            //to open any excess closing brackets 
            for (int i = 0; i < -bracketTally; i++)
                bracketed = B_OPEN + bracketed;

            bracketed = B_OPEN + bracketed + B_CLOSE;
            */
            // remove whitespace and check all characters in our syntax
            for(int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];
                if (c != ' ')
                {
                    if (!IsValidChar(c))
                    {
                        throw new Exception("There are illegal character(s) including '"+c+"' in the expression '" + raw + "'.");
                    }
                    noWhiteSpace += c;
                }
            }

            Program.WriteLine("preprocessed: " + noWhiteSpace);

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

                //Program.WriteLine(j + "" + stmt);
                if (opLookup.ContainsKey(c))
                {
                    IOperator op = opLookup[c];
                    int s = op.Strength() + (bracketDepth * Operators.MAX_OP_STRENGTH);
                    if (s < weakest)
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
            Program.WriteLine("stmt: "+stmt);
            if (stmt.Length == 0) throw new Exception("Cannot evaluate empty statements");

            int weakOp = FindWeakOp(stmt);
            if (weakOp == OP_NOT_FOUND)
            {
                string literal = LITERAL.Match(stmt).Value;
                //Program.WriteLine("LITERAL" + literal);
                double val;
                bool parseSucceeds = Double.TryParse(literal, out val); //pass-by-ref
                if (!parseSucceeds) throw new Exception("Could not evaluate double literal '" + stmt + "' or '" + literal + "', it may be incorrectly formatted.");
                return val;
            }
            
            string stmt1, stmt2;
            SplitStmt(stmt, weakOp, out stmt1, out stmt2);
            IOperator op = opLookup[stmt[weakOp]]; //what we apply to the results of the two statements 
            Program.WriteLine("split: " + stmt1 + "; " + stmt2);

            //left-associate statements (there are no brackets)
            return op.Do(Eval(stmt2), Eval(stmt1));

        }

        private string ReverseStmt(string stmt)
        {
            string r = ""; //plain reversed statement 
            for(int i = stmt.Length - 1; i >= 0; i--)
            {
                if (stmt[i] == B_OPEN) r += B_CLOSE;
                if (stmt[i] == B_CLOSE) r += B_OPEN;
                if(stmt[i] != B_OPEN && stmt[i] != B_CLOSE) r += stmt[i];
            }

            char[] rr = r.ToCharArray(); //statement with literals re-reversed 
            MatchCollection literals = LITERAL.Matches(r);

            //Program.WriteLine(r);

            for(int i = 0; i < literals.Count; i++)
            {
                Match l = literals[i];

                //Program.WriteLine("MATCH" + l.Value);
                int index = l.Index;
                int len = l.Length;
                for (int j = 0; j < len; j++)
                {
                    rr[j + index] = r[len - 1 - j + index];
                }
            }

            return new string(rr);
        }

        public double Parse(string raw)
        {
            string stmt = Preprocess(raw);
            string stmtReversed = ReverseStmt(stmt);  //promotes left associativity in this way of processing it 
            //Program.WriteLine(stmtReversed);

            return Eval(stmtReversed);
        }
    }
}
