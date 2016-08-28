using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace News360
{
    /// <summary>
    /// This is a class representation of a mathematical formula. Taken a literal string (or collection of characters) and an environment with
    /// variable definitions, it calculates transformation of said formula.
    /// </summary>
    public class MathFormula
    {
        #region Members
        /// <summary>
        /// Constant for left association symbols
        /// </summary>
        private readonly int LEFT_ASSOC = 0;

        //set up the regular expression for class use
        private Regex splitRx = new Regex(@"([\+\-\(\)\ ])");
        private Regex tokenRx = new Regex(@"[-+]*\d*\.*\d*[a-z^0-9]*");
        private Regex hasPowerRX = new Regex(@"[a-z]{1}\^[0-9]*");
        private Regex singleInMiddleRx = new Regex(@"[a-z]{1}(?=[a-z]{1})");
        private Regex singleRx = new Regex(@"[a-z]{1}(?![a-z^]{1})");
        private Regex floatNumRx = new Regex(@"[+-]*[0-9]+\.?[0-9]+(?![0-9a-z]+)|((?<!\^)\d(?![\.a-z]))");
        private Regex verificationRx = new Regex(@"[-+]{2}|[=]{2}|[-+]=|=(?![0-9a-z-]+)|((?<![0-9a-z)]+)=)|[-+](?![0-9a-z(]+)|\.(?![0-9]+)");

        //property related to formula
        public string inputFormula { get; set; }
        public string outputFormula { get; set; }
        public string leftFormula = null;
        public string rightFormula = null;

        /// <summary>
        /// Static list of operators in the formula
        /// </summary
        private Dictionary<String, int[]> OPERATORS = new Dictionary<string, int[]>();

        public MathFormula()
        {
            OPERATORS.Add("+", new int[] { 0, LEFT_ASSOC });
            OPERATORS.Add("-", new int[] { 0, LEFT_ASSOC });
            //OPERATORS.Add("^", new int[] { 10, RIGHT_ASSOC });
        }

        public MathFormula(string inputFormula)
            : this()
        {
            if (!verificationRx.IsMatch(inputFormula))
            {
                this.inputFormula = inputFormula;
            }
        }

        /// <summary>
        /// Static method to check if a token is an operator.
        /// </summary>
        /// <param name="token">The token we want to check.</param>
        /// <returns>True if it is an operator, else false.</returns>
        private bool isOperator(String token)
        {
            return OPERATORS.ContainsKey(token);
        }

        //sort every summand to order say, x^2y and yX^2 to x^2y
        public string sortSummand(string input)
        {
            SortedDictionary<char, string> dict = new SortedDictionary<char, string>();

            foreach (Match item in hasPowerRX.Matches(input))
            {
                char key = item.Value[0];
                dict.Add(key, item.Value);
            }
            foreach (Match item in singleInMiddleRx.Matches(input))
            {
                char key = item.Value[0];
                dict.Add(key, item.Value);
            }
            foreach (Match item in singleRx.Matches(input))
            {
                char key = item.Value[0];
                dict.Add(key, item.Value);
            }
            StringBuilder newExpression = new StringBuilder();
            foreach (char key in dict.Keys)
            {
                newExpression.Append(dict[key]);
            }
            return newExpression.ToString();
        }
        public static double getCoefficient(string input)
        {
            Regex floatRx = new Regex(@"[-+]*\d+\.\d+");
            string match = floatRx.Match(input).Value;
            double coefficient = 1.0;
            if (match != "")
            {
                coefficient = Convert.ToDouble(match);
            }
            else if (input[0] == '-')
            {
                coefficient = -coefficient;
            }
            return coefficient;
        }

        //get rid of the bracket e.g. (x+y)-(w-z) => x+y-w+z 
        //borrow the idea from shunging yard algorithm
        public String getRidOfBracket(String input)
        {

            List<String> tokenList = splitRx.Split(input).Select(t => t.Trim()).Where(t => t != "").ToList();


            Stack<string> outList = new Stack<string>();
            Stack<string> stack = new Stack<string>();

            foreach (string token in tokenList)
            {
                if (token == " " || token == String.Empty)
                    continue;
                int opInBracket = 0;

                if (isOperator(token))
                {
                    stack.Push(token);
                }
                else if (token == "(")
                {
                    stack.Push(token);
                    opInBracket = 0;
                }
                else if (token == ")")
                {
                    while (stack.Count != 0 && stack.Peek() != "(")
                    {
                        outList.Push(stack.Pop());
                        opInBracket +=1;
                    }
                    stack.Pop();

                    Boolean emptyStack = stack.Count() == 0 ? true : false;
                    string topOp = null;
                    if (!emptyStack)
                    {
                        topOp = stack.Peek();
                        if (opInBracket >= 2)
                        {
                            topOp = stack.Pop();
                        }
                    }

                    
                    while (isOperator(outList.Peek()))
                    {
                        if (!emptyStack)
                        {
                            if (topOp == "-")
                            {
                                //if (opInBracket>2)
                                //{
                                //    stack.Pop();
                                //    opInBracket = 2;
                                //}
                                if (outList.Peek() == "-")
                                {
                                    outList.Pop();
                                    stack.Push("+");
                                }
                                else if (outList.Peek() == "+")
                                {
                                    outList.Pop();
                                    stack.Push("-");
                                }
                            }
                            else
                            {
                                stack.Push(outList.Pop());
                            }
                        }
                        else
                        {
                            stack.Push(outList.Pop());
                        }
                    }
                }

                else
                {
                    outList.Push(token);
                }
            }

            Stack<string> summandStack = new Stack<string>();
            Stack<string> opStack = new Stack<string>();

            while (outList.Count != 0)
            {
                summandStack.Push(outList.Pop());
            }
            while (stack.Count != 0)
            {
                opStack.Push(stack.Pop());
            }



            StringBuilder builder = new StringBuilder();
            while (summandStack.Count > 0)
            {
                builder.Append(summandStack.Pop());
                if (summandStack.Count != 0)
                {
                    builder.Append(opStack.Pop());
                }
                ;
            }
            //Console.WriteLine(builder.ToString());
            return builder.ToString();
        }

        //extract left and right of the equation
        public void getLeftAndRight(string equation, ref string left, ref string right)
        {
            try
            {
                string[] split = equation.Split('=');
                left = split[0];
                right = split[1];

                if (right.Trim() == "0")
                {
                    //if right size of the equation is 0 , nothing change
                    outputFormula = equation;
                }
                else if (left.Trim() == "0")
                {
                    //if left size is 0, then just swith the direction
                    outputFormula = right + "=0";
                }
                else
                {
                    left = floatNumRx.IsMatch(left) ? left : getRidOfBracket(left);
                    right = floatNumRx.IsMatch(right) ? right : getRidOfBracket(right);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Equation not valid!");
            }

        }

        public void parseFormula(Dictionary<string, double> dict, MatchCollection match, string flag = "Left")
        {
            foreach (Match item in match)
            {
                if (item.Value == "")
                {
                    continue;
                }
                Boolean checkFloat = floatNumRx.IsMatch(item.Value);
                double coeff = checkFloat ? Convert.ToDouble(item.Value) : getCoefficient(item.Value);
                string key = checkFloat ? "constant" : sortSummand(item.Value);

                if (flag == "Left")
                {
                    if (dict.ContainsKey(key))
                    {
                        dict[key] = dict[key] + coeff;
                    }
                    dict.Add(key, coeff);
                }
                else if (flag == "Right")
                {
                    if (dict.ContainsKey(key))
                    {
                        dict[key] = dict[key] - coeff;
                    }
                    else
                    {
                        dict.Add(key, -coeff);
                    }
                }

            }
        }
        public void transform()
        {
            getLeftAndRight(inputFormula, ref leftFormula, ref rightFormula);
            Dictionary<string, double> tokenDict = new Dictionary<string, double>();
            if (outputFormula == null)
            {
                try
                {
                    //parse left side of the formula and add to tokenDict
                    MatchCollection leftColl = tokenRx.Matches(leftFormula);
                    parseFormula(tokenDict, leftColl);
                    //parse right side
                    MatchCollection rightColl = tokenRx.Matches(rightFormula);
                    parseFormula(tokenDict, rightColl, "Right");
                    outputFormula = assembleFormula(tokenDict);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public string assembleFormula(Dictionary<string, double> dict)
        {
            StringBuilder final = new StringBuilder();

            foreach (KeyValuePair<string, double> item in dict)
            {
                if (item.Key != "constant")
                {
                    if (item.Value < 0)
                    {
                        if (item.Value != -1)
                        {
                            string coeff = Convert.ToString(item.Value);
                            final.Append(item.Value);
                            final.Append(item.Key);
                        }
                        else
                        {
                            //get rid of preceding 1,  this way can be improved by regular expression as well
                            final.Append("-");
                            final.Append(item.Key);
                        }

                    }
                    else if (item.Value > 0)
                    {
                        if (item.Value != 1)
                        {
                            string coeff = Convert.ToString(item.Value);
                            final.Append("+");
                            final.Append(item.Value);
                            final.Append(item.Key);
                        }
                        else
                        {
                            final.Append("+");
                            final.Append(item.Key);
                        }
                        ;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {

                    if (item.Value > 0)
                    {
                        final.Append("+");
                        final.Append(Convert.ToString(item.Value));
                    }
                    else
                    {
                        final.Append(Convert.ToString(item.Value));
                    }
                }
            }
            final.Append("=0");
            return final.ToString();
        }
        #endregion
    }
}
