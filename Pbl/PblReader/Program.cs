using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerDoc;

namespace PblReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Usage();
            }
            else if (args[0] == "/d")
            {
                if (args.Length < 2)
                {
                    Usage();
                }
                else
                {
                    var file = PblFile.OpenPbl(args[1]);
                    file.LoadDirectory();
                    foreach (var entry in file.Entries.Cast<PblEntry>())
                    {
                        Console.WriteLine(entry.Name);
                    }
                }
            }
            else if (args[0] == "/e")
            {
                if (args.Length < 3)
                {
                    Usage();
                }
                else
                {
                    var file = PblFile.OpenPbl(args[1]);
                    file.LoadDirectory();
                    string syntax = file.LoadEntrySyntax(args[2]);
                    Console.WriteLine(syntax);
                }
            }
        }

        private static void Usage()
        {
            Console.WriteLine("\t/d [pbl file]\t\t\tList directory");
            Console.WriteLine("\t/e [pbl file] [entry name]\tList entry content");

        }
    }
}
