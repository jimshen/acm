using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ACM
{
    class RunCmd
    {
        private String executable;
        private String arguments;
        private int progTimeLimit;
        private int testCaseTimeLimit;
        private int memoryLimit;
        private String workDir;
        private int numOfTestCases;
        private String[] inFiles;
        private String[] outFiles;

        public RunCmd(String executable, String arguments, int testCaseTimeLimit, int progTimeLimit, int memoryLimit, String workDir, int numOfTestCases, String[] inFiles, String[] outFiles)
        {
            this.executable = executable;
            this.arguments = arguments;
            this.testCaseTimeLimit = testCaseTimeLimit;
            this.progTimeLimit = progTimeLimit;
            this.memoryLimit = memoryLimit;
            this.workDir = workDir;
            this.numOfTestCases = numOfTestCases;
            this.inFiles = inFiles;
            this.outFiles = outFiles;
        }

        public int CompareResult(String str, int i)
        {
            String res = "";
            StreamReader sr = File.OpenText(outFiles[i]);
            while (sr.Peek() != -1)
            {
                string s = sr.ReadLine();
                //Console.WriteLine(s);
                res = res + s + "\r\n";
            }
            sr.Close();
            if (String.Equals(res, str))
                return 0;
            String s1 = new Regex(@"[\r\n\s]*").Replace(str, "");
            String s2 = new Regex(@"[\r\n\s]*").Replace(res, "");

            if (s1.Equals(s2))
                return 1;
            else
                return 4;
        }

        public void doWork()
        {

            int singleTime = 0;
            int totalTime = 0;
            long maxMems = -1;

            if (numOfTestCases != 0)
            {

                for (int i = 0; i < numOfTestCases; i++)
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = executable;
                    proc.StartInfo.Arguments = arguments;
                    proc.StartInfo.UseShellExecute = false;

                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = false;

                    string strRst = "";
                    try
                    {
                        proc.Start();

                        StreamWriter myStreamWriter = proc.StandardInput;


                        StreamReader sr = File.OpenText(inFiles[i]);
                        while (sr.Peek() != -1)
                        {
                            string str = sr.ReadLine();
                            myStreamWriter.WriteLine(str + "\n");
                        }
                        sr.Close();


                        long mems = proc.WorkingSet64;

                        if (mems > memoryLimit)
                        {
                            Console.WriteLine(3);  //Time Limit Exceeded
                            return;
                        }
                        if (mems > maxMems)
                            maxMems = mems;

                        Thread.Sleep(testCaseTimeLimit);

                        if (proc.HasExited)
                        {
                            int exitCode = proc.ExitCode;
                            strRst = proc.StandardOutput.ReadToEnd();
                            String errmsg = proc.StandardError.ReadToEnd();
                            if (exitCode == 0 && errmsg.Trim().Length == 0)
                            {
                                int x = CompareResult(strRst, i);
                                if (x != 0)
                                {
                                    Console.WriteLine(x);
                                    return;
                                }
                                singleTime = (proc.ExitTime - proc.StartTime).Milliseconds;
                                totalTime = totalTime + singleTime;
                            }
                            else
                            {
                                Console.WriteLine(5);  //Run time error
                                Console.Error.WriteLine(errmsg);
                                Console.Error.Flush();
                                return;
                            }

                        }
                        else
                        {
                            proc.Kill();
                            Console.WriteLine(2);  //Time Limit Exceeded!
                            return;
                        }


                    }
                    catch (OutOfMemoryException ex)
                    {
                        Console.WriteLine(6);  //Output Limit Exceeded
                        Console.Error.WriteLine(ex.ToString());
                        Console.Error.Flush();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(98);  //system error
                        Console.Error.WriteLine(ex.ToString());
                        Console.Error.Flush();
                        return;
                    }
                    finally
                    {
                        if (!proc.HasExited)
                            proc.Close();
                    }
                }
                if (totalTime > progTimeLimit)
                {
                    Console.WriteLine(3);  //Time Limit Exceeded
                    return;
                }

                Console.WriteLine(0); //Accepted
                Console.WriteLine(totalTime / numOfTestCases); //Time Costs
                Console.WriteLine(maxMems / 1024); //Max Memory Usage
            }
            else
            {

                Process proc = new Process();
                proc.StartInfo.FileName = executable;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;

                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = false;

                string strRst = "";

                try
                {
                    proc.Start();
                    StreamWriter myStreamWriter = proc.StandardInput;

                    long mems = proc.WorkingSet64;

                    if (mems > memoryLimit)
                    {
                        Console.WriteLine(3);
                        return;
                    }

                    Thread.Sleep(testCaseTimeLimit);

                    if (proc.HasExited)
                    {
                        int exitCode = proc.ExitCode;
                        strRst = proc.StandardOutput.ReadToEnd();
                        String errmsg = proc.StandardError.ReadToEnd();
                        if (exitCode == 0 && errmsg.Trim().Length == 0)
                        {
                            Console.WriteLine(CompareResult(strRst, 0));
                            Console.WriteLine((proc.ExitTime - proc.StartTime).Milliseconds);
                            Console.WriteLine(mems);
                            Console.WriteLine(strRst);
                        }
                        else
                        {
                            Console.WriteLine(5);  //Run time error
                            Console.Error.WriteLine(errmsg);
                            Console.Error.Flush();
                        }

                    }
                    else
                    {
                        proc.Kill();
                        Console.WriteLine(2);  //Time Limit Exceeded!
                    }

                }
                catch (OutOfMemoryException ex)
                {
                    Console.WriteLine(6);  //system error
                    Console.Error.WriteLine(ex.ToString());
                    Console.Error.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(98);  //system error
                    Console.Error.WriteLine(ex.ToString());
                    Console.Error.Flush();
                }
                finally
                {
                    if (!proc.HasExited)
                        proc.Close();
                }
            }
            //Console.ReadKey(true);
        }
    }
}
