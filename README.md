**CsharpToGo的工具**

* 使用 [antlr4](https://www.antlr.org/about.html)对.cs文件进行语法解析生成.go代码，目前使用的是基于java4.g4的修改的CSharpLexer.g4
* 使用 [gofmt.exe](https://golang.org/cmd/gofmt/)对生成go代码进行格式化


**尚未支持，但计划支持的Csharp语法/API/运算符**

* 接口
* go type的实例化函数
* while
* switch
* enum
* array/List 的增删改查API
* map/Dictionary 的增删改查API
* random
* Mathf
* const/readonly 
* package

**不支持的Csharp语法/API/运算符/限制**

* go的 [关键字](https://github.com/Unknwon/the-way-to-go_ZH_CN/blob/master/eBook/04.1.md) 不能为变量名,函数名，类名等;比如：int go = 100;
* Csharp 继承，一般泛型（数组/字典 泛型除外），Task,async,LINQ,out,delegate等独有特性; 比如：async Task Show<T>(T num)
* 局部变量，变量类型不支持var; 比如：var num = 100;
* 三元运算符
* ?.运算符



