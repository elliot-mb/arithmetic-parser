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
        private bool debug;

        public Parser(List<IOperator> ops, bool debug)
        {
            this.ops = ops;
            this.debug = debug;
        }

        private string PrettyCalculation(double left, double right, IOperator op, double result, int sign)
        {
            string builder = "";
            if (sign == -1)
                builder += "-(";
            builder += "" + left + " " + op.Symbol() + " " + right;
            if (sign == -1)
                builder += ")";
            builder += " = " + (result * sign);
            return builder;
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
        }

        public void Visit(Statement stmt)
        {
            visitorEvalVal = Eval(stmt) * stmt.GetSign();
        }

        private double Eval(Literal lit)
        {
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

            double left = Eval(stmt1);
            double right = Eval(stmt2); //save these for use in debug/printing
            double result = op.Do(left, right);

            if(debug)
                Program.WriteLine(PrettyCalculation(left, right, op, result, stmt.GetSign()));

            return result;
        }

        public double Parse(string raw)
        {
            Preprocessor p = new Preprocessor(ops);
            Statement stmt = p.Preprocess(raw);

            if(debug)
                Program.WriteLine("preprocess: " + stmt.ToString());

            return Eval(stmt);
        }
    }
}
