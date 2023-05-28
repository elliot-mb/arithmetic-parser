using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    abstract class Token
    {
        public abstract Token Visit(TokenVisitor tv);

        public class Literal : Token
        {

        }

        public class Operator : Token
        {

        }

        public class Bracket : Token
        {

        }
    }
}
