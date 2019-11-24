using System;
using NLua;
using KeraLua;

namespace PADscript
{
    public class Program
    {

        static void Main(string[] args)
        {
            Script script;
            Library_Adder adder;
            LIB_ADDER_V1 lib;

            Console.WriteLine("PADscript v1.0");
            script = new Script();
            adder = new Library_Adder();
            lib = new LIB_ADDER_V1();
            lib.getLibrarySystem();
            script = adder.loadScript(script);


            if (args.Length == 0)
            {
                while (true)
                {
                    try
                    {
                        Console.Write(">");
                        string info = Console.ReadLine();
                        script.executeScriptLine(info);

                    }
                    catch (Exception p)
                    {
                        Console.WriteLine(p);
                    }
                }
            }
            else
            {
                if(args[0] == "-f" || args[0] == "--file")
                {
                    try
                    {
                        if(args[1] == null)
                        {
                            Console.WriteLine("no file supplied!");
                        }
                        else
                        {
                            string dir = Environment.CurrentDirectory + "\\" + args[1];
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
