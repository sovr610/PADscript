using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PADscript
{
    public class Log
    {

        private static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                             "\\PADscript";

        public Log()
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }



        /// <summary>
        /// record error log with exception data
        /// </summary>
        /// <param name="i">Exception to record info to a file</param>
        public static void RecordError(Exception i)
        {
            var msg = i.Message;
            var stackTrace = i.StackTrace;


            Exception ii = null;

            if (i.InnerException != null) ii = i.InnerException;

            try
            {
                var args = new List<string>();

                args.Add(msg);
                args.Add(stackTrace);

                if (ii.InnerException != null) args.Add(ii.Message);

                File.WriteAllLines(
                    dir + "\\" + Convert.ToString(DateTime.Now.Month) + "_" + Convert.ToString(DateTime.Now.Day) + "_" +
                    Convert.ToString(DateTime.Now.Hour) + "_" + Convert.ToString(DateTime.Now.Minute) + "_error.txt",
                    args.ToArray());
            }
            catch (Exception k)
            {
                //yeaaaaaa
            }
        }

    }
}
