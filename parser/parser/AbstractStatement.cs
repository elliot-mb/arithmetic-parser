using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    abstract class AbstractStatement
    {
        public static readonly int POSITIVE = 1;
        public static readonly int NEGATIVE = -1;

        abstract public void Accept(IStatementVisitor visitor);

        public bool Equals(AbstractStatement a)
        {
            EqualsVisitor eqV = new EqualsVisitor(this, a);
            return eqV.Equals();
        }

        abstract public int GetSign();

        abstract public override string ToString();
    }
}
