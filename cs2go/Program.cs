using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using cs2go.tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace cs2go
{
    internal class Program
    {
        private static void Main(string[] args)
        {
           
           var fileInput = File.ReadAllText(@"E:\cs2go\cs2go\TestClass.cs");
            
             /*var stream = CharStreams.fromstring(fileInput);
            ITokenSource lexer = new CSharpLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.compilationUnit();
            MyEvent printer = new MyEvent();
            ParseTreeWalker.Default.Walk(printer, tree);*/
  
             AnalyzerToGolang analyzerToGolang = new AnalyzerToGolang();
             analyzerToGolang.AnalyzerStart(fileInput);
             
           /* var roslynTree = CSharpSyntaxTree.ParseText(fileInput);
            CompilationUnitSyntax syntax = (CompilationUnitSyntax)roslynTree.GetRoot();
        
            var d =  syntax.DescendantNodes();

            foreach (var VARIABLE in d)
            {
                Console.WriteLine(VARIABLE.ToString());
            }
            
            for (int i = 0; i < syntax.Members.Count; i++)
            {
                ClassDeclarationSyntax member = (ClassDeclarationSyntax)syntax.Members[i];

                Console.WriteLine("key:" + member.Keyword);
                
                for (int j = 0; j < member.Members.Count; j++)
                {
                    Console.WriteLine( member.Members[j].GetText().ToString());
                }

            }*/

            Console.ReadLine();
         
        }
        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {

        }
    }
    class ModelCollector : CSharpSyntaxWalker
    {
        public readonly Dictionary<string, List<string>> Models = new Dictionary<string, List<string>>();
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var classnode = node.Parent as ClassDeclarationSyntax;
            if (classnode != null && !Models.ContainsKey(classnode.Identifier.ValueText))
            {
                Models.Add(classnode.Identifier.ValueText, new List<string>());
            }

            Models[classnode.Identifier.ValueText].Add(node.Identifier.ValueText);
        }
        
        
    }

    class TestAnalysis:AnalysisContext
    {
        public override void RegisterCompilationStartAction(Action<CompilationStartAnalysisContext> action)
        {
            throw new NotImplementedException();
        }

        public override void RegisterCompilationAction(Action<CompilationAnalysisContext> action)
        {
            throw new NotImplementedException();
        }

        public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
        {
            throw new NotImplementedException();
        }

        public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
        {
            throw new NotImplementedException();
        }

        public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
        {
            throw new NotImplementedException();
        }

        public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
        {
            throw new NotImplementedException();
        }

        public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
        {
            throw new NotImplementedException();
        }

        public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
        {
            throw new NotImplementedException();
        }
    }
   
}