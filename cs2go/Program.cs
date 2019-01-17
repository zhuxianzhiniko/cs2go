using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace cs2go
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /* String input = @"public class TestRole
             {
         public int Attack;
         public int Defense;
 
 
         public int GetHurt(TestRole testRole)
         {
             return Attack - testRole.Defense;
         }
     }";*/

            var input = @"int num = 10;";
           
            var fileInput = File.ReadAllText(@"E:\cs2go\cs2go\TestRole.cs");
            
            var stream = CharStreams.fromstring(fileInput);
            ITokenSource lexer = new CSharpLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.compilationUnit();
            MyEvent printer = new MyEvent();
            ParseTreeWalker.Default.Walk(printer, tree);
         
//           Console.WriteLine("out"+tree.GetText());
  
        }
    }
}