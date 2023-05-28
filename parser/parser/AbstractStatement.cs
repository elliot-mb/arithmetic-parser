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

        void Accept(IStatementVisitor visitor)
        {
            visitor.Visit(this);
            return;
        }
        abstract public int GetSign();

        abstract public override string ToString();
    }
}
