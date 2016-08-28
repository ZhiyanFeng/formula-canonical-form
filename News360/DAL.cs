using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace News360
{
    class DAL
    {
        public string inputFormula { get; set; }
        public string fullName { get; set; }

        private string[] lines;
        private List<string> transformedFormulaList = new List<string>();
        private MathFormula mf;

        public DAL()
        {

        }

        public DAL(string inputString)
        {
            this.inputFormula = inputString;
        }


        public void accessFileData()
        {
            try
            {
                lines = File.ReadAllLines(fullName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void processFileData()
        {
            foreach (string line in lines)
            {
                mf = new MathFormula(line);
                mf.transform();
                transformedFormulaList.Add(mf.outputFormula);
            }
        }

        public void outputFileData()
        {
            foreach (string line in transformedFormulaList)
            {
                string newFileName = Path.ChangeExtension(fullName, ".out");
                using (StreamWriter sw = File.AppendText(newFileName))
                {
                    sw.WriteLine(line);
                }
            }
        }

        public void accessConsoleData()
        {
            inputFormula = Console.ReadLine();
        }

        public void processConsoleData()
        {
            mf = new MathFormula(inputFormula);
            mf.transform();
        }

        public void outputToConsole()
        {
            Console.WriteLine(mf.outputFormula);
        }
    }
}
