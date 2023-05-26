using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    //a binary operator, acting as a "rule", standard ones include '+' (plus), '-' (minus), '*' (multiply) and '/' (divide) 
    interface IOperator 
    {
        //aquire the symbol which this operator represents 
        char Symbol();
        
        //complete the operation which this operator represents 
        double Do(double a, double b);
    }
}
