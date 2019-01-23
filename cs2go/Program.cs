using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace cs2go
{
    internal class Program
    {
        private static void Main(string[] args)
        {
           
            var fileInput = File.ReadAllText(@"E:\cs2go\cs2go\TestClass.cs");
            
            var stream = CharStreams.fromstring(fileInput);
            ITokenSource lexer = new CSharpLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.compilationUnit();
            MyEvent printer = new MyEvent();
            ParseTreeWalker.Default.Walk(printer, tree);
  
            
            var roslynTree = CSharpSyntaxTree.ParseText(fileInput);

           /*var root = (CompilationUnitSyntax)tree.GetRoot();
            var modelCollector = new ModelCollector();
            modelCollector.Visit(root)*/
            Console.WriteLine();
         
        }
    }
}