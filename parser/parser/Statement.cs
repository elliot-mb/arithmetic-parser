using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    class Statement : AbstractStatement
    {
        private readonly List<AbstractStatement> stmts;
        private readonly List<IOperator> ops;
        private readonly int sign;

        public Statement(List<AbstractStatement> stmts, List<IOperator> ops, int sign)
        {
            this.stmts = stmts;
            this.ops = ops;
            this.sign = sign;
            //is a statement not well formed? this can be the case where |ops| >= |stmts| (too many binary operations for given quanity of stmts)
            if (ops.Count != stmts.Count - 1)
                throw new Exception("Tried producing statement '" + this.ToString() + "' which has the wrong number of operators, given it's sub-statements.");
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
