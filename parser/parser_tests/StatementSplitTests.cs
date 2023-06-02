using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using parser;
using System.Collections.Generic;

namespace parser_tests
{
    [TestClass]
    public class StatementSplitTests
    {

        private readonly Statement TEST_STATEMENT_LARGE = new Statement(
        new List<AbstractStatement>()
        {
            new Literal(2, Literal.POSITIVE),
            new Literal(3, Literal.POSITIVE),
            new Literal(4, Literal.POSITIVE),
            new Literal(5, Literal.POSITIVE)
        },
        new List<IOperator>()
        {
            new Operators.Add(),
            new Operators.Add(),
            new Operators.Add()
        },
        Literal.POSITIVE);

        [TestMethod]
        public void HeadTail_MiddleOperator_Splits()
        {
            int MIDDLE = 1;

            Statement s1 = TEST_STATEMENT_LARGE.Head(MIDDLE);
            Statement s2 = TEST_STATEMENT_LARGE.Tail(MIDDLE);

            Statement supposedS1 = new Statement(
            new List<AbstractStatement>()
            {
                new Literal(2, Literal.POSITIVE),
                new Literal(3, Literal.POSITIVE)
            },
            new List<IOperator>()
            {
                new Operators.Add()
            },
            Literal.POSITIVE);

            Statement supposedS2 = new Statement(
            new List<AbstractStatement>()
            {
                new Literal(4, Literal.POSITIVE),
                new Literal(5, Literal.POSITIVE)
            },
            new List<IOperator>()
            {
                new Operators.Add()
            },
            Literal.POSITIVE);

            Assert.IsTrue(s1.Equals(supposedS1));
            Assert.IsTrue(s2.Equals(supposedS2));
        }

        [TestMethod]
        public void HeadTail_FirstOperator_Splits()
        {
            int FIRST = 0;

            Statement s1 = TEST_STATEMENT_LARGE.Head(FIRST);
            Statement s2 = TEST_STATEMENT_LARGE.Tail(FIRST);

            Statement supposedS1 = new Statement(
            new List<AbstractStatement>()
            {
                new Literal(2, Literal.POSITIVE)
            },
            new List<IOperator>()
            {
            },
            Literal.POSITIVE);

            Statement supposedS2 = new Statement(
            new List<AbstractStatement>()
            {
                new Literal(3, Literal.POSITIVE),
                new Literal(4, Literal.POSITIVE),
                new Literal(5, Literal.POSITIVE)
            },
            new List<IOperator>()
            {
                new Operators.Add(),
                new Operators.Add()
            },
            Literal.POSITIVE);

            Assert.IsTrue(s1.Equals(supposedS1));
            Assert.IsTrue(s2.Equals(supposedS2));
        }
    }
}
