
using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PADscript
{
    /// <summary>
    /// the holy grail, the scripting engine that allows custom functions from C# to be integrated into lua to create PADscript
    /// </summary>
    public class Script
    {
        private readonly List<string> _funcs = new List<string>();
        private readonly List<string> _funcsWithArgs = new List<string>();
        private readonly Lua _l;
        private List<object> _class = new List<object>();

        /// <summary>
        /// constructor
        /// </summary>
        public Script()

        {
            _l = new Lua();
            addFunc("PrintCommands", this, false);
        }

        /// <summary>
        /// execute a script file
        /// </summary>
        /// <param name="file">the filename</param>
        /// <returns>if successful or not</returns>
        public bool executeScriptFile(string file, string[] files = null)

        {
            try
            {
                foreach (var l in files)
                {
                    _l.LoadFile(l);
                }

                _l.DoFile(file);
                
                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                //Console.WriteLine(i.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// returns bool if function in scripting engine exists or not (not case sensitive).
        /// </summary>
        /// <param name="func">the function name</param>
        /// <returns>true or false</returns>
        public bool getFunctionExists(string func)

        {
            foreach (var _f in _funcs)
                if (func.Trim().ToLower() == _f.Trim().ToLower())
                    return true;
            return false;
        }

        /// <summary>
        /// get the raw function list as a list of strings
        /// </summary>
        /// <returns>list of functions in scripting engine</returns>
        public List<string> getFuncList()

        {
            return _funcs;
        }

        /// <summary>
        /// execute a line of PADscript
        /// </summary>
        /// <param name="line">the line of code</param>
        /// <returns>if successful or not</returns>
        public bool executeScriptLine(string line)

        {
            try
            {
                if (line != null)
                {
                    _l.DoString(line);
                    return true;
                }
                return false;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                //Console.WriteLine(i.StackTrace);
                return false;
            }
        }



        public static Exception ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                var procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + command)
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                // Do not create the black window.
                // Now we create a process, assign its ProcessStartInfo and start it
                var proc = new Process { StartInfo = procStartInfo };
                proc.Start();
                // Get the output into a string
                var result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
                return null;
            }
            catch (Exception objException)
            {
                return objException;
                // Log the exception
            }
        }


        /// <summary>
        /// add a custom C# function to PADscript
        /// </summary>
        /// <param name="func">the method name as a string</param>
        /// <param name="obj">the object that is referenced to the class the method is associated to (not as a string)</param>
        /// <param name="obsolete">set true to become obsolete</param>
        /// <returns>if successful or not</returns>
        public bool addFunc(string func, object obj, bool obsolete)

        {
            if (!obsolete)
                try
                {
                    _funcs.Add(func);
                    _l.RegisterFunction(func, obj, obj.GetType().GetMethod(func));
                    var info = obj.GetType().GetMethod(func);
                    var _params = info.GetParameters();

                    var total = func + "(";
                    foreach (var _p in _params)
                    {
                        var _type = _p.ParameterType;
                        var _name = _p.Name;
                        total = total + _type.Name + " " + _name + ",";
                    }

                    var last = total.LastIndexOf(',');
                    var _total = "";
                    if (total.Contains(","))
                        _total = total.Substring(0, total.Length - 1);
                    _total = _total + ")";
                    if (_total.Length > 1)
                        _funcsWithArgs.Add(_total);
                    return true;
                }
                catch (Exception i)
                {
                    Console.Write(i.Message);
                    Console.WriteLine("_error Script: func: " + func + "_class: " + obj);
                    return false;
                }

            return false;
        }

        /// <summary>
        /// get the Lua object instance
        /// </summary>
        /// <returns>Lua object</returns>
        public Lua getLuaRef()

        {
            return _l;
        }

        /// <summary>
        /// print all the commands to the console
        /// </summary>
        public void PrintCommands()

        {
            var newFuncs = _funcsWithArgs.OrderBy(x => x).ToList();
            var _str = new StringBuilder();
            foreach (var com in newFuncs)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(com);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("-------------------------------------------------------");
                _str.AppendLine(com);
            }

            Console.Write("Would you want these commands to a text file? (Y/N): ");
            var _response = Console.ReadLine();
            if (_response.Trim().ToUpper() == "Y")
                try
                {
                    if (File.Exists(Environment.CurrentDirectory + "\\" + "currentFunctionsText.txt"))
                        File.Delete(Environment.CurrentDirectory + "\\" + "currentFunctionsText.txt");
                    File.WriteAllText(Environment.CurrentDirectory + "\\" + "currentFunctionsText.txt",
                        _str.ToString());
                    Process.Start("notepad.exe", Environment.CurrentDirectory + "\\" + "currentFunctionsText.txt");
                }
                catch (Exception i)
                {
                    Log.RecordError(i);
                }
        }

        /// <summary>
        /// get a list of functions in the scripting engine, with the argument type and name attached to the function
        /// </summary>
        /// <returns>list of functions in the scripting engine with arguments attached</returns>
        public List<string> getFuncWithArgs()

        {
            return _funcsWithArgs;
        }

        /// <summary>
        /// save the list of commands in a file
        /// </summary>
        public void saveCommandList()

        {
            try
            {
                if (File.Exists(Environment.CurrentDirectory + "\\commands.txt"))
                    File.Delete(Environment.CurrentDirectory + "\\commands.txt");

                File.WriteAllLines(Environment.CurrentDirectory + "\\commands.txt", _funcsWithArgs.ToArray());
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                Console.WriteLine(i.StackTrace);
            }
        }

        /// <summary>
        /// check to see if a method is obsolete or not
        /// </summary>
        /// <param name="function">the method name</param>
        /// <returns>if it is obsoloete or not</returns>
        public bool getMethodObsolete(string function)

        {
            foreach (var _func in _funcs)
                if (_func == function)
                    return true;
            return false;
        }
    }
}
