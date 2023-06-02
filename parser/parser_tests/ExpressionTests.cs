using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using parser;
using System.Collections.Generic;

namespace parser_tests
{
    [TestClass]
    public class ExpressionTests
    {
        private readonly Parser PARSER = new Parser(new List<IOperator>()
        {
            new Operators.Add(),
            new Operators.Mul(),
            new Operators.Sub(),
            new Operators.Div(),
            new Operators.Pow()
        }, false); //debug mode off

        //floating point error
        private readonly double FPE = 0.01;


        //allows a specified error to account for floating point calculation errors 
        private bool TollerantEquals(double test, double real)
        {
            return test > real - FPE && test < real + FPE;
        }

        [TestMethod]
        public void Parse_NoBracketsLeftAssociative1_ResultEquals()
        {
            string expression = "1+2+3";
            double parserResult = PARSER.Parse(expression);
            double trueResult = 6.0;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }

        [TestMethod]
        public void Parse_NoBracketsLeftAssociative2_ResultEquals()
        {
            string expression = "1-2-3";
            double parserResult = PARSER.Parse(expression);
            double trueResult = -4.0;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }

        [TestMethod]
        public void Parse_NoBracketsLeftAssociative3_ResultEquals()
        {
            string expression = "-1+-2-3";
            double parserResult = PARSER.Parse(expression);
            double trueResult = -6.0;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }

        [TestMethod]
        public void Parse_NoBracketsLeftAssociative4_ResultEquals()
        {
            string expression = "1+2-3+4-5+6-7+8-9";
            double parserResult = PARSER.Parse(expression);
            double trueResult = -3.0;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }

        [TestMethod]
        public void Parse_NestedStatements_ResultEquals()
        {
            string expression = "((1-2)-2-(1))";
            double parserResult = PARSER.Parse(expression);
            double trueResult = -4.0;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }

        [TestMethod]
        //I am using swizzled here to mean combinations like "--" and "+-" and "*-" etc., i.e. just combinations of operators that are legal
        public void Parse_NestedVariedBindStrengthsExpressionSwizzledOperators1_ResultEquals()
        {
            string expression = "--1*((-9/2)^3-(2+3)^2)";
            double parserResult = PARSER.Parse(expression);
            double trueResult = -116.125;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }

        [TestMethod]
        public void Parse_NestedVariedBindStrengthsExpression1_ResultEquals()
        {
            string expression = "((4+2/4+1+6.54)^3+(3*2-89/88)^(-1/2))+(2-3^3-2)";
            double parserResult = PARSER.Parse(expression);
            double trueResult = 1718.785387;

            Assert.IsTrue(TollerantEquals(parserResult, trueResult));
        }
    }
}
