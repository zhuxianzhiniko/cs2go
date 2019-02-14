## CsharpToGo的工具

* 使用[ roslyn ](https://github.com/dotnet/roslyn)对.cs文件进行代码分析,根据go语法特性生成go代码
* 使用[ gofmt.exe ](https://golang.org/cmd/gofmt/)对生成go代码进行格式化


## 语法支持

**已支持的语法**
* 基本的语法和运算符 参考已测试[例子](https://github.com/zhuxianzhiniko/cs2go/blob/master/cs2go/TestCase/TestClass.cs) 
* 支持的数据类型 int,long,short,(包括有符号和无符号),bool,string,float,double,class,enum,Dictionary,List
* 支持的控制结构 for,switch,if
* 支持的修饰符 const,readonly,static

**已转译的Csharp API**

* `List.RemoveAt(0)` To `List = append(List[:0], a[0+1:]...)`
* `list.Count` To `cap(list)`
* `list.Length` To `cap(list)`
* `list.Add(100)` To `list = append(list, 100)`
* `list.Clear()` To `list = list[:0:0]`



**因为go的语言特性而限制的Csharp语法**

* go的 [关键字](https://github.com/Unknwon/the-way-to-go_ZH_CN/blob/master/eBook/04.1.md) 不能为变量名,函数名，类名等;比如：int go = 100;
* 局部变量不要和全局变量名或当前类成员一致 (区分大小写)
* CSharp 继承，一般泛型（数组/字典 泛型除外），Task,async,LINQ,out,delegate等独有特性; 比如：async Task Show<T>(T num)
* 三元运算符
* ?.运算符
* foreach
* while
* CSharp接口的get;set; 比如：int GetHp { get; set; }

## 语言的差异处理

* go没有class，所以CSharp的class当作go的struct
* go的struct无法声明初始化值，只能自定义一个函数
* CSharp中静态函数和静态变量/静态常量 都在go里都当作全局函数/全局变量/全局常量
* go没有Enum类型，使用iota模拟CSharp Enum; [参考实现](https://studygolang.com/articles/5386)
* go模拟的enum不能为私有或者类enum;
* CSharp array/List 对于go来讲都是切片(Slice)
* 由于go的切片(Slice)只有对索引的操作，在CSharp这边也统一使用只对索引进行操作的API。list.RemoveAt(0);






