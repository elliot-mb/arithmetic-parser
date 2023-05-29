using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    public class Literal : AbstractStatement
    {
        private readonly double val;
        private readonly int sign;

        public Literal(double val, int sign)
        {
            this.val = val;
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

        public double GetVal()
        {
            return val;
        }

        public override string ToString()
        {
            return "Literal(" + (sign * val).ToString() + ")";
        }
    }
}
