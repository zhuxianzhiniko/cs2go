using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public class MyEvent : CSharpParserBaseListener
{
    private readonly StringBuilder goStr = new StringBuilder();
    private readonly StringBuilder member = new StringBuilder();
    private string className;

    private bool isStatic;

    //类名


    public override void EnterClassDeclaration(CSharpParser.ClassDeclarationContext context)
    {
        className = context.GetChild(1).GetText();
        goStr.AppendLine("package main");
        goStr.AppendLine($"type {className} struct " + "{");
        goStr.AppendLine("}");
        base.EnterClassDeclaration(context);
    }

    public override void ExitClassDeclaration(CSharpParser.ClassDeclarationContext context)
    {
        var index = goStr.ToString().IndexOf("{", StringComparison.Ordinal);
        goStr.Insert(index + 1, "\n" + member);
        Console.WriteLine(goStr.ToString());

        var path = Environment.CurrentDirectory + $"\\{className}.go";

        var bytes = Encoding.UTF8.GetBytes(goStr.ToString());
        File.WriteAllBytes(path, bytes);

        var info = new ProcessStartInfo();
        info.FileName = "gofmt.exe";
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.UseShellExecute = true;
        info.ErrorDialog = true;
        info.Arguments = "-w " + Environment.CurrentDirectory;
        Process.Start(info).WaitForExit();

        base.ExitClassDeclaration(context);
    }


    private string GetArrayType(string tpye)
    {
        if (tpye.IndexOf("[", StringComparison.Ordinal) != -1 && tpye.IndexOf("]", StringComparison.Ordinal) != -1)
        {
            tpye= tpye.Replace("[", "").Replace("]", "");
            return "[]" + tpye;
        }
        else if (tpye.IndexOf("Dictionary", StringComparison.Ordinal) != -1)
        {
            tpye = tpye.Replace("Dictionary<", "").Replace(">", "");
            var dicInfo =tpye.Split(",");
            return $"map[{dicInfo[0]}]{dicInfo[1]}";
        }
        else
        {
            return tpye;
        }
    }

    //类成员    int Attack;
    public override void EnterMemberDeclaration(CSharpParser.MemberDeclarationContext context)
    {
        for (var i = 0; i < context.ChildCount; i++)
            if (context.GetChild(i) is CSharpParser.FieldDeclarationContext)
            {
                if (context.GetChild(i) is CSharpParser.FieldDeclarationContext fieldDeclarationContext)
                {
              
                    if (!isStatic)
                    {
                        var fieldvalue = fieldDeclarationContext.GetChild(1).GetChild(0).GetChild(0);
                        var str = fieldvalue.GetText() + " " + GetArrayType(fieldDeclarationContext.GetChild(0).GetText());
                        member.AppendLine(str);
                    }
                    else
                    {
                        var fieldvalue = fieldDeclarationContext.GetChild(1).GetChild(0).GetText();
                        var str = "var "+ fieldvalue;
                        goStr.AppendLine(str);
                    }
                 
                }
            }

        //Console.WriteLine("evenet EnterMemberDeclaration:  "+isStatic.ToString() +"  "+ context.GetText());
        base.EnterMemberDeclaration(context);
    }

    //Attribute
    public override void EnterAttributeDeclaration(CSharpParser.AttributeDeclarationContext context)
    {
     //   Console.WriteLine("Event EnterAttributeDeclaration: "+context.GetText());
        base.EnterAttributeDeclaration(context);
    }
    

    public override void EnterClassBodyDeclaration(CSharpParser.ClassBodyDeclarationContext context)
    {
        isStatic = false;
        if (context.GetChild(1) is CSharpParser.ModifierContext)
        {
            if (context.GetChild(1).GetText().ToLower() == "static")
            {
                isStatic = true;
            }
        }
        //Console.WriteLine("Event EnterClassBodyDeclaration: "+context.GetText());
        base.EnterClassBodyDeclaration(context);
    }

    //MemberDeclaration
    //MethodDeclaration
    //类函数 
    public override void EnterMethodDeclaration(CSharpParser.MethodDeclarationContext context)
    {
        for (var i = 0; i < context.ChildCount; i++)
        {
            var child = context.GetChild(i);
            //      Console.WriteLine("child"+i+": "+child.GetText());
        }

      //  Console.WriteLine("Event EnterMethodDeclaration:"+context.GetText());
        var returnType = context.GetChild(0).GetText();
        if (typeof(void).Name.ToLower() == returnType)
            returnType = string.Empty;


        var paramets = context.GetChild(2);

        var parametStr = new StringBuilder();
        for (var i = 0; i < paramets.ChildCount; i++)
        {
            var iParseTree = paramets.GetChild(i);

            if (iParseTree is TerminalNodeImpl)
            {
                parametStr.Append(iParseTree.GetText());
            }
            else if (iParseTree is CSharpParser.FormalParameterContext)
            {
                var type = iParseTree.GetChild(0);
                var name = iParseTree.GetChild(1);
                parametStr.Append(name.GetText() + " " + type.GetText());
            }
            else if (iParseTree is CSharpParser.FormalParameterListContext)
            {
                for (var j = 0; j < iParseTree.ChildCount; j++)
                {
                    var item = iParseTree.GetChild(j);
                    if (item is TerminalNodeImpl)
                    {
                        parametStr.Append(item.GetText());
                    }
                    else if (item is CSharpParser.FormalParameterContext)
                    {
                        var type = item.GetChild(0);
                        var name = item.GetChild(1);
                        parametStr.Append(name.GetText() + " " + type.GetText());
                    }
                }
            }
        }

        if (isStatic)
        {
            goStr.AppendLine($"func  {context.GetChild(1)} {parametStr} {returnType}" + " {");
        }
        else
        {
            goStr.AppendLine($"func (tn *{className}) {context.GetChild(1)} {parametStr} {returnType}" + " {");
        }
      // Console.WriteLine("isStatic"+isStatic+ " evenet EnterMethodDeclaration:  "+context.GetText());
        base.EnterMethodDeclaration(context);
    }

    public override void ExitMethodDeclaration(CSharpParser.MethodDeclarationContext context)
    {
        goStr.AppendLine("}");
        base.ExitMethodDeclaration(context);
    }


    //函数过程
    public override void EnterBlockStatement(CSharpParser.BlockStatementContext context)
    {
        //  goStr.AppendLine(context.GetText());

        //    Console.WriteLine("evenet EnterBlockStatement:  " + context.GetText());
        base.EnterBlockStatement(context);
    }

    //局部变量信息
    public override void EnterLocalVariableDeclaration(CSharpParser.LocalVariableDeclarationContext context)
    {
        var type = context.GetChild(0);
        var name = context.GetChild(1);

        if (type.GetChild(0) is CSharpParser.PrimitiveTypeContext)
            goStr.AppendLine("var " + name.GetText());
        else if (type.GetChild(0) is CSharpParser.ClassOrInterfaceTypeContext)
            goStr.AppendLine("var " + name.GetChild(0).GetChild(0).GetText() + " = new(" + type.GetText() + ")");

//        goStr.AppendLine("var "+name.GetText());
        // Console.WriteLine("event EnterLocalVariableDeclaration:  "+context.GetText());
        base.EnterLocalVariableDeclaration(context);
    }


    //函数局部语法  
    public override void EnterStatement(CSharpParser.StatementContext context)
    {
       // Console.WriteLine("evenet EnterStatement:  " + context.GetText());
        if (context.GetChild(0) is CSharpParser.ExpressionContext)
        {
            if (context.GetChild(0).GetChild(0) is CSharpParser.MethodCallContext) //函数调用
            {

             string str = context.GetChild(0).GetChild(0).GetText();

             if (str.IndexOf("this.", StringComparison.Ordinal) != -1)
             {
                 goStr.AppendLine(str.Replace("this.","tn.")); 
             }
             else
             {
                 goStr.AppendLine("tn."+str); 
             }
            
            }
            else //当作普通表达式计算
            {
                var str = context.GetText();
                str = str.Replace(";", "");
                goStr.AppendLine(str);
            }
        }
        else
        {
            var str = context.GetText();
            str = str.Replace(";", "");
            str = str.Replace("return", "return ");
            goStr.AppendLine(str);
        }

        base.EnterStatement(context);
    }

    public override void ExitStatement(CSharpParser.StatementContext context)
    {
        base.ExitStatement(context);
    }


    public override void EnterCreator(CSharpParser.CreatorContext context)
    {
        // Console.WriteLine("evenet EnterCreator:  " + context.GetText());
        base.EnterCreator(context);
    }

    public override void EnterClassType(CSharpParser.ClassTypeContext context)
    {
        // Console.WriteLine("evenet EnterClassType:  " + context.GetText());
        base.EnterClassType(context);
    }

    public override void EnterBlock(CSharpParser.BlockContext context)
    {
        base.EnterBlock(context);
    }

    public override void EnterArguments(CSharpParser.ArgumentsContext context)
    {
        //  Console.WriteLine("evenet EnterArguments:  " + context.GetText());
        base.EnterArguments(context);
    }

    public override void EnterResources(CSharpParser.ResourcesContext context)
    {
        //  Console.WriteLine("evenet EnterResources:  " + context.GetText());
        base.EnterResources(context);
    }


    public override void EnterGenericMethodDeclaration(CSharpParser.GenericMethodDeclarationContext context)
    {
        //   Console.WriteLine("evenet EnterGenericMethodDeclaration:  " + context.GetText());
        base.EnterGenericMethodDeclaration(context);
    }

    public override void EnterInterfaceMethodModifier(CSharpParser.InterfaceMethodModifierContext context)
    {
        // Console.WriteLine("evenet EnterInterfaceMethodModifier:  " + context.GetText());
        base.EnterInterfaceMethodModifier(context);
    }

    public override void EnterInterfaceMemberDeclaration(CSharpParser.InterfaceMemberDeclarationContext context)
    {
        // Console.WriteLine("evenet EnterInterfaceMemberDeclaration:  " + context.GetText());
        base.EnterInterfaceMemberDeclaration(context);
    }

    public override void EnterGenericInterfaceMethodDeclaration(
        CSharpParser.GenericInterfaceMethodDeclarationContext context)
    {
        //Console.WriteLine("evenet EnterGenericInterfaceMethodDeclaration:  " + context.GetText());
        base.EnterGenericInterfaceMethodDeclaration(context);
    }

    public override void EnterInterfaceMethodDeclaration(CSharpParser.InterfaceMethodDeclarationContext context)
    {
        // Console.WriteLine("evenet EnterInterfaceMethodDeclaration:  " + context.GetText());
        base.EnterInterfaceMethodDeclaration(context);
    }

    public override void EnterAnnotationMethodRest(CSharpParser.AnnotationMethodRestContext context)
    {
        // Console.WriteLine("evenet EnterAnnotationMethodRest:  " + context.GetText());
        base.EnterAnnotationMethodRest(context);
    }

    //值类型
    public override void EnterPrimary(CSharpParser.PrimaryContext context)
    {
        // Console.WriteLine("evenet Primary:  " + context.GetText());
        base.EnterPrimary(context);
    }

    public override void EnterTypeType(CSharpParser.TypeTypeContext context)
    {
        // Console.WriteLine("evenet EnterTypeType:  " + context.GetText());
        base.EnterTypeType(context);
    }

    public override void EnterModifier(CSharpParser.ModifierContext context)
    {
        // Console.WriteLine("evenet EnterModifier:  " + context.GetText());
        base.EnterModifier(context);
    }


    public override void EnterTypeParameter(CSharpParser.TypeParameterContext context)
    {
        // Console.WriteLine("evenet EnterTypeParameter:  " + context.GetText());
        base.EnterTypeParameter(context);
    }


    public override void EnterMethodBody(CSharpParser.MethodBodyContext context)
    {
        //  Console.WriteLine("evenet EnterMethodBody:  " + context.GetText());
        base.EnterMethodBody(context);
    }

    public override void EnterVariableModifier(CSharpParser.VariableModifierContext context)
    {
        // Console.WriteLine("evenet EnterVariableModifier:  " + context.GetText());
        base.EnterVariableModifier(context);
    }

    public override void EnterVariableDeclarator(CSharpParser.VariableDeclaratorContext context)
    {
        //   Console.WriteLine("evenet EnterVariableDeclarator:  " + context.GetText());
        base.EnterVariableDeclarator(context);
    }

    //函数语句 num3=num+num1
    public override void EnterVariableDeclarators(CSharpParser.VariableDeclaratorsContext context)
    {
        // Console.WriteLine("evenet EnterVariableDeclarators:  " + context.GetText());
        base.EnterVariableDeclarators(context);
    }

    public override void EnterElementValue(CSharpParser.ElementValueContext context)
    {
        //  Console.WriteLine("evenet EnterElementValue:  " + context.GetText());
        base.EnterElementValue(context);
    }

    public override void EnterVariableInitializer(CSharpParser.VariableInitializerContext context)
    {
        //     Console.WriteLine("evenet EnterVariableInitializer:  " + context.GetText());
        base.EnterVariableInitializer(context);
    }

    public override void EnterElementValuePair(CSharpParser.ElementValuePairContext context)
    {
        //   Console.WriteLine("evenet EnterElementValuePair:  " + context.GetText());
        base.EnterElementValuePair(context);
    }

    public override void EnterExpression(CSharpParser.ExpressionContext context)
    {
        // Console.WriteLine("evenet EnterExpression:  " + context.GetText());
        base.EnterExpression(context);
    }


    public override void EnterLiteral(CSharpParser.LiteralContext context)
    {
        //  Console.WriteLine("evenet EnterLiteral:  " + context.GetText());
        base.EnterLiteral(context);
    }


    public override void EnterAnnotation(CSharpParser.AnnotationContext context)
    {
        //  Console.WriteLine("evenet EnterAnnotation:  " + context.GetText());
        base.EnterAnnotation(context);
    }

    public override void EnterForInit(CSharpParser.ForInitContext context)
    {
        // Console.WriteLine("evenet EnterForInit:  " + context.GetText());
        base.EnterForInit(context);
    }

    public override void EnterCatchType(CSharpParser.CatchTypeContext context)
    {
        //   Console.WriteLine("evenet EnterCatchType:  " + context.GetText());
        base.EnterCatchType(context);
    }

    //函数调用 Show()
    public override void EnterMethodCall(CSharpParser.MethodCallContext context)
    {
        //   Console.WriteLine("evenet EnterMethodCall:  " + context.GetText());
        base.EnterMethodCall(context);
    }

    // 引用包  usingSystem;

    public override void EnterUsingDeclaration(CSharpParser.UsingDeclarationContext context)
    {
        //  Console.WriteLine("evenet EnterUsingDeclaration:  " + context.GetText());
        base.EnterUsingDeclaration(context);
    }

    public override void EnterEveryRule(ParserRuleContext context)
    {
        //   Console.WriteLine("evenet EnterEveryRule:  " + context.GetText());
        base.EnterEveryRule(context);
    }

    public override void EnterResource(CSharpParser.ResourceContext context)
    {
        //  Console.WriteLine("evenet EnterResource:  " + context.GetText());
        base.EnterResource(context);
    }

    public override void EnterTypeList(CSharpParser.TypeListContext context)
    {
        // Console.WriteLine("evenet EnterTypeList:  " + context.GetText());
        base.EnterTypeList(context);
    }

    public override void EnterTypeBound(CSharpParser.TypeBoundContext context)
    {
        //  Console.WriteLine("evenet EnterTypeBound:  " + context.GetText());
        base.EnterTypeBound(context);
    }

    public override void EnterForControl(CSharpParser.ForControlContext context)
    {
        //  Console.WriteLine("evenet EnterForControl:  " + context.GetText());
        base.EnterForControl(context);
    }

    public override void EnterLambdaBody(CSharpParser.LambdaBodyContext context)
    {
        // Console.WriteLine("evenet EnterLambdaBody:  " + context.GetText());
        base.EnterLambdaBody(context);
    }

    public override void EnterCatchClause(CSharpParser.CatchClauseContext context)
    {
        //  Console.WriteLine("evenet EnterCatchClause:  " + context.GetText());
        base.EnterCatchClause(context);
    }

    public override void EnterCreatedName(CSharpParser.CreatedNameContext context)
    {
        //  Console.WriteLine("evenet EnterCreatedName:  " + context.GetText());
        base.EnterCreatedName(context);
    }

    public override void EnterFinallyBlock(CSharpParser.FinallyBlockContext context)
    {
        //   Console.WriteLine("evenet EnterFinallyBlock:  " + context.GetText());
        base.EnterFinallyBlock(context);
    }

    public override void EnterSwitchLabel(CSharpParser.SwitchLabelContext context)
    {
        //  Console.WriteLine("evenet EnterSwitchLabel:  " + context.GetText());
        base.EnterSwitchLabel(context);
    }

    public override void EnterSuperSuffix(CSharpParser.SuperSuffixContext context)
    {
        //  Console.WriteLine("evenet EnterSuperSuffix:  " + context.GetText());
        base.EnterSuperSuffix(context);
    }

    public override void EnterFloatLiteral(CSharpParser.FloatLiteralContext context)
    {
        //   Console.WriteLine("evenet EnterFloatLiteral:  " + context.GetText());
        base.EnterFloatLiteral(context);
    }

    public override void EnterDefaultValue(CSharpParser.DefaultValueContext context)
    {
        //   Console.WriteLine("evenet EnterDefaultValue:  " + context.GetText());
        base.EnterDefaultValue(context);
    }

    public override void EnterEnumDeclaration(CSharpParser.EnumDeclarationContext context)
    {
        //Console.WriteLine("evenet EnterEnumDeclaration:  " + context.GetText());
        base.EnterEnumDeclaration(context);
    }

    public override void EnterEnumConstant(CSharpParser.EnumConstantContext context)
    {
        // Console.WriteLine("evenet EnterEnumConstant:  " + context.GetText());
        base.EnterEnumConstant(context);
    }

    public override void EnterTypeArgument(CSharpParser.TypeArgumentContext context)
    {
        // Console.WriteLine("evenet EnterTypeArgument:  " + context.GetText());
        base.EnterTypeArgument(context);
    }

    public override void EnterQualifiedName(CSharpParser.QualifiedNameContext context)
    {
        //Console.WriteLine("evenet EnterQualifiedName:  " + context.GetText());
        base.EnterQualifiedName(context);
    }

    public override void EnterTypeParameters(CSharpParser.TypeParametersContext context)
    {
        //   Console.WriteLine("evenet EnterTypeParameters:  " + context.GetText());
        base.EnterTypeParameters(context);
    }
}