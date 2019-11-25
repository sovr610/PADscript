using System;
using System.Collections.Generic;
using System.Text;

namespace PADscript
{
    public class core
    {
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
