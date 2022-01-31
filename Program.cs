using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWinLogon
{
    internal class Program
    {
        static string consoleOutput = string.Empty;

        static void Main(string[] args)
        {
            PsGetSid(true);
            PsLoggedon(true);
            Subst("D:", "C:\\Users\\heinr");
            SDelete("c:\\Users\\heinr\\AppData\\Local\\Temp", "*.", 2, true);
            SDelete("c:\\Users\\heinr\\AppData\\Local\\Temp", "*.*", 1, true);
            SDelete("c:\\Windows\\Temp", "*.", 1, true);
            SDelete("c:\\Windows\\Temp", "*.*", 2, true);
            PsPing(false);
            ProcessExplorer(true);

        }

        static bool Subst(string targetSubstitutionDrive, string sourceDirectory, bool adminPrivilegePermission = false)
        {
            // Run "Subst.exe {targetSubstitutionDrive} {sourceDirectory}"
            try
            {
                using (Process compiler = new Process())
                {
                    compiler.StartInfo.FileName = "subst ";
                    DirectoryInfo di = new DirectoryInfo(sourceDirectory);
                    if (!di.Exists)
                    {
                        throw new InvalidProgramException($"Directory sourceDirectory {sourceDirectory} doesn't exists");
                    }
                    compiler.StartInfo.Arguments = $"{targetSubstitutionDrive} {sourceDirectory}";
                    compiler.StartInfo.UseShellExecute = false;
                    compiler.StartInfo.RedirectStandardOutput = true;
                    compiler.Start();

                    consoleOutput = compiler.StandardOutput.ReadToEnd();
                    Console.WriteLine(consoleOutput);

                    compiler.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}\n");
                return false;
            }

            return true;
        }


        static int SDelete(string filepath, string wildcards = "*.*", 
            int trys = 1, bool recursive = true, bool adminPrivilegePermission = false)
        {
            // Run "sdelete.exe  -p {trys} (recursive) ? " -r " : "" {filepath}\\{wildcards}"
            //  or start "sdelete64.exe  -p {trys} (recursive) ? " -r " : "" {filepath}\\{wildcards}" 
            //  when adminPrivilegePermission is set to true
            using (Process compiler = new Process())
            {
                compiler.StartInfo.FileName = (adminPrivilegePermission) ? "sdelete64.exe" : "sdelete.exe";
                string argTrys = $" -p {trys} ";
                string argRecursive = (recursive) ? " -r " : string.Empty;
                DirectoryInfo di = new DirectoryInfo(filepath);
                if (!di.Exists) {
                    throw new InvalidProgramException($"Directory filepath {filepath} doesn't exists");
                }
                compiler.StartInfo.Arguments = $"{argTrys} {argRecursive} {filepath}\\{wildcards}";
                compiler.StartInfo.UseShellExecute = false;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.Start();

                consoleOutput = compiler.StandardOutput.ReadToEnd();
                Console.WriteLine(consoleOutput);
                
                int lineFeeds = consoleOutput.AsQueryable().Count(ch => (ch == '\n'));

                compiler.WaitForExit();
                
                return lineFeeds;
            }
        }

        static bool PsGetSid(bool adminPrivilegePermission = false)
        {
            // Run "PsGetSid.exe" or "PsGetSid64.exe" when adminPrivilegePermission = true
            try
            {
                using (Process compiler = new Process())
                {
                    compiler.StartInfo.FileName = (adminPrivilegePermission) ? "PsGetsid64.exe" : "PsGetsid.exe";
                    // compiler.StartInfo.Arguments = $"{arg}";
                    compiler.StartInfo.UseShellExecute = false;
                    compiler.StartInfo.RedirectStandardOutput = true;
                    compiler.Start();

                    consoleOutput = compiler.StandardOutput.ReadToEnd();
                    Console.WriteLine(consoleOutput);

                    compiler.WaitForExit();
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}\n");
                return false;
            }

            return true;
        }

        static bool PsLoggedon(bool adminPrivilegePermission = false)
        {
            // Run "PsLoggedon.exe" or "PsLoggedon64.exe" when adminPrivilegePermission = true
            try
            {
                using (Process compiler = new Process())
                {
                    compiler.StartInfo.FileName = (adminPrivilegePermission) ? "PsLoggedon64.exe" : "PsLoggedon.exe";
                    // compiler.StartInfo.Arguments = $"{arg}";
                    compiler.StartInfo.UseShellExecute = false;
                    compiler.StartInfo.RedirectStandardOutput = true;
                    compiler.Start();

                    consoleOutput = compiler.StandardOutput.ReadToEnd();
                    Console.WriteLine(consoleOutput);

                    compiler.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}\n");
                return false;
            }

            return true;
        }


        static int PsPing(bool adminPrivilegePermission = false)
        {
            // Console.ReadKey();
            Console.WriteLine("psping with Bandwidth test with request size of 1k, 2k to ");
            Console.WriteLine("A1 DNS servers 213.33.42.97 213.33.42.113 ");
            Console.WriteLine("nic.at  131.130.249.233 \tunivie.ac.at 131.130.70.63");
            Console.WriteLine("orf.at ipv6 2a01:468:1000:9::141 ");
            // Run "psping.exe" or "psping64.exe" when adminPrivilegePermission = true
            // run Bandwidth test with request size of 1k, 2k, 4k to
            // A1 DNS servers 213.33.42.97 213.33.42.113 
            // nic.at  131.130.249.233
            // univie.ac.at 131.130.70.63
            // orf.at ipv6 2a01:468:1000:9::141 
            List<TimeSpan> tsList = new List<TimeSpan>();
            string[] args = { " -w 3 -n 14 131.130.249.233 ",
                " -b -l 1k 213.33.42.97 ", " -b -l 2k 213.33.42.113  ",
                " -w 2 -n 12 orf.at", " -w 2 -n 12 orf.at"
                // " -w 2 -n 12 orf.at", " -w 2 -n 12 -6 2a01:468:1000:9::141"
                };

            foreach (string arg in args)
            {
                using (Process compiler = new Process())
                {
                    compiler.StartInfo.FileName = (adminPrivilegePermission) ? "psping64.exe" : "psping.exe";                   
                    compiler.StartInfo.Arguments = $"{arg}";
                    compiler.StartInfo.UseShellExecute = false;
                    compiler.StartInfo.RedirectStandardOutput = true;
                    compiler.Start();

                    consoleOutput = compiler.StandardOutput.ReadToEnd();
                    Console.WriteLine(consoleOutput);
                    string parseOutput = consoleOutput;
                    while((parseOutput.IndexOf(":") > -1) && (parseOutput.Contains("ms")))
                    {
                        parseOutput = parseOutput.Substring(parseOutput.IndexOf(":") + 1);
                        int chIdx = 0;
                        while (parseOutput[chIdx] == ':' && !Char.IsDigit(parseOutput[chIdx])) chIdx++;
                        string milliSeconds = parseOutput.Substring(chIdx, parseOutput.IndexOf("ms"));
                        double msec = 0;
                        if (Double.TryParse(milliSeconds.Trim("ms".ToArray()), out msec))
                        {
                            TimeSpan timeSpan = TimeSpan.FromMilliseconds(msec);
                            tsList.Add(timeSpan);
                        }
                        parseOutput = parseOutput.Substring(parseOutput.IndexOf("ms"));
                        parseOutput = parseOutput.Substring(parseOutput.IndexOf('\n'));
                    }

                    compiler.WaitForExit();
                }
            }

            return tsList.Count;
        }


        static bool ProcessExplorer(bool adminPrivilegePermission = false)
        {
            Console.ReadKey();
            Console.WriteLine("Run Procexp64.exe");
            // Run "Procexp.exe" or "Procexp64.exe" when adminPrivilegePermission = true
            try
            {
                using (Process compiler = new Process())
                {
                    compiler.StartInfo.FileName = (adminPrivilegePermission) ? "Procexp64.exe" : "Procexp.exe";
                    // compiler.StartInfo.Arguments = $"{arg}";
                    compiler.StartInfo.UseShellExecute = true;
                    compiler.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    compiler.StartInfo.LoadUserProfile = true;

                    compiler.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}\n");
                return false;
            }

            return true;
        }

    }
}
