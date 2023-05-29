using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using parser;
using System.Collections.Generic;

namespace parser_tests
{
    [TestClass]
    public class StatementEqualsTests
    {
        private readonly List<IOperator> EMPTY = new List<IOperator>();
        private readonly int TEST_LITERAL_1 = 1;
        private readonly int TEST_LITERAL_2 = 4;

        [TestMethod]
        public void Equals_SameLiteral_AreEqual()
        {
            Statement s1 = new Statement(new List<AbstractStatement>() { new Literal(TEST_LITERAL_1, 1) }, EMPTY, Literal.POSITIVE);
            Statement s2 = new Statement(new List<AbstractStatement>() { new Literal(TEST_LITERAL_1, 1) }, EMPTY, Literal.POSITIVE);

            Assert.IsTrue(s1.Equals(s2));
        }

        [TestMethod]
        public void Equals_DifferentLiteral_NotEqual()
        {
            Statement s1 = new Statement(new List<AbstractStatement>() { new Literal(TEST_LITERAL_1, 1) }, EMPTY, Literal.POSITIVE);
            Statement s2 = new Statement(new List<AbstractStatement>() { new Literal(TEST_LITERAL_2, 1) }, EMPTY, Literal.POSITIVE);

            Assert.IsFalse(s1.Equals(s2));
        }

        [TestMethod]
        public void Equals_LiteralOperatorLiteral_AreEqual()
        {
            Statement s1 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.POSITIVE),
                new Literal(TEST_LITERAL_1, Literal.NEGATIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.POSITIVE);
            Statement s2 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.POSITIVE),
                new Literal(TEST_LITERAL_1, Literal.NEGATIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.POSITIVE);

            Assert.IsTrue(s1.Equals(s2));
        }

        [TestMethod]
        public void Equals_LiteralOperatorDifferentLiteral_NotEqual()
        {
            Statement s1 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.POSITIVE),
                new Literal(TEST_LITERAL_2, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.POSITIVE);
            Statement s2 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.POSITIVE),
                new Literal(TEST_LITERAL_1, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.POSITIVE);

            Assert.IsFalse(s1.Equals(s2));
        }

        [TestMethod]
        public void Equals_NestedLiteralOperatorLiteral_AreEqual()
        {
            Statement ss1 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.POSITIVE),
                new Literal(TEST_LITERAL_1, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.NEGATIVE);
            Statement ss2 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.NEGATIVE),
                new Literal(TEST_LITERAL_1, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.POSITIVE);

            Statement s1 = new Statement(
                new List<AbstractStatement>() { ss1, ss2 }, 
                new List<IOperator>() { new Operators.Sub() },
                Literal.NEGATIVE);

            Statement s2 = new Statement(
                new List<AbstractStatement>() { ss1, ss2 },
                new List<IOperator>() { new Operators.Sub() },
                Literal.NEGATIVE);

            Assert.IsTrue(s1.Equals(s2));
        }

        [TestMethod]
        public void Equals_NestedDiffLiteralOperatorLiteral_NotEqual()
        {
            Statement ss1 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.POSITIVE),
                new Literal(TEST_LITERAL_1, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.NEGATIVE);
            Statement ss2 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_1, Literal.NEGATIVE),
                new Literal(TEST_LITERAL_1, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Add()
            }, Literal.POSITIVE);

            Statement ss3 = new Statement(new List<AbstractStatement>()
            {
                new Literal(TEST_LITERAL_2, Literal.NEGATIVE),
                new Literal(TEST_LITERAL_1, Literal.POSITIVE)
            }, new List<IOperator>()
            {
                new Operators.Sub()
            }, Literal.POSITIVE);

            Statement s1 = new Statement(
                new List<AbstractStatement>() { ss1, ss2 },
                new List<IOperator>() { new Operators.Sub() },
                Literal.NEGATIVE);

            Statement s2 = new Statement(
                new List<AbstractStatement>() { ss3, ss2 },
                new List<IOperator>() { new Operators.Sub() },
                Literal.NEGATIVE);

            Assert.IsFalse(s1.Equals(s2));
        }
    }
}
