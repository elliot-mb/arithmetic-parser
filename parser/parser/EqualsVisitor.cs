using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    public class EqualsVisitor : IStatementVisitor
    {
        private readonly bool isEqual;
        private readonly AbstractStatement a;
        private readonly AbstractStatement b;
        private List<Statement> compareStmt = new List<Statement>();
        private List<Literal> compareLit = new List<Literal>();

        public EqualsVisitor(AbstractStatement a, AbstractStatement b) 
        {
            this.a = a;
            this.b = b; //those statements we must find the underlying types of and compare
        }

        public bool Equals()
        {
            a.Accept(this);
            b.Accept(this);
            if (compareStmt.Count == compareLit.Count) //if there is one in each (type inequality); rejects e.g. literals vs literals wrapped in statements
                return false;
            if(compareLit.Count == 2)
            {
                return compareLit[0].GetVal() == compareLit[1].GetVal();
            }
            //the case where they are at least both statements 
            Statement s1 = compareStmt[0];
            Statement s2 = compareStmt[1];
            //unpack for repeated use below
            List<AbstractStatement> s1Stmts = s1.GetStatements();
            List<AbstractStatement> s2Stmts = s2.GetStatements();
            List<IOperator> s1Ops = s1.GetOperators();
            List<IOperator> s2Ops = s2.GetOperators();
            //the below check ensures they have the same number of operators, too.
            //This is because statements throw an error if created with the wrong number of ops
            if (s1Stmts.Count == s2Stmts.Count && s1.GetSign() == s2.GetSign())
            {
                for(int i = 0; i < s1Ops.Count; i++)
                {
                    if (s1Ops[i].Symbol() != s2Ops[i].Symbol()) //all operators are identical
                        return false;
                }
                List<bool> eqs = new List<bool>();
                for (int i = 0; i < s1Stmts.Count; i++)
                {
                    EqualsVisitor eqV = new EqualsVisitor(s1Stmts[i], s2Stmts[i]);
                    eqs.Add(eqV.Equals());
                }
                Program.WriteLine(string.Join(", ", eqs));
                return eqs.All(x => x); //lambdas are in this language?? amazing
            }
            return false;
        }

        public void Visit(Statement stmt)
        {
            compareStmt.Add(stmt);
        }

        public void Visit(Literal lit)
        {
            compareLit.Add(lit);   
        }
    }
}
