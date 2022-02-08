using JavaScriptEngineSwitcher.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PADscript
{
    public class v8
    {
        private IJsEngine engine;

        public int createEngine()
        {
            try
            {
                var libb = new LIB_ADDER_V1();
                libb.getLibrarySystem();

                var lib_adder = new Library_Adder();
                engine = lib_adder.getLibraryJSengine();
                engine.EmbedHostType("log", typeof(Console));
                engine.EmbedHostType("file", typeof(File));
                engine.EmbedHostType("env", typeof(Environment));
                engine.EmbedHostType("MStype", typeof(Type));
                engine.EmbedHostType("core", typeof(core));
                // engine.EmbedHostType("graphics", typeof(runGraphicsApp));
                return 1;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return -1;
            }
        }


        public bool executeJScode(string code = null, string fileName = null, string[] jsLibs = null)
        {
            try
            {
                if (jsLibs != null)
                {
                    if (jsLibs.Length > 0)
                    {
                        foreach (var lib in jsLibs)
                        {
                            engine.PrecompileFile(lib);
                        }
                    }
                }


                if (code != null)
                {
                    if (File.Exists(Environment.CurrentDirectory + "\\tmp.js"))
                    {
                        File.Delete(Environment.CurrentDirectory + "\\tmp.js");
                    }

                    string[] args =
                    {
                        // "var file = require('file');", 
                        // "var sms = require('sms');",
                        // "var console = require('console');", 
                        // "var vision = require('padVision');",
                        code
                    };
                    File.WriteAllLines(Environment.CurrentDirectory + "\\tmp.js", args);
                    engine.ExecuteFile(Environment.CurrentDirectory + "\\tmp.js");
                }
                else
                {
                    engine.ExecuteFile(fileName);
                }
            }
            catch (Exception i)
            {
                if (code.Contains("help.getFunctions("))
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("ERROR: " + i.Message);
                    Console.WriteLine("");
                    Console.WriteLine("Try using thisPlugin.listFunctions(); instead");
                    Console.WriteLine("--------------------------------------------");
                    return false;
                }

                Console.WriteLine(i);
                return false;
            }

            return true;
        }
    }
}
