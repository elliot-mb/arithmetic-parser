using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    interface IStatementVisitor
    {
        void Visit(Statement stmt);

        void Visit(Literal lit);
    }
}
