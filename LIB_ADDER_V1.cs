using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PADscript
{
    public class LIB_ADDER_V1
    {
        private string dir_libs = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PADScript\\plugins";

        public LIB_ADDER_V1()
        {
            try
            {
                if (!Directory.Exists(dir_libs))
                {
                    Directory.CreateDirectory(dir_libs);
                }

                if (!Directory.Exists(dir_libs + "\\library"))
                {
                    Directory.CreateDirectory(dir_libs + "\\library");
                    Directory.CreateDirectory(dir_libs + "\\library\\lib");
                    Directory.CreateDirectory(dir_libs + "\\library\\commands");
                }

                if (!File.Exists(dir_libs + "\\library\\lib_list.txt"))
                {
                    string[] nuller = { };
                    File.WriteAllLines(dir_libs + "\\library\\lib_list.txt", nuller);
                }

                if (!File.Exists(dir_libs + "\\library\\namespace_list.txt"))
                {
                    string[] nuller = { };
                    File.WriteAllLines(dir_libs + "\\library\\namespace_list.txt", nuller);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void getLibrarySystem()
        {
            File.Delete(dir_libs + "\\library\\lib_list.txt");
            File.Delete(dir_libs + "\\library\\namespace_list.txt");
            string[] nuller = { };
            File.WriteAllLines(dir_libs + "\\library\\lib_list.txt", nuller);
            File.WriteAllLines(dir_libs + "\\library\\namespace_list.txt", nuller);
            IEnumerable<string> files = Directory.GetFiles(dir_libs + "\\library\\lib");

            foreach (var _file in files)
                if (!_file.Trim().Contains("Newtonsoft.Json") && !_file.Trim().ToLower().Contains(".json") && _file.Trim().ToLower().Contains(".dll"))
                {
                    var DLL = Assembly.LoadFile(_file);
                    var name = DLL.GetName();
                    var dllName = name.Name;


                    foreach (var type in DLL.GetExportedTypes())
                    {
                        var c = Activator.CreateInstance(type);
                        //type.InvokeMember("Output", BindingFlags.InvokeMethod, null, c, new object[] {@"Hello"});
                        var info = type.GetMethods();

                        if (File.Exists(dir_libs + "\\library\\commands\\" + dllName + ".txt"))
                            File.Delete(dir_libs + "\\library\\commands\\" + dllName + ".txt");
                        var commands = new List<string>();
                        foreach (var _meth in info)
                        {
                            string _m = _meth.Name.ToLower();
                            if (!(_m == "tostring" || _m == "equals" || _m == "gethashcode" || _m == "gettype"))
                                commands.Add(_meth.Name);
                        }

                        File.WriteAllLines(dir_libs + "\\library\\commands\\" + dllName + ".txt",
                            commands.ToArray());
                        File.AppendAllText(dir_libs + "\\library\\lib_list.txt", dllName + "\n");
                        var fullname = type.FullName;
                        File.AppendAllText(dir_libs + "\\library\\namespace_list.txt",
                            type.FullName + "\n");
                    }
                }
        }
    }
}
