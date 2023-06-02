using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    //a binary operator, standard ones include '+' (plus), '-' (minus), '*' (multiply) and '/' (divide) 
    public interface IOperator 
    {
        //aquire the symbol which this operator represents 
        char Symbol();

        //closure presenting their relative binding strength (integer)
        int GetStrength();

        //complete the operation which this operator represents 
        double Do(double a, double b);
    }
}
