using System;
using System.IO;
using cs2go.tools;

namespace cs2go
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileInput = File.ReadAllText(@"E:\cs2go\cs2go\TestClass.cs");
            AnalyzerToGolang analyzerToGolang = new AnalyzerToGolang();
            analyzerToGolang.AnalyzerStart(fileInput);
            Console.ReadLine();
        }
    }
}