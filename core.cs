using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PADscript
{
    public class core
    {
        private readonly string dir_lib = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PADScript\\plugins\\Library\\";
        public bool buildPluginProject(string pluginName)
        {
            try
            {
                string cmd = "dotnet new classlib -lang \"C#\" -n \"" + pluginName + "\" -f netcoreapp3.0";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = cmd;
                process.StartInfo = startInfo;
                process.Start();
                return true;
            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return false;
            }
        }

        public bool integratePlugin()
        {
            try
            {
                string build = "dotnet build --configuration Release";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = build;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                string tempDir = Environment.CurrentDirectory + "\\bin\\Release\\netcoreapp3.0";
                string[] files = Directory.GetFiles(tempDir);
                List<string> dll = new List<string>();
                foreach(var file in files)
                {
                    var test = file.Split('.')[1];
                    if (file.Split('.')[1] == "dll")
                    {
                        int slash = file.LastIndexOf('\\');
                        string filesName = file.Substring(slash).Replace('\\', ' ');
                        File.Copy(file, dir_lib + filesName);
                    }
                }
                Console.WriteLine("plugins have been added");
                return true;
                
            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return false;
            }
        }


        public void speak(string say)
        {
            Speech(say);
        }

        private static void Speech(string textToSpeech, bool wait = false)
        {
            try
            {
                // Command to execute PS  
                Execute($@"Add-Type -AssemblyName System.speech;  
            $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;                           
            $speak.Speak(""{textToSpeech}"");"); // Embedd text  

                void Execute(string command)
                {
                    // create a temp file with .ps1 extension  
                    var cFile = System.IO.Path.GetTempPath() + Guid.NewGuid() + ".ps1";

                    //Write the .ps1  
                    using var tw = new System.IO.StreamWriter(cFile, false, Encoding.UTF8);
                    tw.Write(command);

                    // Setup the PS  
                    var start =
                        new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = "C:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe",  // CHUPA MICROSOFT 02-10-2019 23:45                    
                        LoadUserProfile = false,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = $"-executionpolicy bypass -File {cFile}",
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                        };

                    //Init the Process  
                    var p = System.Diagnostics.Process.Start(start);
                    // The wait may not work! :(  
                    if (wait) p.WaitForExit();
                }
            }catch(Exception i)
            {
                Log.RecordError(i);
            }
        }

        public Script loadScript(Script l)
        {
            l.addFunc("speak", this, false);
            return l;
        }
    }
}
