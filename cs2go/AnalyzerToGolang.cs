using System;
using System.Collections.Generic;
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

        //关键字
        public const string STATICKEY = "static";
        public const string READONLYKEY = "readonly";
        public const string CONST = "const";
        public const string VOID = "void";
        public const string COUNT = "Count";
        public const string LENGHT = "Length";
        public const string TH = "th";
        public const string VAR = "var";
        public const string BracesS ="{";
        public const string BracesE ="}";
        public const string TYPE = "type";
        public const string STRUCT = "struct";
        public const string FUNC = "func";

        
        public const string  FALSE = "false";

        public const string DICTIONARY = "Dictionary";
        public const string LIST = "List";


        //API
        public const string REMOVEAT = "RemoveAt";
        public const string ADD = "Add";
        public const string CLEAR = "Clear";
        
        
        //类型
        
        public const string INT = "int";
        public const string UINT = "uint";
        
        public const string USHORT = "ushort";
        public const string SHORT = "short";
        
        public const string ULONG = "ulong";
        public const string LONG = "long";
        
        public const string FLOAT = "float";
        public const string DOUBLE = "double";
        
        public const string STRING = "string";

        public const string BOOL = "bool";

        public string DefaultPackageName = "package main";

        private List<string> _classNameList;
    
        /// <summary>
        /// 基元类型字典  key为CSharp类型，value对应go的类型
        /// </summary>
        public Dictionary<string,string> PrimitiveTypes = new Dictionary<string, string>()
        {
            {INT,INT},
            {UINT,"uint32"},

            
            {USHORT,"uint16"},
            {SHORT,"int16"},
            
            {ULONG,"uint64"},
            {LONG,"int64"},
            
            {FLOAT,"float32"},
            {DOUBLE,"float64"},
            
            {STRING,STRING},
            {BOOL,BOOL},
        };


      
        public string AnalyzerStart(CompilationUnitSyntax  syntax,List<string> classNameList)
        {
            _classNameList = classNameList;
            main.AppendLine(DefaultPackageName); 
            AnalyzerMemberDeclaration(syntax.Members);
            main.AppendLine(structInfo.ToString());
            Console.WriteLine(main);
            return main.ToString();
        }
  
        private void AnalyzerMemberDeclaration(SyntaxList<MemberDeclarationSyntax> memberDeclarationSyntaxs)
        {
            foreach (var member in memberDeclarationSyntaxs)
            {
                if (member is NamespaceDeclarationSyntax)
                {
                    AnalyzerMemberDeclaration(((NamespaceDeclarationSyntax) member).Members);
                }
                if (member is InterfaceDeclarationSyntax)
                {
                    AnalyzerInterfaceDeclaration((InterfaceDeclarationSyntax) member);
                }
                if (member is ClassDeclarationSyntax)
                {
                    classDeclarationSyntax = (ClassDeclarationSyntax) member;
                    structInfo.AppendLine($"{TYPE} {classDeclarationSyntax.Identifier.ToString()} {STRUCT} {BracesS}" );

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
                    structInfo.AppendLine(BracesE);
                 
                }
            }
        }

        private void AnalyzerInterfaceDeclaration(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
        {
            main.AppendLine($"{TYPE} {interfaceDeclarationSyntax.Identifier.ToString()} {interfaceDeclarationSyntax.Keyword} {BracesS}");
            foreach (var member in interfaceDeclarationSyntax.Members)
            {
                if (member is MethodDeclarationSyntax)
                {
                    MethodDeclarationSyntax methodDeclarationSyntax = member as MethodDeclarationSyntax;
                    var returnType = methodDeclarationSyntax.ReturnType.ToString();
                    if (returnType == VOID)
                    {
                        returnType = String.Empty;
                    }
                    main.AppendLine(
                        $"{methodDeclarationSyntax.Identifier.Text} ({AnalyzerParameterList(methodDeclarationSyntax.ParameterList)}) {returnType}");
                }
            }
            main.AppendLine(BracesE);
        }

        /// <summary>
        /// 判断是否是enum
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        private bool IsEnum(string enumName)
        {
            for (int i = 0; i < classDeclarationSyntax.Members.Count; i++)
            {
                var item = classDeclarationSyntax.Members[i];
                if (item is EnumDeclarationSyntax)
                {
                    if (((EnumDeclarationSyntax) item).Identifier.Text == enumName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 通过函数名获得是否需要加调用指针 th,如果没找到则不需要添加(默认静态函数)
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool GetMethodPointer(string methodName)
        {
            for (int i = 0; i < classDeclarationSyntax.Members.Count; i++)
            {
                var item = classDeclarationSyntax.Members[i];

                if (item is MethodDeclarationSyntax)
                {
                    if (((MethodDeclarationSyntax) item).Identifier.Text == methodName)
                    {
                        //如果是静态函数/常量，不需要加th调用
                        return !GetStaticOrConst(((MethodDeclarationSyntax) item).Modifiers);
                    }
                }
            }
            return false;
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
                        return GetStaticOrConst(((FieldDeclarationSyntax) item).Modifiers);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获得是否是包含静态或者常量修饰符
        /// </summary>
        /// <param name="syntaxTokenList"></param>
        /// <returns></returns>
        private bool GetStaticOrConst(SyntaxTokenList syntaxTokenList)
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

        /// <summary>
        /// enum转译
        /// </summary>
        /// <param name="enumDeclarationSyntax"></param>
        private void AnalyzerEnum(EnumDeclarationSyntax enumDeclarationSyntax)
        {
            main.AppendLine($"{TYPE} {enumDeclarationSyntax.Identifier.Text} {INT}");

            main.AppendLine($"{CONST} (");

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
            var flg = GetStaticOrConst(methodDeclarationSyntax.Modifiers);

            var returnType = methodDeclarationSyntax.ReturnType.ToString();
            if (returnType == VOID)
            {
                  returnType = String.Empty;
            }
            else
            {
                if (!PrimitiveTypes.TryGetValue(returnType,out _))
                {
                    returnType = "*" + CharpTypeToGolangType(methodDeclarationSyntax.ReturnType);
                }
            }
            
            if (flg)
            {
                main.AppendLine(
                    $"{FUNC} {methodDeclarationSyntax.Identifier.Text} ({AnalyzerParameterList(methodDeclarationSyntax.ParameterList)}) {returnType} {BracesS}");
            }
            else
            {
                main.AppendLine(
                    $"{FUNC} ({TH} *{classDeclarationSyntax.Identifier.Text}) {methodDeclarationSyntax.Identifier.Text} ({AnalyzerParameterList(methodDeclarationSyntax.ParameterList)}) {returnType} {BracesS}");
            }

            main.AppendLine(GetStatements(methodDeclarationSyntax.Body.Statements));
            main.AppendLine(BracesE);
        }


        /// <summary>
        /// for 循环
        /// </summary>
        /// <returns></returns>
        private string AnalyzerForStatement(ForStatementSyntax forStatementSyntax)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                $"{forStatementSyntax.ForKeyword} {GetIdentifier(forStatementSyntax.Declaration)} :{GetInitializer(forStatementSyntax.Declaration)}; {AnalyzerExpression(forStatementSyntax.Condition)}; {forStatementSyntax.Incrementors} {BracesS}");

            BlockSyntax blockSyntax = (BlockSyntax) forStatementSyntax.Statement;
            var sd = GetStatements(blockSyntax.Statements);
            stringBuilder.AppendLine(sd);

            stringBuilder.AppendLine(BracesE);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// switch 
        /// </summary>
        /// <param name="switchStatementSyntax"></param>
        /// <returns></returns>
        private string AnalyzerSwitchStatement(SwitchStatementSyntax switchStatementSyntax)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                $"{switchStatementSyntax.SwitchKeyword} {AnalyzerExpression(switchStatementSyntax.Expression)} {BracesS}");

            foreach (var sectionSyntax in switchStatementSyntax.Sections)
            {
                stringBuilder.AppendLine(sectionSyntax.Labels.ToString());

                stringBuilder.AppendLine(GetStatements(sectionSyntax.Statements));
            }

            stringBuilder.AppendLine(BracesE);

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
                stringBuilder.AppendLine(GetStatement(item));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 单一个Statement
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetStatement(StatementSyntax item)
        {
            if (item is ReturnStatementSyntax)
            {
                return AnalyzerReturnStatement((ReturnStatementSyntax) item);
            }

            if (item is LocalDeclarationStatementSyntax)
            {
                return AnalyzerLocalDeclarationStatement((LocalDeclarationStatementSyntax) item);
            }

            if (item is ExpressionStatementSyntax)
            {
                return AnalyzerExpressionStatement((ExpressionStatementSyntax) item);
            }

            if (item is ForStatementSyntax)
            {
                return AnalyzerForStatement((ForStatementSyntax) item);
            }

            if (item is SwitchStatementSyntax)
            {
                return AnalyzerSwitchStatement((SwitchStatementSyntax) item);
            }

            if (item is BreakStatementSyntax)
            {
                return item.ToString().Replace(";", "");
            }

            if (item is IfStatementSyntax)
            {
                return AnalyzerIfStatement((IfStatementSyntax)item);
            }

            if (item is BlockSyntax)
            {
                return GetStatements(((BlockSyntax)item).Statements);
            }
            return String.Empty;
        }

        /// <summary>
        /// if 解析
        /// </summary>
        /// <param name="ifStatementSyntax"></param>
        /// <returns></returns>
        private string AnalyzerIfStatement(IfStatementSyntax ifStatementSyntax)
        {
           StringBuilder stringBuilder = new StringBuilder();

           stringBuilder.AppendLine($"{ifStatementSyntax.IfKeyword} {AnalyzerExpression(ifStatementSyntax.Condition)} {BracesS}");
           stringBuilder.AppendLine(GetStatement(ifStatementSyntax.Statement));
           stringBuilder.AppendLine(BracesE+(AnalyzerelseClause(ifStatementSyntax.Else)));
           return stringBuilder.ToString();
        }
        /// <summary>
        /// else 解析
        /// </summary>
        /// <param name="elseClauseSyntax"></param>
        /// <returns></returns>
        private string AnalyzerelseClause(ElseClauseSyntax elseClauseSyntax)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (elseClauseSyntax.Statement is BlockSyntax) //因为{问题，在没有下一个else if 的特殊处理
            {
                stringBuilder.AppendLine($"{elseClauseSyntax.ElseKeyword} {BracesS}");
                stringBuilder.AppendLine(GetStatement(elseClauseSyntax.Statement));
            }
            else
            {
                stringBuilder.Append($"{elseClauseSyntax.ElseKeyword}  "); 
                stringBuilder.AppendLine(GetStatement(elseClauseSyntax.Statement));
                stringBuilder.AppendLine(BracesE);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 函数过程 语法
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
        ///  .访问符号 API 转译
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
                var str = (memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax)?.ArgumentList.ToString().Replace("(", "").Replace(")", "");
                return $"{memberAccessExpressionSyntax.Expression} = append({memberAccessExpressionSyntax.Expression}[:{str}], {memberAccessExpressionSyntax.Expression}[{str}+1:]...)";
            }

            if (memberAccessExpressionSyntax.Name.ToString() == ADD)
            {
                var str = (memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax)?.ArgumentList.ToString().Replace("(", "").Replace(")", "");
                return $"{memberAccessExpressionSyntax.Expression} = append({memberAccessExpressionSyntax.Expression}, {str})";
            }

            if (memberAccessExpressionSyntax.Name.ToString() == CLEAR)
            {
                return $"{memberAccessExpressionSyntax.Expression} = {memberAccessExpressionSyntax.Expression}[:0:0]";
            }
            
            //修饰enum
            if (IsEnum(memberAccessExpressionSyntax.Expression.ToString()))
            {
                return memberAccessExpressionSyntax.Name.ToString();
            }
            
            return String.Empty;
        }

        /// <summary>
        /// 静态，动态表达式判断
        /// </summary>
        public string GetElementAccessExpression(ElementAccessExpressionSyntax elementAccessExpressionSyntax)
        {
            if (elementAccessExpressionSyntax.Expression is IdentifierNameSyntax)
            {
                return $"{AnalyzerExpression(elementAccessExpressionSyntax.Expression)}{elementAccessExpressionSyntax.ArgumentList.ToString()}";
            }
            return elementAccessExpressionSyntax.ToString();
        }

        private string  AnalyzerAssignmentExpression(AssignmentExpressionSyntax assignmentExpression)
        {
            string str = String.Empty;
            str += AnalyzerExpression(assignmentExpression.Left);
            str += $" {assignmentExpression.OperatorToken.Text} ";
            str += AnalyzerExpression(assignmentExpression.Right);
            return str;
        }
        
        private string  AnalyzerBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            string str = String.Empty;
            str += AnalyzerExpression(binaryExpression.Left);
            str += $" {binaryExpression.OperatorToken.Text} ";
            str += AnalyzerExpression(binaryExpression.Right);
            return str;
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
                var str = expressionSyntax.ToString();
                if (str == FALSE) 
                    return str;
                return expressionSyntax.ToString().Replace("f", "");//屏蔽掉因为浮点数的字符串
            }
            if (expressionSyntax is BinaryExpressionSyntax)
            {
               return AnalyzerBinaryExpression(expressionSyntax as BinaryExpressionSyntax);
            }
            if (expressionSyntax is AssignmentExpressionSyntax)
            {
                return AnalyzerAssignmentExpression(expressionSyntax as AssignmentExpressionSyntax);
            }
            
            if (expressionSyntax is MemberAccessExpressionSyntax)
            {
                var exp = GetMemberAccessExpression((MemberAccessExpressionSyntax) expressionSyntax);
                if (string.IsNullOrEmpty(exp))
                {
                    return expressionSyntax.ToString();
                }
                return exp;
            }

            if (expressionSyntax is IdentifierNameSyntax)
            {
                if (!GetFieldStatic(expressionSyntax.ToString()))
                {
                    return TH + "." + expressionSyntax;
                }
                return  expressionSyntax.ToString();
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
            
            if (expressionSyntax is InvocationExpressionSyntax)
            {
                InvocationExpressionSyntax syntaxNode = (InvocationExpressionSyntax) expressionSyntax;

                if (syntaxNode.Expression is IdentifierNameSyntax)
                {
                    var identifier =((IdentifierNameSyntax) syntaxNode.Expression).Identifier;
                    bool isAddPointer = GetMethodPointer(syntaxNode.Expression.ToString());
                    if (isAddPointer)
                    {
                        return $"{TH}." + expressionSyntax;
                    }
                    return expressionSyntax.ToString();
                }

                if (syntaxNode.Expression is MemberAccessExpressionSyntax)
                {
                    var expStr = GetMemberAccessExpression(syntaxNode.Expression as MemberAccessExpressionSyntax);
                    if (string.IsNullOrEmpty(expStr))
                    {
                        IdentifierNameSyntax identifier =(IdentifierNameSyntax)((MemberAccessExpressionSyntax)syntaxNode.Expression).Expression;

                        if (_classNameList.IndexOf(identifier.ToString()) != -1)
                        {
                            return syntaxNode.ToString().Replace($"{identifier.ToString()}.","");
                        }
                        return syntaxNode.ToString();
                    }
                    return expStr;
                }
                return syntaxNode.ToString();
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
            var flg = GetStaticOrConst(fieldDeclarationSyntax.Modifiers);
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
                string gotype;
                if (PrimitiveTypes.TryGetValue(typeSyntax.ToString(),out gotype))
                {
                    return gotype;
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