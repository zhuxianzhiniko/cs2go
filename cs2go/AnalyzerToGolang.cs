using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace cs2go.tools
{
    public class AnalyzerToGolang
    {
        private ClassDeclarationSyntax classDeclarationSyntax;

        private StringBuilder main = new StringBuilder();
        private StringBuilder structInfo = new StringBuilder();

        public const string STATICKEY = "static";
        public const string READONLYKEY = "readonly";
        public const string CONST = "const";
        public const string VOID = "void";
        public const string COUNT = "Count";
        public const string LENGHT = "Length";
        public const string TH = "th";
        public const string VAR = "var";

        public const string DICTIONARY = "Dictionary";
        public const string LIST = "List";


        //API
        public const string REMOVEAT = "RemoveAt";
        public const string ADD = "Add";
        public const string CLEAR = "Clear";

        public void AnalyzerStart(string code)
        {
            var roslynTree = CSharpSyntaxTree.ParseText(code);

            var syntax = (CompilationUnitSyntax) roslynTree.GetRoot();


            classDeclarationSyntax = (ClassDeclarationSyntax) syntax.Members[0];
            main.AppendLine("package main");
            structInfo.AppendLine($"type {classDeclarationSyntax.Identifier.ToString()} struct " + "{");

            for (int i = 0; i < classDeclarationSyntax.Members.Count; i++)
            {
                var item = classDeclarationSyntax.Members[i];

                if (item is FieldDeclarationSyntax)
                {
                    AnalyzerField((FieldDeclarationSyntax) item);
                }
                else if (item is MethodDeclarationSyntax)
                {
                    AnalyzerMethod((MethodDeclarationSyntax) item);
                }
                else if (item is EnumDeclarationSyntax)
                {
                    AnalyzerEnum((EnumDeclarationSyntax) item);
                }
            }

            structInfo.AppendLine("}");
            main.AppendLine(structInfo.ToString());
            Console.WriteLine(main);

            var path = Environment.CurrentDirectory + $"\\{classDeclarationSyntax.Identifier.ToString()}.go";
            SaveGoFile(main.ToString(), path);
        }

        private void SaveGoFile(string str, string path)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            File.WriteAllBytes(path, bytes);
            var info = new ProcessStartInfo();
            info.FileName = "gofmt.exe";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.ErrorDialog = true;
            info.Arguments = "-w " + Environment.CurrentDirectory;
            Process.Start(info).WaitForExit();
        }

        /// <summary>
        /// 通过函数名获得是否是静态
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool GetMethodStatic(string methodName)
        {
            for (int i = 0; i < classDeclarationSyntax.Members.Count; i++)
            {
                var item = classDeclarationSyntax.Members[i];

                if (item is MethodDeclarationSyntax)
                {
                    if (((MethodDeclarationSyntax) item).Identifier.Text == methodName)
                    {
                        return GetStatic(((MethodDeclarationSyntax) item).Modifiers);
                    }
                }
            }
            return true;
        }

        private bool GetFieldStatic(string fieldName)
        {
            for (int i = 0; i < classDeclarationSyntax.Members.Count; i++)
            {
                var item = classDeclarationSyntax.Members[i];

                if (item is FieldDeclarationSyntax)
                {
                    if (GetIdentifier((item as FieldDeclarationSyntax).Declaration) == fieldName)
                    {
                        return GetStatic(((FieldDeclarationSyntax) item).Modifiers);
                    }
                }
            }

            return true;
        }

        private bool GetFieldExists(string fieldName)
        {
            for (int i = 0; i < classDeclarationSyntax.Members.Count; i++)
            {
                var item = classDeclarationSyntax.Members[i];

                if (item is FieldDeclarationSyntax)
                {
                    if (GetIdentifier((item as FieldDeclarationSyntax).Declaration) == fieldName)
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 获得是否是静态类
        /// </summary>
        /// <param name="syntaxTokenList"></param>
        /// <returns></returns>
        private bool GetStatic(SyntaxTokenList syntaxTokenList)
        {
            foreach (var item in syntaxTokenList)
            {
                if (item.Text == STATICKEY || item.Text == READONLYKEY || item.Text == CONST)
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzerEnum(EnumDeclarationSyntax enumDeclarationSyntax)
        {
            main.AppendLine($"type {enumDeclarationSyntax.Identifier.Text} int");

            main.AppendLine($"const (");

            for (int i = 0; i < enumDeclarationSyntax.Members.Count; i++)
            {
                var member = enumDeclarationSyntax.Members[i];

                if (i == 0)
                {
                    main.AppendLine($"{member.Identifier} {enumDeclarationSyntax.Identifier.Text} = iota");
                }
                else
                {
                    main.AppendLine($"{member.Identifier}");
                }
            }

            main.AppendLine($")");
        }

        /// <summary>
        /// 函数解析
        /// </summary>
        /// <param name="methodDeclarationSyntax"></param>
        private void AnalyzerMethod(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var flg = GetStatic(methodDeclarationSyntax.Modifiers);

            var returnType = CharpTypeToGolangType(methodDeclarationSyntax.ReturnType);
            if (returnType == VOID)
                returnType = String.Empty;
            if (flg)
            {
                main.AppendLine(
                    $"func {methodDeclarationSyntax.Identifier.Text} ({AnalyzerParameterList(methodDeclarationSyntax.ParameterList)}) {returnType}" +
                    "{");
            }
            else
            {
                main.AppendLine(
                    $"func ({TH} *{classDeclarationSyntax.Identifier.Text}) {methodDeclarationSyntax.Identifier.Text} ({AnalyzerParameterList(methodDeclarationSyntax.ParameterList)}) {returnType}" +
                    "{");
            }

            main.AppendLine(GetStatements(methodDeclarationSyntax.Body.Statements));
            main.AppendLine("}");
        }


        /// <summary>
        /// for 循环
        /// </summary>
        /// <returns></returns>
        private string AnalyzerForStatement(ForStatementSyntax forStatementSyntax)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                $"{forStatementSyntax.ForKeyword} {GetIdentifier(forStatementSyntax.Declaration)} :{GetInitializer(forStatementSyntax.Declaration)}; {forStatementSyntax.Condition}; {forStatementSyntax.Incrementors}" +
                "{");

            BlockSyntax blockSyntax = (BlockSyntax) forStatementSyntax.Statement;
            var sd = GetStatements(blockSyntax.Statements);
            stringBuilder.AppendLine(sd);

            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }

        private string AnalyzerSwitchStatement(SwitchStatementSyntax switchStatementSyntax)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                $"{switchStatementSyntax.SwitchKeyword} {AnalyzerExpression(switchStatementSyntax.Expression)}" + "{");

            foreach (var sectionSyntax in switchStatementSyntax.Sections)
            {
                stringBuilder.AppendLine(sectionSyntax.Labels.ToString());

                stringBuilder.AppendLine(GetStatements(sectionSyntax.Statements));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// {}函数体集合
        /// </summary>
        /// <param name="syntaxList"></param>
        /// <returns></returns>
        private string GetStatements(SyntaxList<StatementSyntax> syntaxList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in syntaxList)
            {
                if (item is ReturnStatementSyntax)
                {
                    stringBuilder.AppendLine(AnalyzerReturnStatement((ReturnStatementSyntax) item));
                }
                else if (item is LocalDeclarationStatementSyntax)
                {
                    stringBuilder.AppendLine(AnalyzerLocalDeclarationStatement((LocalDeclarationStatementSyntax) item));
                }
                else if (item is ExpressionStatementSyntax)
                {
                    stringBuilder.AppendLine(AnalyzerExpressionStatement((ExpressionStatementSyntax) item));
                }
                else if (item is ForStatementSyntax)
                {
                    stringBuilder.AppendLine(AnalyzerForStatement((ForStatementSyntax) item));
                }
                else if (item is SwitchStatementSyntax)
                {
                    stringBuilder.AppendLine(AnalyzerSwitchStatement((SwitchStatementSyntax) item));
                }
                else if (item is BreakStatementSyntax)
                {
                    stringBuilder.AppendLine(item.ToString().Replace(";", ""));
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 函数内 语法
        /// </summary>
        /// <param name="expressionStatementSyntax"></param>
        /// <returns></returns>
        private string AnalyzerExpressionStatement(ExpressionStatementSyntax expressionStatementSyntax)
        {
            return AnalyzerExpression(expressionStatementSyntax.Expression);
        }

        /// <summary>
        /// 局部变量 初始化
        /// </summary>
        /// <param name="localDeclarationStatementSyntax"></param>
        /// <returns></returns>
        private string AnalyzerLocalDeclarationStatement(
            LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string type = String.Empty;
            stringBuilder.Append($"{VAR} ");
            if (localDeclarationStatementSyntax.Declaration.Type.ToString() != VAR)
            {
                type = CharpTypeToGolangType(localDeclarationStatementSyntax.Declaration.Type);
            }


            foreach (var variable in localDeclarationStatementSyntax.Declaration.Variables)
            {
                stringBuilder.Append(
                    $"{variable.Identifier.ToString()} {type} {variable.Initializer.EqualsToken.ToString()} {AnalyzerExpression(variable.Initializer.Value)}");
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// reutrn 表达式
        /// </summary>
        /// <returns></returns>
        private string AnalyzerReturnStatement(ReturnStatementSyntax returnStatementSyntax)
        {
            return
                $"{returnStatementSyntax.ReturnKeyword.ToString()} {AnalyzerExpression(returnStatementSyntax.Expression)}";
        }


        /// <summary>
        /// API 转译
        /// </summary>
        /// <param name="memberAccessExpressionSyntax"></param>
        /// <returns></returns>
        private string GetMemberAccessExpression(MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            if (memberAccessExpressionSyntax.Name.ToString() == COUNT)
            {
                return "cap(" + memberAccessExpressionSyntax.Expression + ")";
            }

            if (memberAccessExpressionSyntax.Name.ToString() == LENGHT)
            {
                return "cap(" + memberAccessExpressionSyntax.Expression + ")";
            }

            if (memberAccessExpressionSyntax.Name.ToString() == REMOVEAT)
            {
                return $"append({memberAccessExpressionSyntax.Expression}[:i], a[i+1:]...)";
            }

            return memberAccessExpressionSyntax.ToString();
        }

        /// <summary>
        /// 静态，动态表达式判断
        /// </summary>
        public string GetElementAccessExpression(ElementAccessExpressionSyntax elementAccessExpressionSyntax)
        {
            if (elementAccessExpressionSyntax.Expression is IdentifierNameSyntax)
            {
                if (GetFieldExists(elementAccessExpressionSyntax.Expression.ToString()))
                {
                    if (!GetFieldStatic(elementAccessExpressionSyntax.Expression.ToString()))
                    {
                        return TH + "." + elementAccessExpressionSyntax;
                    }
                }
            }

            return elementAccessExpressionSyntax.ToString();
        }

        /// <summary>
        ///  表达式解析
        /// </summary>
        /// <param name="expressionSyntax"></param>
        /// <returns></returns>
        private string AnalyzerExpression(ExpressionSyntax expressionSyntax)
        {
            if (expressionSyntax is LiteralExpressionSyntax)
            {
                return expressionSyntax.ToString().Replace("f", "");
            }

            if (expressionSyntax is MemberAccessExpressionSyntax)
            {
                return GetMemberAccessExpression((MemberAccessExpressionSyntax) expressionSyntax);
            }

            if (expressionSyntax is IdentifierNameSyntax)
            {
                return "*" + expressionSyntax;
            }

            if (expressionSyntax is ElementAccessExpressionSyntax)
            {
                return GetElementAccessExpression(expressionSyntax as ElementAccessExpressionSyntax);
            }

            if (expressionSyntax is ArrayCreationExpressionSyntax)
            {
                var ex = (ArrayCreationExpressionSyntax) expressionSyntax;
                return CharpTypeToGolangType(ex.Type) + ex.Initializer;
            }

            if (expressionSyntax is PostfixUnaryExpressionSyntax)
            {
                return expressionSyntax.ToString();
            }

            if (expressionSyntax is AssignmentExpressionSyntax)
            {
                AssignmentExpressionSyntax assignmentExpressionSyntax = expressionSyntax as AssignmentExpressionSyntax;

                string str = String.Empty;
                if (assignmentExpressionSyntax.Left is ElementAccessExpressionSyntax)
                {
                    str += GetElementAccessExpression(assignmentExpressionSyntax.Left as ElementAccessExpressionSyntax);
                }
                else
                {
                    str += assignmentExpressionSyntax.Left.ToString();
                }

                str += " = ";

                if (assignmentExpressionSyntax.Right is ElementAccessExpressionSyntax)
                {
                    str += GetElementAccessExpression(
                        assignmentExpressionSyntax.Right as ElementAccessExpressionSyntax);
                }
                else
                {
                    str += assignmentExpressionSyntax.Right.ToString();
                }

                return str;
            }

            if (expressionSyntax is InvocationExpressionSyntax)
            {
                InvocationExpressionSyntax syntaxNode = (InvocationExpressionSyntax) expressionSyntax;

                if (syntaxNode.Expression is IdentifierNameSyntax)
                {
                    bool isStatic = GetMethodStatic(syntaxNode.Expression.ToString());
                    if (isStatic)
                    {
                        return expressionSyntax.ToString();
                    }

                    return $"{TH}." + expressionSyntax;
                }

                if (syntaxNode.Expression is MemberAccessExpressionSyntax)
                {
                    MemberAccessExpressionSyntax ex = (MemberAccessExpressionSyntax) syntaxNode.Expression;
                    if (ex.Name.ToString() == REMOVEAT)
                    {
                        var str = syntaxNode.ArgumentList.ToString().Replace("(", "").Replace(")", "");
                        return $"{ex.Expression} = append({ex.Expression}[:{str}], {ex.Expression}[{str}+1:]...)";
                    }

                    if (ex.Name.ToString() == ADD)
                    {
                        var str = syntaxNode.ArgumentList.ToString().Replace("(", "").Replace(")", "");
                        return $"{ex.Expression} = append({ex.Expression}, {str})";
                    }

                    if (ex.Name.ToString() == CLEAR)
                    {
                        return $"{ex.Expression} = {ex.Expression}[:0:0]";
                    }
                    else
                    {
                        return syntaxNode.ToString();
                    }
                }
                else
                {
                    return syntaxNode.ToString();
                }
            }

            if (expressionSyntax is ObjectCreationExpressionSyntax)
            {
                ObjectCreationExpressionSyntax syntaxNode = (ObjectCreationExpressionSyntax) expressionSyntax;

                if (syntaxNode.Type is GenericNameSyntax)
                {
                    return CharpTypeToGolangType(syntaxNode.Type) + syntaxNode.Initializer;
                }
                else
                {
                    return $"{syntaxNode.NewKeyword.ToString()}({syntaxNode.Type.ToString()})";
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        /// <param name="parameterListSyntax"></param>
        /// <returns></returns>
        private string AnalyzerParameterList(ParameterListSyntax parameterListSyntax)
        {
            StringBuilder parameters = new StringBuilder();
            foreach (var parameter in parameterListSyntax.Parameters)
            {
                parameters.Append($"{parameter.Identifier.ToString()} {CharpTypeToGolangType(parameter.Type)},");
            }

            if (parameters.Length > 0)
                parameters.Remove(parameters.Length - 1, 1);
            return parameters.ToString();
        }

        /// <summary>
        /// 解析类成员
        /// </summary>
        /// <param name="fieldDeclarationSyntax"></param>
        private void AnalyzerField(FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            var flg = GetStatic(fieldDeclarationSyntax.Modifiers);
            if (flg)
            {
                main.AppendLine(
                    $"{CONST} {GetIdentifier(fieldDeclarationSyntax.Declaration)} {GetInitializer(fieldDeclarationSyntax.Declaration)}");
            }
            else
            {
                structInfo.AppendLine(
                    $"{GetIdentifier(fieldDeclarationSyntax.Declaration)} {CharpTypeToGolangType(fieldDeclarationSyntax.Declaration.Type)}");
            }
        }


        /// <summary>
        /// 变量 变量名
        /// </summary>
        /// <returns></returns>
        private string GetIdentifier(VariableDeclarationSyntax variableDeclaration)
        {
            foreach (var value in variableDeclaration.Variables)
            {
                return value.Identifier.ToString();
            }

            return String.Empty;
        }

        /// <summary>
        /// 变量 表达式的值
        /// </summary>
        private string GetInitializer(VariableDeclarationSyntax variableDeclaration)
        {
            foreach (var value in variableDeclaration.Variables)
            {
                return value.Initializer.ToString();
            }

            return String.Empty;
        }

        /// <summary>
        ///  变量类型的转换
        /// </summary>
        /// <param name="typeSyntax"></param>
        /// <returns></returns>
        private string CharpTypeToGolangType(TypeSyntax typeSyntax)
        {
            if (typeSyntax is PredefinedTypeSyntax)
            {
                var type = typeSyntax.ToString();
                if (type == "ushort")
                {
                    return "uint16";
                }

                if (type == "short")
                {
                    return "int16";
                }

                if (type == "ulong")
                {
                    return "uint64";
                }

                if (type == "long")
                {
                    return "int64";
                }

                if (type == "float")
                {
                    return "float32";
                }

                if (type == "double")
                {
                    return "float64";
                }

                return typeSyntax.ToString();
            }

            if (typeSyntax is IdentifierNameSyntax)
            {
                return typeSyntax.ToString();
            }

            if (typeSyntax is GenericNameSyntax)
            {
                var genericSyntax = typeSyntax as GenericNameSyntax;

                if (genericSyntax.Identifier.Text == DICTIONARY)
                {
                    var key = genericSyntax.TypeArgumentList.Arguments[0];
                    var value = genericSyntax.TypeArgumentList.Arguments[1];
                    return $"map[{key}]{value}";
                }

                if (genericSyntax.Identifier.Text.IndexOf(LIST, StringComparison.Ordinal) != -1)
                {
                    var typeArgumen = genericSyntax.TypeArgumentList.ToString();
                    return "[]" + typeArgumen.Replace("<", "").Replace(">", "");
                }
            }

            if (typeSyntax is ArrayTypeSyntax)
            {
                return "[]" + ((ArrayTypeSyntax) typeSyntax).ElementType;
            }

            return typeSyntax.ToString();
        }
    }
}