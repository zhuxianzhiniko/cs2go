using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public  class MyEvent : CSharpParserBaseListener
{
    private readonly StringBuilder goStr = new StringBuilder();
    private readonly StringBuilder member = new StringBuilder();
    private string className;


    private  List<string> staticMethods;



    public override void EnterInterfaceMethodDeclaration(CSharpParser.InterfaceMethodDeclarationContext context)
    {
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
        
        goStr.AppendLine($"{context.GetChild(1)}{parametStr} {returnType}" );
  
        base.EnterInterfaceMethodDeclaration(context);
    }

  

  
    public override void EnterInterfaceDeclaration(CSharpParser.InterfaceDeclarationContext context)
    {
        className = context.GetChild(1).GetText();
        goStr.AppendLine("package main");
        Console.WriteLine("event EnterInterfaceDeclaration: "+context.GetText());
        goStr.AppendLine($"type {context.GetChild(1).GetText()} {context.GetChild(0).GetText()}"+"{");
        base.EnterInterfaceDeclaration(context);
    }

    public override void ExitInterfaceDeclaration(CSharpParser.InterfaceDeclarationContext context)
    {
        goStr.AppendLine("}");
        Console.WriteLine(goStr.ToString());
        var path = Environment.CurrentDirectory + $"\\{className}.go";
        SaveGoFile(goStr.ToString(), path);
        base.ExitInterfaceDeclaration(context);
    }


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
        SaveGoFile(goStr.ToString(), path);
        base.ExitClassDeclaration(context);
    }

    private void SaveGoFile(string str,string path)
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

    public bool IsStatic(string key)
    {
        return staticMethods.IndexOf(key) != -1;
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
                    var fieldName = fieldDeclarationContext.GetChild(1).GetChild(0).GetChild(0);

                    if (!IsStatic(fieldName.GetText()))
                    {
                        var str = fieldName.GetText() + " " + GetGoType(fieldDeclarationContext.GetChild(0).GetText());
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


//    public override void EnterClassBodyDeclaration(CSharpParser.ClassBodyDeclarationContext context)
//    {
//        isStatic = false;
//        if (context.GetChild(1) is CSharpParser.ModifierContext)
//            if (context.GetChild(1).GetText().ToLower() == "static")
//                isStatic = true;
//        Console.WriteLine("Event EnterClassBodyDeclaration: "+context.GetText());
//        base.EnterClassBodyDeclaration(context);
//    }


  

    public override void EnterClassBody(CSharpParser.ClassBodyContext context)
    {
        staticMethods = new List<string>();
        for (int i = 0; i < context.ChildCount; i++)
        {
           var child =  context.GetChild(i);

           if (child is CSharpParser.ClassBodyDeclarationContext)
           {
               if (child.ChildCount > 2 && child.GetChild(1).GetText() == "static")
               {
                  var method = child.GetChild(2).GetChild(0);
                  string key = String.Empty;
                  if (method is CSharpParser.FieldDeclarationContext)
                  {
                      key = method.GetChild(1).GetChild(0).GetChild(0).GetText();
                  }
                  else
                  {
                      key = method.GetChild(1).GetText();
                  }
                  staticMethods.Add(key);
               }
               
 
               
           }
        }
        base.EnterClassBody(context);
    }


    private string GetReturnType(string returnType)
    {
      
        if (typeof(void).Name.ToLower() == returnType.ToLower())
            return String.Empty;
        if ("int" == returnType.ToLower())
            return "int";
        if (typeof(string).Name.ToLower() == returnType.ToLower())
            return "string";
        if ("bool" == returnType.ToLower())
            return "bool";
        return "*" + returnType;
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
        var returnType = GetReturnType(context.GetChild(0).GetText());
  


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

        
        if (IsStatic(context.GetChild(1).GetText()))
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
        Console.WriteLine("evenet EnterLocalVariableDeclaration:  " + context.GetText());

        if (context.Parent is CSharpParser.ForInitContext) //屏蔽掉for的语句。因为无法区别for的变量，和普通的局部变量
        {
            base.EnterLocalVariableDeclaration(context);
            return;
        }
        var type = context.GetChild(0);
        var name = context.GetChild(1);

   
            
         if(type.GetChild(0) is CSharpParser.PrimitiveTypeContext )
        {
            goStr.AppendLine($"var {name.GetChild(0).GetChild(0).GetText()}={CSharpAPIToGo(name.GetChild(0)?.GetChild(2))}");
        }
        else if (type.GetChild(0) is CSharpParser.ClassOrInterfaceTypeContext)
        {
              goStr.AppendLine("var " + name.GetChild(0).GetChild(0).GetText() + " = new(" + type.GetText() + ")");
        }

        
        base.EnterLocalVariableDeclaration(context);
    }



    private string CSharpAPIToGo(IParseTree expressionContext)
    {
        var text = expressionContext.GetText().Replace(";","");
        if (expressionContext.GetChild(0) is CSharpParser.MethodCallContext) //函数调用
        {
            var str = expressionContext.GetChild(0).GetText();
            var name = str.Replace("()", "");
            if (IsStatic(name))
                return str;
            else
                return "tn." + str;
        }
        else if (text.IndexOf(".Length", StringComparison.Ordinal) != -1) //数组
        {
            return $"cap({expressionContext.GetChild(0).GetText().Replace(".Length","")})";
        }

        return expressionContext.GetText().Replace(";","");;
    }



    //函数局部语法  
    public override void EnterStatement(CSharpParser.StatementContext context)
    {
         //Console.WriteLine("evenet EnterStatement:  " + context.GetText());
        if (context.GetChild(0) is CSharpParser.ExpressionContext) //普通的表达式
        {
            goStr.AppendLine(CSharpAPIToGo(context.GetChild(0)));
   
        }
        else if (context.GetChild(0) is TerminalNodeImpl && context.GetChild(0).GetText() == "for") //for循环
        {
            var forContext = context.GetChild(2) as CSharpParser.ForControlContext;

            var ex = forContext.GetChild(0).GetChild(0).GetChild(1).GetChild(0);

            var ex1 = forContext.GetChild(2);

            var exStr = CSharpAPIToGo(ex1.GetChild(0)) + ">" + CSharpAPIToGo(ex1.GetChild(2));
            
            goStr.Append(
                $"for {ex.GetChild(0).GetText()} := {ex.GetChild(2).GetText()}; {exStr}; {forContext.GetChild(4).GetText()}");
        }
        else if (context.GetChild(0) is CSharpParser.BlockContext) //忽略
        {
            
        }
        else if (context.GetChild(0) is TerminalNodeImpl && context.GetChild(0).GetText() == "return")//return 
        {
            goStr.AppendLine("return "+CSharpAPIToGo(context.GetChild(1)));
        }

        base.EnterStatement(context);
    }

}