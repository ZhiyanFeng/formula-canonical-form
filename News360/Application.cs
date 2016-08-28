using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace News360
{
    class Application
    {
        public string file { get; set; }
        public string inputFormula { get; set; }
        
        public Application()
        {
            
        }

        public void start()
        {
            Console.Clear();
            string mode;
            while (true)
            {
                Console.WriteLine("Please input modes of operation, input F for file mode followed by space and a file name, I for interactive mode");
                mode = Console.ReadLine();
                DAL dataProcess = new DAL();
                string[] inputParts = mode.Split(' ');

                switch (inputParts[0])
                {
                    case "F":
                        dataProcess.fullName = inputParts[1];
                        dataProcess.accessFileData();
                        dataProcess.processFileData();
                        dataProcess.outputFileData();
                        break;
                    case "I":
                        Console.WriteLine("Please input expression:");
                        dataProcess.accessConsoleData();
                        dataProcess.processConsoleData();
                        dataProcess.outputToConsole();
                        break;
                    default:
                        break;
                }

                
            }
        }

    };
 
}
