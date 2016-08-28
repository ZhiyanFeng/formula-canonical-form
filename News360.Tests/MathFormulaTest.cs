using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace News360.Tests
{
    [TestClass]
    public class MathFormulaTests
    {
        [TestMethod]
        public void getRidOfBracketTest()
        {
            MathFormula mf = new MathFormula(); 
            string testString = "(x+y)";
            string testString2 = "(x+y)+(w-z)";
            string testString3 = "((x+y)-(w-z))";
            string testString4 = "-(-y-z)";
            string testString5 = "x-(+y-z)";
            string result = mf.getRidOfBracket(testString);
            string result2 = mf.getRidOfBracket(testString2);
            string result4 = mf.getRidOfBracket(testString4);
            string result5 = mf.getRidOfBracket(testString5);
            string result3 = mf.getRidOfBracket(testString3);
            
            

            Assert.AreEqual("x+y", result);
            Assert.AreEqual("x+y+w-z", result2);
            Assert.AreEqual("x+y-w+z", result3);
            Assert.AreEqual("y+z", result4);
            Assert.AreEqual("x-y+z", result5);
        }
    }
}
