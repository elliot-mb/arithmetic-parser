using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace parser
{
    public class Parser : IStatementVisitor
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

        private readonly List<IOperator> ops;

        //visitation return value
        private double visitorEvalVal;

        public Parser(List<IOperator> ops)
        {
            this.ops = ops;
        }

        private int FindWeakOp(Statement stmt, out IOperator op) //if they are all the same we left-associate 
        {
            int weakest = int.MaxValue;
            int index = OP_NOT_FOUND;
            List<IOperator> ops = stmt.GetOperators();
            op = ops[0]; //it always gets (re)assigned in the below loop
            for (int i = 0; i < ops.Count; i++)
            {
                if (weakest >= ops[i].GetStrength())
                {
                    weakest = ops[i].GetStrength();
                    index = i;
                    op = ops[i];
                }
            }

            return index;
        }

        //split on the nth operator 
        private void SplitStmt(Statement stmt, int n, out Statement stmt1, out Statement stmt2)
        {
            if(n >= stmt.GetLength() - 1 || n < 0)
            //if n - 1 is equal to stmt.GetLength(), it means it still wants to split on an operator that shouldn't exist
            {
                throw new Exception("Cannot split '" + stmt + "' on the " + n + "th operator.");
            }
            stmt1 = stmt.Head(n);
            stmt2 = stmt.Tail(n);

            return;
        }

        public void Visit(Literal lit)
        {
            visitorEvalVal = Eval(lit);
            Program.WriteLine("visitation Literal and got " + visitorEvalVal);
        }

        public void Visit(Statement stmt)
        {
            visitorEvalVal = Eval(stmt) * stmt.GetSign();
            Program.WriteLine("visitation Statement and got " + visitorEvalVal);
        }

        private double Eval(Literal lit)
        {
            Program.WriteLine(lit + " => " + lit.GetSign() + " * " + lit.GetVal());
            return lit.GetVal() * lit.GetSign();
        }

        //evaluates statment stmt
        private double Eval(Statement stmt)
        {
            if (stmt.GetLength() == 1) //it is either a LITERAL or single substatement
            {
                AbstractStatement inner = stmt.GetStatements()[0];
                inner.Accept(this); //sets a global variable with the contents of evaluating the underlying class
                return visitorEvalVal;
            }

            IOperator op;
            int weakOp = FindWeakOp(stmt, out op);
            
            Statement stmt1, stmt2;
            SplitStmt(stmt, weakOp, out stmt1, out stmt2);

            Program.WriteLine(stmt + " => " + stmt.GetSign() + " * (" + stmt1 + " " + op.Symbol() + " " + stmt2);

            double result = op.Do(Eval(stmt1), Eval(stmt2));

            Program.WriteLine(stmt.GetSign() + " * (" + stmt1 + " " + op.Symbol() + " " + stmt2 + " = " + result);
            return result;
        }

        public double Parse(string raw)
        {
            Preprocessor p = new Preprocessor(ops);
            Statement stmt = p.Preprocess(raw);
            Program.WriteLine(stmt.ToString());

            return Eval(stmt);
        }
    }
}
