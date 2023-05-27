using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    class Program
    {
        public static void WriteLine(string o)
        {
            Debug.WriteLine(o);
            Console.WriteLine(o);
        }

        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Program.WriteLine("Must take exactly one argument (the expression, wrapped in quotes)");
            }

            Program.WriteLine("args: " + string.Join(",", args));

            List<IOperator> operators = new List<IOperator>();
            operators.Add(new Operators.Add());
            operators.Add(new Operators.Mul());
            operators.Add(new Operators.Sub());
            operators.Add(new Operators.Div());

            Parser p = new Parser(operators);

            Program.WriteLine("out: " + p.Parse(args[0]).ToString());
        }
    }
}
