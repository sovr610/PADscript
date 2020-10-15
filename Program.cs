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
            core c = new core();

            Console.WriteLine("PADscript v1.0");
            script = new Script();

            script = c.loadScript(script);


            adder = new Library_Adder();
            lib = new LIB_ADDER_V1();
            lib.getLibrarySystem();
            script = adder.loadScript(script);
            Console.WriteLine("/help for help");

            if (args.Length == 0)
            {
                while (true)
                {
                    try
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
                            c.buildPluginProject(name);
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
                            script.executeScriptFile(dir);
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
