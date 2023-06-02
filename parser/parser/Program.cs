﻿using System;
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
            if(args.Length != 1 && args.Length != 2)
            {
                throw new Exception("Must take at least one argument (the expression, wrapped in quotes), and an optional debug flag '-d'");
            }

            Program.WriteLine("args: " + string.Join(",", args));

            List<IOperator> operators = new List<IOperator>();
            operators.Add(new Operators.Add());
            operators.Add(new Operators.Mul());
            operators.Add(new Operators.Sub());
            operators.Add(new Operators.Div());
            operators.Add(new Operators.Pow());

            Parser p = new Parser(operators);

            Program.WriteLine("out: " + p.Parse(args[0]).ToString());
        }
    }
}
