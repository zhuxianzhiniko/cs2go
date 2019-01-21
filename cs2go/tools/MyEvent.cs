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


    private string GetGoType(string tpye)
    {
        if (tpye.IndexOf("[", StringComparison.Ordinal) != -1 && tpye.IndexOf("]", StringComparison.Ordinal) != -1)
        {
            tpye = tpye.Replace("[", "").Replace("]", "");
            return "[]" + tpye;
        }

        if (tpye.IndexOf("Dictionary", StringComparison.Ordinal) != -1)
        {
            tpye = tpye.Replace("Dictionary<", "").Replace(">", "");
            var dicInfo = tpye.Split(",");
            return $"map[{dicInfo[0]}]{dicInfo[1]}";
        }

        return tpye;
    }

    //类成员    int Attack;
    public override void EnterMemberDeclaration(CSharpParser.MemberDeclarationContext context)
    {
        for (var i = 0; i < context.ChildCount; i++)
            if (context.GetChild(i) is CSharpParser.FieldDeclarationContext)
                if (context.GetChild(i) is CSharpParser.FieldDeclarationContext fieldDeclarationContext)
                {
                    if (!isStatic)
                    {
                        var fieldvalue = fieldDeclarationContext.GetChild(1).GetChild(0).GetChild(0);
                        var str = fieldvalue.GetText() + " " + GetGoType(fieldDeclarationContext.GetChild(0).GetText());
                        member.AppendLine(str);
                    }
                    else
                    {
                        var fieldvalue = fieldDeclarationContext.GetChild(1).GetChild(0).GetText();
                        var str = "var " + fieldvalue;
                        goStr.AppendLine(str);
                    }
                }

        // Console.WriteLine("evenet EnterMemberDeclaration:  "+isStatic.ToString() +"  "+ context.GetText());
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
            if (context.GetChild(1).GetText().ToLower() == "static")
                isStatic = true;
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

        //Console.WriteLine("Event EnterMethodDeclaration:"+context.GetText());
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
            goStr.Append($"func  {context.GetChild(1)} {parametStr} {returnType}" );
        else
            goStr.Append($"func (tn *{className}) {context.GetChild(1)} {parametStr} {returnType}" );
        // Console.WriteLine("isStatic"+isStatic+ " evenet EnterMethodDeclaration:  "+context.GetText());
        base.EnterMethodDeclaration(context);
    }


 

    public override void EnterBlock(CSharpParser.BlockContext context)
    {
        goStr.Append("{"+"\n");
        base.EnterBlock(context);
    }

    public override void ExitBlock(CSharpParser.BlockContext context)
    {
        goStr.AppendLine("}");
        base.ExitBlock(context);
    }

    //局部变量信息
    public override void EnterLocalVariableDeclaration(CSharpParser.LocalVariableDeclarationContext context)
    {
        var type = context.GetChild(0);
        var name = context.GetChild(1);

        if (context.Parent is CSharpParser.ForInitContext) //屏蔽掉for的语句。因为无法区别for的变量，和普通的局部变量
        {
            base.EnterLocalVariableDeclaration(context);
            return;
        }

        if (type.GetChild(0) is CSharpParser.PrimitiveTypeContext)
            goStr.AppendLine("var " + name.GetText());
        else if (type.GetChild(0) is CSharpParser.ClassOrInterfaceTypeContext)
            goStr.AppendLine("var " + name.GetChild(0).GetChild(0).GetText() + " = new(" + type.GetText() + ")");

        base.EnterLocalVariableDeclaration(context);
    }



    //函数局部语法  
    public override void EnterStatement(CSharpParser.StatementContext context)
    {
       Console.WriteLine("evenet EnterStatement:  " + context.GetText());
        if (context.GetChild(0) is CSharpParser.ExpressionContext)
        {
            if (context.GetChild(0).GetChild(0) is CSharpParser.MethodCallContext) //函数调用
            {
                var str = context.GetChild(0).GetChild(0).GetText();

                if (str.IndexOf("this.", StringComparison.Ordinal) != -1)
                    goStr.AppendLine(str.Replace("this.", "tn."));
                else
                    goStr.AppendLine("tn." + str);
            }
            else //当作普通表达式计算
            {
                var str = context.GetText();
                str = str.Replace(";", "");
                goStr.AppendLine(str);
            }
        }
        else if (context.GetChild(0) is TerminalNodeImpl && context.GetChild(0).GetText() == "for") //for循环
        {
            var forContext = context.GetChild(2) as CSharpParser.ForControlContext;

            var ex = forContext.GetChild(0).GetChild(0).GetChild(1).GetChild(0);
            var bs = context.GetChild(4).GetChild(0).GetChild(1).GetChild(0);

            Console.WriteLine("evenet 4:  " + bs.GetText());

            goStr.Append(
                $"for {ex.GetChild(0).GetText()} := {ex.GetChild(2).GetText()}; {forContext.GetChild(2).GetText()}; {forContext.GetChild(4).GetText()}");
        }
        else if (context.GetChild(0) is CSharpParser.BlockContext)
        {
            
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

}