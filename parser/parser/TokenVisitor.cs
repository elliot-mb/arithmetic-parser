using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    interface TokenVisitor
    {
        void Accept(Token t);
    }
}
