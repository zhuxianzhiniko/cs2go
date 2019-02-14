using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using cs2go.tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace cs2go
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            
            await Start();
        }

        private static async Task Start()
        {
            List<string> list = new List<string>(2)
                {@"E:\cs2go\cs2go\TestClass.cs", @"E:\cs2go\cs2go\TestInterface.cs"};
            
            List<string> classNameList = new List<string>();

            Dictionary<string,CompilationUnitSyntax> dictionary = new Dictionary<string, CompilationUnitSyntax>();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var path = list[i];
                    var compilationUnitSyntax = await Task.Run(() => GetCompilationUnitSyntax(path));

                    if (compilationUnitSyntax != null)
                    {
                        dictionary.Add(path, compilationUnitSyntax);
                        var classNames = GetClassName(compilationUnitSyntax.Members);
                        if(classNames!=null && classNames.Count>0)
                        classNameList.AddRange(classNames);
                    }
                    
                }

                foreach (var item in dictionary)
                {
                    AnalyzerToGolang analyzerToGolang = new AnalyzerToGolang();
                     
                    var fileStr = await Task.Run(() =>  analyzerToGolang.AnalyzerStart(item.Value,classNameList));
                    var bytes = Encoding.UTF8.GetBytes(fileStr);
                    var path = Environment.CurrentDirectory + $"\\{Path.GetFileNameWithoutExtension(item.Key)}.go";
                    File.WriteAllBytes(path, bytes);
                }
          
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            RunGofmt();
            Console.WriteLine("解析完成");
        }

        private static CompilationUnitSyntax GetCompilationUnitSyntax(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileInput = File.ReadAllText(filePath);
     
                var roslynTree = CSharpSyntaxTree.ParseText(fileInput);
                var syntax = (CompilationUnitSyntax) roslynTree.GetRoot();
                return syntax;
            }
            return null;
        }
        private static void RunGofmt()
        {
            var info = new ProcessStartInfo();
            info.FileName = "gofmt.exe";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.ErrorDialog = true;
            info.Arguments = "-w " + Environment.CurrentDirectory;
            Process.Start(info)?.WaitForExit();
        }
        private static List<string> GetClassName(SyntaxList<MemberDeclarationSyntax> memberDeclarationSyntaxs)
        {
            List<string> list = new List<string>();
            foreach (var member in memberDeclarationSyntaxs)
            {
                if (member is NamespaceDeclarationSyntax)
                {
                    list.AddRange(GetClassName(((NamespaceDeclarationSyntax) member).Members));
                }
                if (member is ClassDeclarationSyntax)
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax) member;
                    list.Add(classDeclarationSyntax.Identifier.ToString());
                }
            }
            return list;
        }
    }
}