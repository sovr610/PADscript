using System;
using NLua;
using KeraLua;
using System.Linq;

namespace PADscript
{
    public class Program
    {

        static void Main(string[] args)
        {
            Script script;
            Library_Adder adder;
            LIB_ADDER_V1 lib;
            v8 js = new v8();
            js.createEngine();
            core c = new core();

            
            script = new Script();

            script = c.loadScript(script);


            adder = new Library_Adder();
            lib = new LIB_ADDER_V1();
            lib.getLibrarySystem();
            script = adder.loadScript(script);
            

            if (args.Length == 0 || (args.Length == 1 && (args.Contains("--v8") || args.Contains("--lua"))))
            {
                bool isJS = false;
                if (args.Length == 0)
                {
                    Console.Write("do you want JavaScript or Lua to run as the core engine (js/lua): ");
                    string type = Console.ReadLine();
                    if (type.Trim().ToLower() == "js")
                    {
                        isJS = true;
                    }
                    

                }
                Console.Clear();
                Console.WriteLine("PADscript v1.0");
                Console.WriteLine("/help for help");

                while (true)
                {
                    try
                    {
                        if (args.Contains("--lua") || isJS == false)
                        {
                            Console.Write(">");
                            string info = Console.ReadLine();
                            if (info.Trim().ToLower() == "/help")
                            {
                                Console.WriteLine("Base scripting engine is based on Lua 5.1");
                                script.executeScriptLine("PrintCommands()");
                            }
                            else
                            {
                                script.executeScriptLine(info);
                            }
                        }

                        if (args.Contains("--v8") || isJS)
                        {
                            Console.Write(">");
                            string info = Console.ReadLine();
                            if (info.Trim().ToLower() == "/help")
                            {
                                Console.WriteLine("Base scripting engine is based on V8 JavaScript engine");
                            }
                            else
                            {
                                js.executeJScode(info, null, null);
                            }
                        }

                    }
                    catch (Exception p)
                    {
                        Console.WriteLine(p);
                    }
                }
            }
            else
            {
                
                if(args.Contains("-h") || args.Contains("--help")){
                    Console.WriteLine("-f, --file: The filename to run (optinal)");
                    Console.WriteLine("create-plugin: create a project template for a plugin. args: -n, --name: name of plugin");
                    Console.WriteLine("--v8: This is for using the JavaScript syntax to run the functions you want padScript to do");
                    Console.WriteLine("--lua: This is for using the Lua syntax to run the functions you want padScript to do");
                }

                if (args.Contains("create-plugin"))
                {
                    if(args.Contains("-n") || args.Contains("--name"))
                    {
                        string name = null;
                        int index = 0;
                        foreach(var arg in args)
                        {
                            if(arg == "-n" || arg == "--name")
                            {
                                name = args[index + 1];
                            }
                            index++;
                        }

                        if(name == null)
                        {
                            Console.WriteLine("No name is supplied");
                        }
                        else
                        {
                            core.buildPluginProject(name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("no name is supplied for the plugin (-n,--name)");
                    }
                }

                if(args.Contains("-f") || args.Contains("--file"))
                {
                    try
                    {
                        int index = 0;
                        string val = null;
                        foreach(var ind in args)
                        {
                            if(ind == "-f" || ind == "--file")
                            {
                                val = args[index + 1];
                            }

                            index++;
                        }
                        if(val == null)
                        {
                            Console.WriteLine("no file supplied!");
                        }
                        else
                        {
                            string dir = "";
                            if (val.Contains("/") || val.Contains("\\"))
                            {
                                dir = val;
                            }
                            else
                            {
                                dir = Environment.CurrentDirectory + "\\" + val;
                            }

                            if (args.Contains("--lua"))
                            {
                                script.executeScriptFile(dir);
                            }

                            if (args.Contains("--v8"))
                            {
                                js.executeJScode(null, dir, null);
                            }
                        }

                    }catch(Exception i)
                    {
                        Console.WriteLine(i);
                    }
                }
            }
        }
    }
}
