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
using Newtonsoft.Json;

namespace cs2go
{
    public class Config
    {
        /// <summary>
        /// go 语言的包名
        /// </summary>
        public string DefaultPackageName;

        /// <summary>
        /// 需要转换的文件路径
        /// </summary>
        public string CSharpPath;

        /// <summary>
        /// go 文件的储存路径
        /// </summary>
        public string GoFilePath;
    }

    internal class Program
    {
        public static Config _config;
        private static string _configPath = Environment.CurrentDirectory + "\\Config.json";

        private static async Task Main(string[] args)
        {
            LoadConfig();
            await Start();
            SaveConfig();
        }

        private static void LoadConfig()
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath, Encoding.UTF8);
                _config = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                _config = new Config();
                _config.DefaultPackageName = "main";
                _config.CSharpPath = Environment.CurrentDirectory;
                _config.GoFilePath = Environment.CurrentDirectory;
                SaveConfig();
            }
        }

        private static void SaveConfig()
        {
            if (_config != null)
                using (var fileStream = new FileStream(_configPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_config));
                    fileStream.Write(bytes, 0, bytes.Length);
                }
        }

        private static async Task Start()
        {
            List<string> list = GetAllFile(_config.CSharpPath, new string[1] {"cs"});
            if (list.Count <= 0)
            {
                Console.WriteLine("解析完成,路径没有找到.cs文件");
                return;
            }
            List<string> classNameList = new List<string>();

            Dictionary<string, CompilationUnitSyntax> dictionary = new Dictionary<string, CompilationUnitSyntax>();
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
                        if (classNames != null && classNames.Count > 0)
                            classNameList.AddRange(classNames);
                    }
                }

                foreach (var item in dictionary)
                {
                    AnalyzerToGolang analyzerToGolang = new AnalyzerToGolang();

                    var fileStr = await Task.Run(() => analyzerToGolang.AnalyzerStart(item.Value, classNameList));
                    var bytes = Encoding.UTF8.GetBytes(fileStr);
                    var path = _config.GoFilePath + $"\\{Path.GetFileNameWithoutExtension(item.Key)}.go";
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
        public static List<string> GetAllFile(string sourcePath, string[] fileType)
        {
            List<string> fileList = new List<string>();
            GetDirectory(sourcePath, fileType, ref fileList);
            return fileList;
        }

        private static void GetDirectory(string sourcePath, string[] fileTypes, ref List<string> fileList)
        {
            if (Directory.Exists(sourcePath)) //判断源文件夹是否存在
            {
                string[] tmp = Directory.GetFileSystemEntries(sourcePath); //获取源文件夹中的目录及文件路径，存入字符串
                //循环遍历
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (File.Exists(tmp[i])) //如果是文件则存入FileList
                    {
                        string[] names = tmp[i].Split('.');
                        if (names.Length >= 2)
                        {
                            var fileType = names[names.Length - 1];

                            if (Array.IndexOf(fileTypes, fileType) != -1)
                            {
                                fileList.Add(tmp[i]);
                            }
                        }
                    }

                    //递归开始.......
                    GetDirectory(tmp[i], fileTypes, ref fileList);
                }
            }
        }
    }
}