using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;

namespace PADscript
{
    /// <summary>
    /// 3rd party plugin system to add dynamically C# dll libraries into the scripting engine
    /// </summary>
    public class Library_Adder
    {
        private readonly List<object> assm = new List<object>();
        private readonly string[,] cmd_assm = new string[1000, 1000];

        private readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PADScript\\plugins\\Library\\";
        private readonly string dir_libs = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PADScript\\plugins";

        private Script _p = new Script();
        private readonly IJsEngine engine;

        //private Script _p = new Script();
        private readonly IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;

        /// <summary>
        /// constructor to setup folders n stuff
        /// </summary>
        public Library_Adder()
        {
            if (!Directory.Exists(dir_libs + "\\Library"))
                Directory.CreateDirectory(dir_libs + "\\Library");

            if (!Directory.Exists(dir_libs + "\\Library\\commands"))
                Directory.CreateDirectory(dir_libs + "\\Library\\commands");

            if (!Directory.Exists(dir_libs + "\\Library\\lib"))
                Directory.CreateDirectory(dir_libs + "\\Library\\lib");

            var _lib_files = Directory.GetFiles(dir_libs + "\\Library\\lib");
            engineSwitcher.EngineFactories.Add(new V8JsEngineFactory());
            engine = JsEngineSwitcher.Current.CreateEngine(V8JsEngine.EngineName);
            foreach (var _file in _lib_files)
            {
                //if (!_file.Trim().Contains(".pdb") && !_file.Trim().Contains(".xml"))
                //{
                //    var type = Assembly.LoadFrom(_file);
                //    AssemblyName _name = type.GetName();
                //    string _namespace = _name.Name;
                //    var obj1 = type.CreateInstance(_namespace);

                //    var names = (from typess in type.GetTypes()
                //                 from method in typess.GetMethods(
                //                   BindingFlags.Public | BindingFlags.NonPublic |
                //                   BindingFlags.Instance | BindingFlags.Static)
                //                 select type.FullName + ":" + method.Name).Distinct().ToList();

                //    Console.WriteLine();
                //}
            }
        }

        /// <summary>
        ///     loads all libraries from .dll files
        /// </summary>
        public void LoadAllLibraries()
        {
            try
            {
                string[] namespaces = null;
                string[] lib_names = null;
#pragma warning disable CS0219 // The variable 'commands' is assigned but its value is never used
                string[] commands = null;
#pragma warning restore CS0219 // The variable 'commands' is assigned but its value is never used

                var nuller = new string[1];
                nuller[0] = "";
                if (!File.Exists(dir + "namespace_list.txt"))
                    File.WriteAllLines(dir + "namespace_list.txt", nuller);

                if (!File.Exists(dir + "lib_list.txt"))
                    File.WriteAllLines(dir + "lib_list.txt", nuller);

                if (!File.Exists(dir + "commands"))


                    namespaces = File.ReadAllLines(dir + "namespace_list.txt");
                lib_names = File.ReadAllLines(dir + "lib_list.txt");


                var x = 0;
                var lib_num = 0;
                string[] cmds = null;

                foreach (var _ns in namespaces)
                {
                    if (_ns != "" && _ns != null)
                    {
                        if (lib_names[x] != "" && lib_names[x] != null)
                        {
                            lib_num = addLibraryToList(lib_names[x], _ns);
                            var _namespace = _ns.Split('.')[1];
                            addJavaScriptTypeToList(lib_num, _namespace);
                        }
                    }
                }

                foreach (var _ns in namespaces)
                    if (_ns != "" && _ns != null)
                    {
                        if (lib_names[x] != "" && lib_names[x] != null)
                        {
                            lib_num = addLibraryToList(lib_names[x], _ns);
                            var _nameSpace = _ns.Split('.')[0];
                            cmds = File.ReadAllLines(dir + "commands\\" + _nameSpace + ".txt");
                            foreach (var com in cmds)
                                addCommandLibraryToList(com, lib_num);
                            addInstanceInScript(lib_num);
                        }

                        x++;
                    }
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
            }
        }

        public void addJavaScriptTypeToList(int lib_num, string namespace_)
        {
            var _obj = assm[lib_num];
            engine.EmbedHostType(namespace_, _obj.GetType());
        }

        /// <summary>
        ///     Add a library that is a .dll file
        /// </summary>
        /// <param name="lib">lib name</param>
        /// <param name="_namespace">the namespace of the lib file</param>
        /// <returns>the index of the list</returns>
        public int addLibraryToList(string lib, string _namespace)
        {
            try
            {
                if (File.Exists(dir + "\\lib\\" + lib + ".dll"))
                {
                    var type = Assembly.LoadFrom(dir + "\\lib\\" + lib + ".dll");
                    var obj1 = type.CreateInstance(_namespace);
                    //object obj2 = type.CreateInstance(_namespace + ".Class1");


                    assm.Add(obj1);
                    return assm.Count - 1;
                }

                Console.WriteLine(dir + lib + ".dll: file does not exist");
                return -1;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                return -1;
            }
        }

        /// <summary>
        ///     add the commands found in the command file
        /// </summary>
        /// <param name="cmd">the command name string</param>
        /// <param name="obj_index">the object index</param>
        /// <returns>the index of the command</returns>
        public int addCommandLibraryToList(string cmd, int obj_index)
        {
            try
            {
                var count = 0;
                var x = 1;
                while (true)
                {
                    count = cmd_assm.GetLength(1) - x;
                    var obj = cmd_assm[count, obj_index];
                    if (obj == null)
                        break;
                    x++;
                }

                cmd_assm[count, obj_index] = cmd;
                return count;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                return -1;
            }
        }

        /// <summary>
        /// add the library functions into the scripting engine
        /// </summary>
        /// <param name="obj_index"></param>
        /// <returns></returns>
        public Script addInstanceInScript(int obj_index)
        {
            try
            {
                var _obj = assm[obj_index];
                var cmds = new string[1000];
                for (var u = 0; u < cmd_assm.GetLength(1); u++)
                    cmds[u] = cmd_assm[u, obj_index];


                foreach (var a in cmds)
                    if (a != "" && a != null)
                        _p.addFunc(a, _obj, false);

                return _p;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                return null;
            }
        }

        public void addLibraryToLists(string name, string _ns, string[] cmds)
        {
            try
            {
                string[] a_name = { name };
                string[] a_ns = { _ns };
                File.AppendAllLines(dir + "namespace_list.txt", a_ns);
                File.AppendAllLines(dir + "lib_list.txt", a_name);
                if (File.Exists(dir + "commands\\" + name + ".txt"))
                    File.AppendAllLines(dir + "commands\\" + name + ".txt", cmds);
                else
                    File.WriteAllLines(dir + "commands\\" + name + ".txt", cmds);
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
            }
        }

        public void removeLibraryFromLists(string name, string _namespace)
        {
            try
            {
                var _ns = File.ReadAllLines(dir + "namespace_list.txt");
                var _lib = File.ReadAllLines(dir + "lib_list.txt");

                var new_ns = new string[10000];
                var new_lib = new string[10000];

                var x = 0;
                foreach (var single_ns in _ns)
                    if (single_ns != _namespace)
                    {
                        new_ns[x] = single_ns;
                        x++;
                    }

                x = 0;
                foreach (var _name in _lib)
                    if (_name != name)
                    {
                        new_lib[x] = _name;
                        x++;
                    }
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
            }
        }

        /// <summary>
        /// load the functions and 3rd party libraries into the scripting engine
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Script loadScript(Script l)
        {
            l.addFunc("LoadAllLibraries", this, false);
            l.addFunc("addLibraryToList", this, false);
            l.addFunc("addCommandLibraryToList", this, false);
            l.addFunc("addInstanceInScript", this, false);
            l.addFunc("addLibraryToLists", this, false);
            l.addFunc("removeLibraryFromLists", this, false);

            _p = l;

            LoadAllLibraries();

            return _p;
        }

        public IJsEngine getLibraryJSengine()
        {
            LoadAllLibraries();
            return engine;
        }
    }
}
