using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ACM
{
    class Program
    {
        static void Main(string[] args)
        {
            String executable;
            String arguments;
            int progTimeLimit;
            int testCaseTimeLimit;
            int memoryLimit;
            String workDir;
            int numOfTestCases;
            

            testCaseTimeLimit = Convert.ToInt32(Console.ReadLine());
            progTimeLimit = Convert.ToInt32(Console.ReadLine());
            memoryLimit = Convert.ToInt32(Console.ReadLine());

            String cmdLine = Console.ReadLine();
            int pos = cmdLine.IndexOf(' ');
            if (pos == -1)
            {
                executable = cmdLine;
                arguments = "";
            }
            else
            {
                executable = cmdLine.Substring(0, pos);
                arguments = cmdLine.Substring(pos + 1, cmdLine.Length - pos - 1);
            }

            workDir = Console.ReadLine();
            numOfTestCases = Convert.ToInt32(Console.ReadLine());

            String[] inFiles = new String[numOfTestCases];
            String[] outFiles = new String[numOfTestCases];

            for(int i=0;i<numOfTestCases;i++){
                inFiles[i] = Console.ReadLine();
                outFiles[i] = Console.ReadLine();
            }

            RunCmd rc = new RunCmd(executable,arguments,testCaseTimeLimit,progTimeLimit,memoryLimit,workDir,numOfTestCases,inFiles,outFiles);
            Thread th = new Thread(rc.doWork);
            th.Start();
        }
    }
}
