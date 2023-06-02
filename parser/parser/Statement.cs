using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    public class Statement : AbstractStatement
    {
        private readonly List<AbstractStatement> stmts;
        private readonly List<IOperator> ops;
        private readonly int sign;

        public Statement(List<AbstractStatement> stmts, List<IOperator> ops, int sign)
        {
            //is a statement not well formed? this can be the case where |ops| >= |stmts| (too many binary operations for given quanity of stmts)
            if (ops.Count != stmts.Count - 1)
                throw new Exception("Tried producing a statement which has the wrong number of operators (" + ops.Count + "), given the number of sub-statements " + stmts.Count + ").");

            this.stmts = stmts;
            this.ops = ops;
            this.sign = sign;
        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override int GetSign()
        {
            return sign;
        }

        public List<IOperator> GetOperators()
        {
            return ops;
        }

        public List<AbstractStatement> GetStatements()
        {
            return stmts;
        }

        //how many statements does this statement contain
        public int GetLength()
        {
            return stmts.Count;
        }

        //checks if n indexes our operators properly
        private bool OpInRange(int n)
        {
            if (n >= stmts.Count - 1 || n < 0)
                throw new IndexOutOfRangeException("There is no " + n + "th operator in " + ToString());
            return true;
        }

        //gets the substatement which appears before the nth operator
        public Statement Head(int n)
        {
            OpInRange(n);
            return new Statement(stmts.GetRange(0, n + 1), ops.GetRange(0, n), Literal.POSITIVE);
        }

        //gets the substatement which appears after the nth operator
        public Statement Tail(int n)
        {
            OpInRange(n);
            return new Statement(stmts.GetRange(n + 1, stmts.Count - n - 1), ops.GetRange(n + 1, ops.Count - n - 1), Literal.POSITIVE);
        }

        override public string ToString()
        {
            string result = "";
            if (sign == AbstractStatement.NEGATIVE)
                result += "-"; //indicates the statement, once evalualted, will be inverted/complemented
            result += "Statement(";

            int i;
            for(i = 0; i < ops.Count; i++)
            {
                result += stmts[i].ToString();
                result += ops[i].Symbol();
            }
            if(i < stmts.Count)
                result += stmts[i].ToString();

            return result + ")";
        }
    }
}
