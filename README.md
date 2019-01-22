**CsharpToGo的工具**

* 使用[antlr4](https://www.antlr.org/about.html)对.cs文件进行语法解析生成.go代码，目前使用的是基于 [java4.g4](https://github.com/antlr/antlr4/blob/master/tool-testsuite/test/org/antlr/v4/test/tool/Java.g4) 修改的CSharpLexer.g4
* 使用[gofmt.exe](https://golang.org/cmd/gofmt/)对生成go代码进行格式化


**已支持的语法**

* 基本的所有语法 比如 int num = 100 , func(100) staticFun() 
* 常规的运算符  int num = 100 - 10 * 5 + 4 / 10
* for循环结构
* Csharp类结构/类的实例化(new)
* 字典/数组的声明
* 静态函数/类函数
* 接口


**计划支持的Csharp语法/API/运算符**

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
* Csharp接口的get;set; 比如：int GetHp { get; set; }

**差异处理**

* go没有class，所以Csharp的class当作go struct
* go的struct无法声明初始化值，只能自定义一个函数
* go没有静态函数static标识，Csharp静态函数转译成无指针func函数
* go没有修饰符，直接忽略掉Csharp的修饰符





