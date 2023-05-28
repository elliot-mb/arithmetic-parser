using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    class Operators
    {
        public static int MAX_OP_STRENGTH = 10;

        public class Add : IOperator
        {
            public Add() { }
            public char Symbol() { return '+';  }
            public int Strength() { return 1;  }
            public double Do(double a, double b) { return a + b; }
        }

        public class Sub : IOperator
        {
            public Sub() { }
            public char Symbol() { return '-'; }
            public int Strength() { return 1; }
            public double Do(double a, double b) { return a - b; }
        }

        public class Mul : IOperator
        {
            public Mul() { }
            public char Symbol() { return '*'; }
            public int Strength() { return 2; }
            public double Do(double a, double b) { return a * b; }
        }

        public class Div : IOperator
        {
            public Div() { }
            public char Symbol() { return '/'; }
            public int Strength() { return 2; }
            public double Do(double a, double b) { return (double) a / b; }
        }

        public class Pow : IOperator
        {
            public Pow() { }
            public char Symbol() { return '^'; }
            public int Strength() { return 3; }

            public double Do(double a, double b) { return Math.Pow(a, b);  }
        }
    }
}
