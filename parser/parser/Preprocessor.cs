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
        private enum Token
        {
            None,
            Literal,
            Operator,
            Bracket
        };
        private readonly string TOKEN_T = "NLOB";
        private readonly char TOKEN_T_DELIM = ':';

        // table for our operators mapping to functions 
        private readonly Dictionary<char, IOperator> opLookup;

        public Preprocessor(Dictionary<char, IOperator> opLookup)
        {
            this.opLookup = opLookup;
        }


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

        private List<string> Destructure(string cleaned)
        {
            //split the input into in-order literals and operator streaks for more granular processing 
            List<string> destructured = new List<string>();
            string currentToken = "";
            Token currentID = Token.None;
            for (int i = 0; i < cleaned.Length; i++)
            {
                char c = cleaned[i];
                bool delta = false;
                
                if (LITERAL_PART.IsMatch("" + c)) //is part of a literal
                {
                    if (currentID != Token.Literal)
                    {
                        destructured.Add(currentToken);
                        currentID = Token.Literal;
                        delta = true;
                    }
                }
                else if (c == B_OPEN || c == B_CLOSE) // each and every bracket is its own token
                {
                    destructured.Add(currentToken);
                    currentID = Token.Bracket;
                    delta = true;
                }
                else //if current character is an operator (or sequence of in the case of negatives etc.)
                {
                    if (currentID != Token.Operator)
                    {
                        destructured.Add(currentToken);
                        currentID = Token.Operator;
                        delta = true;
                    }
                }

                if (delta)
                    currentToken = "" + TOKEN_T[(int)currentID] + TOKEN_T_DELIM;

                currentToken += c;
            }

            if (currentToken.Length != 0)
                destructured.Add(currentToken);
            destructured.RemoveAt(0);

            return destructured;
        }

        private bool IsAdd(char c)
        {
            return c == new Operators.Add().Symbol();
        }

        private bool IsSub(char c)
        {
            return c == new Operators.Sub().Symbol();
        }

        //may return statement or literal
        //returns the statement and its substatements through recurison 
        //returns-by-ref whats left to be processed so it does not repeat itself
        private Statement Consume(List<string> tokens, int inPtr, out int outPtr)
        {
            outPtr = inPtr;
            string tkn = tokens[outPtr];

            //state info
            char lastType = TOKEN_T[(int)Token.None];
            int sign = AbstractStatement.POSITIVE;

            List<AbstractStatement> subStmts = new List<AbstractStatement>();
            List<IOperator> subOps = new List<IOperator>();

            while(outPtr < tokens.Count) //for all of the tokens (except those processed by sub calls; they create statements which represent nested/brackets)
            {
                char type = tkn[0];
                string content = tkn.Split(TOKEN_T_DELIM)[1];

                if(type == TOKEN_T[(int)Token.Literal])
                {
                    double val;
                    bool parseSucceeds = Double.TryParse(content, out val);
                    if (!parseSucceeds) throw new Exception("Cannot evaluate double literal '" + content + "', it may be incorrectly formatted.");

                    subStmts.Add(new Literal(val, sign));

                    sign = AbstractStatement.POSITIVE; //if we were set negative before we now reset, as we have consumed the prior minus 
                }
                if(type == TOKEN_T[(int)Token.Operator])
                {
                    char op = content[0];

                    if (content.Length > 2 || (content.Length == 2 && content[1] != new Operators.Sub().Symbol()))
                    {
                        throw new Exception("Nonsensical operator combination discovered, expression may not be well-formed.");
                    }
                    else
                    {
                        if (lastType == TOKEN_T[(int)Token.Literal] || lastType == TOKEN_T[(int)Token.Bracket])
                        {
                            subOps.Add(opLookup[op]);
                            //if there is a second operator, since we already caught when its not, it is a minus, so we just check it exists 
                            if (content.Length == 2)
                                sign = AbstractStatement.NEGATIVE;
                        }
                        //ELSE IF it appears before Nothing (case that there is a leading negative) 
                        else if (IsSub(op) && content.Length != 2) 
                        {
                            //a plus in this context does nothing, so we only act on a negative
                            sign = AbstractStatement.NEGATIVE;
                        }
                    }
                }
                if(type == TOKEN_T[(int)Token.Bracket])
                {
                    if (content == "" + B_OPEN)
                    {//outPtr is incremented so we move PAST the bracket (ignore it, only using it to distinguish sub-statements)

                        Statement subStmt = Consume(tokens, outPtr + 1, out outPtr); //save it so we can copy it and set the sign

                        subStmts.Add(new Statement(subStmt.GetStatements(), subStmt.GetOperators(), sign));
                        sign = AbstractStatement.POSITIVE;
                    }
                    else //if its closed 
                    {
                        return new Statement(subStmts, subOps, 1);
                    }
                }
                outPtr++;
                if(outPtr < tokens.Count)
                    tkn = tokens[outPtr];
                lastType = type;
            }
            return new Statement(subStmts, subOps, AbstractStatement.POSITIVE);
        }

        public Statement ToStatement(string cleaned)
        {
            List<string> tokens = Destructure(cleaned);

            return Consume(tokens, 0, out int _);
        }

        public Statement Preprocess(string raw)
        {
            return ToStatement(Clean(raw, opLookup));
        }
    }
}
