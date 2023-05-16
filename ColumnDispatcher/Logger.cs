using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnDispatcher
{
    internal static class Logger
    {
        public static void Log(string component, string message)
        {
            var time = DateTime.Now.ToString("O");
            var tid = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{time}\t {tid}\t {component}\t {message}");
        }
    }
}
