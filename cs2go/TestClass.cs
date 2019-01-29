using System;
using System.Collections.Generic;
using System.Diagnostics;

public class TestClass
{
    //Enum测试
    public enum MyTestEnum
    {
        One,
        Tow
    }
  
    //数组字典测试
    public Dictionary<int, string> dic;
    public List<int> lists;


    //静态/常量/只读常量测试
    public const int Max = 100;
    public static string Name = "MyName";
    public readonly bool OnlyFlg = false;

    //类成员测试
    public int Attack;
    public int Defense;
    public bool Flg;
    public MyTestEnum myTestEnum;


    //实例化返回测试    
    public static TestClass Init()
    {
        var testClass = new TestClass();
        testClass.Attack = 10;
        testClass.Defense = 20;
        testClass.Flg = false;
        return testClass;
    }

    //主要是函数过程测试
    public int GetHurt(List<string> list, TestClass testClass, int callNum)
    {
      
        
        //变量声明语法
        var testVar = 0;
        int TestInt = 0;
        uint TestUint = 0;
        short testshort = 0;
        ushort testUshort = 0;
        ulong testUulong = 0;
        long testUlong = 0;
        float testFloat = 0f;
        double testdouble = 0f;
        string str = "sss";
        bool flg = false;
       MyTestEnum myTestEnum = MyTestEnum.One;
        
        int[] TestArray = new int[] {1, 2};
        List<int> TestList = new List<int> {1, 2, 3, 4};
        List<string> TestList1 = new List<string> {"false","false"};

        
        //API 转译
        var listLen = TestList.Count;
        var arrLen = TestArray.Length;
        TestList.RemoveAt(0);
        TestList.Add(100);
        TestList.Clear();

        // 字典，数组访问 局部变量，类函数，静态函数调用测试
        dic[100] = "dd";
        var b = list[0];
        dic[100] = list[0];
        StShow();
        ShowTest(100);
        var attack = GetMaxAttack(100);
        GetMaxAttack(100);
        testClass.Show();

        //实例化类测试
        var test = new TestClass();

        //获得实例化测试
        var test1 = Init();
        test1.Show();

        //基础运算符
        var num1 = 100 + 2 - 10 * 100 / 2;

        //for表达式
        for (int i = 10; i < testClass.lists.Count; i++)
        {
            i++;
        }
        
        //if表达式
        if (myTestEnum == MyTestEnum.One ||  testClass.dic[100]!="test" && myTestEnum == MyTestEnum.Tow)
        {
            dic[100] = "dd";
        }
        else if(myTestEnum == MyTestEnum.Tow)
        {
            dic[100] = "dd1";
        }
        else
        {
            dic[100] = "dd2";
        }

        //switch表达式
        switch (testClass.GetMaxAttack(100))
        {
            case 1:
                //嵌套测试 1
                for (int i = 10; i < 100; i++)
                {
                    i++;
                    //嵌套测试 2
                    for (int k = 10; k < 100; k++)
                    {
                        k++;

                        //嵌套测试 3
                        switch (1)
                        {
                            case 1:
                                break;
                            case 2:
                                if (myTestEnum == MyTestEnum.One ||  testClass.dic[100]!="test" && myTestEnum == MyTestEnum.Tow)
                                {
                                    dic[100] = "dd";
                                }
                                else if(myTestEnum == MyTestEnum.Tow)
                                {
                                    dic[100] = "dd1";
                                }
                                else
                                {
                                    dic[100] = "dd2";
                                }
                                break;
                        }
                    }
                }

                break;


            //+= /= -= 测试
            case 2:
                var k1 = 1;
                k1 += 1;
                break;
            case 3:
                var k2 = 1;
                k2 -= 1;
                break;
            default:
                var k3 = 1;
                k3 /= 1;
                break;
        }

        //返回值
        return 0;
    }

    //类函数测试 1
    public void Show()
    {
    }


    //类函数测试 2
    public int GetMaxAttack(int num)
    {
        return 100;
    }

    //静态函数测试 1
    public static int ShowTest(int num)
    {
        return 100;
    }

    //静态函数测试 2
    public static void StShow()
    {
    }
}