using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using parser;
using System.Collections.Generic;

namespace parser_tests
{
    [TestClass]
    public class PreprocessorTests
    {

        private readonly string SMALL_STMT_INPUT = "2+2";
        private readonly Statement SMALL_STMT = new Statement(new List<AbstractStatement>()
        {
            new Literal(2, Literal.POSITIVE),
            new Literal(2, Literal.POSITIVE)
        }, 
            new List<IOperator>() { new Operators.Add() }, Literal.POSITIVE);

        private readonly List<IOperator> ALL_OPS = new List<IOperator>() 
        { 
            new Operators.Add(),
            new Operators.Sub(),
            new Operators.Mul(),
            new Operators.Div(),
            new Operators.Pow()
        };

        [TestMethod]
        public void Preprocess_SmallString_EqualsStmt()
        {
            Preprocessor p = new Preprocessor(new List<IOperator> { new Operators.Add() });
            Statement s = p.Preprocess(SMALL_STMT_INPUT);
            Assert.IsTrue(SMALL_STMT.Equals(s));
        }
    }
}
