## CsharpToGo的工具

* 使用[antlr4](https://www.antlr.org/about.html)对.cs文件进行语法解析生成.go代码，目前使用的是基于 [java4.g4](https://github.com/antlr/antlr4/blob/master/tool-testsuite/test/org/antlr/v4/test/tool/Java.g4) 修改的CSharpLexer.g4
* 使用[gofmt.exe](https://golang.org/cmd/gofmt/)对生成go代码进行格式化


## 语法支持

**已支持的语法**
* 基本的所有语法 比如 int num = 100 , func(100) staticFun() 
* 常规的运算符  int num = 100 - 10 * 5 + 4 / 10
* for
* switch
* Csharp类结构/类的实例化(new)
* 字典/数组的声明
* 静态函数/类函数
* 接口
* const
* enum
* 局部变量var 

**计划支持的Csharp语法/API/运算符**

* array/List 的增删改查API
* map/Dictionary 的增删改查API
* package

**考虑支持的语法/API**

* buffer类型操作
* Math 常用API


**不支持的CSharp语法/API/运算符/限制**

* go的 [关键字](https://github.com/Unknwon/the-way-to-go_ZH_CN/blob/master/eBook/04.1.md) 不能为变量名,函数名，类名等;比如：int go = 100;
* CSharp 继承，一般泛型（数组/字典 泛型除外），Task,async,LINQ,out,delegate等独有特性; 比如：async Task Show<T>(T num)
* 三元运算符
* ?.运算符
* CSharp接口的get;set; 比如：int GetHp { get; set; }
* 不支持while，由于go没有while控制结构


## 安全忽略的CSharp关键字

* using
* namespace
* readonly
* 所有的访问修饰符(private,public等)

## 语言的差异处理

* go没有class，所以CSharp的class当作go的struct
* go的struct无法声明初始化值，只能自定义一个函数
* CSharp中静态函数和静态变量/静态常量 都在go里都当作全局函数/全局变量/全局常量
* go没有Enum类型，使用iota模拟CSharp Enum; [参考实现](https://studygolang.com/articles/5386)







