using System;
using System.Collections.Generic;

public class TestClass
{
    public enum MyTestEnum
    {
        One1,
        Tow1
    }

    public Dictionary<int, string> dic;

    public List<int> lists;


    public bool Flg;
    public const int Max = 100;
    public static string Name = "MyName";
    public readonly bool OnlyFlg = false;
    public int Attack;
    public int Defense;
    
    


  public static TestClass Init()
   {
       var testClass = new TestClass();
       testClass.Attack = 10;
       testClass.Defense = 20;
       testClass.Flg = false;
       return testClass;
   }
 

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
        int[] d = new int[] {1, 2};
        List<int> d1 = new List<int> {1, 2, 3, 4};
        
        //API 转译
        var listLen = d1.Count;
        var arrLen = d.Length;
        d1.RemoveAt(0);
        d1.Add(100);
        d1.Clear();
        
        // 字典，数组访问 局部变量，类函数，静态函数调用测试
        dic[100] = "dd";
        var b = list[0];
        dic[100] = list[0];
        StShow();
        ShowTest(100);
        var attack =GetMaxAttack(100);
        GetMaxAttack(100);
        testClass.Show();
        
        //实例化类测试
        var test = new TestClass();
        var test1 = Init();
        test1.Show();

        //基础运算符
        var num1 = 100 + 2 - 10 * 100 / 2;
        
        //for表达式
        for (int i = 10; i < testClass.lists.Count; i++)
        {
            i++;
        }
        
        //switch表达式
        switch (testClass.GetMaxAttack(100))
        {
            case 1:
                //嵌套测试
                for (int i = 10; i < 100; i++)
                {
                    i++;
                    for (int k = 10; k < 100; k++)
                    {
                        k++;

                        switch (1)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                        }
                    }
                }
                break;
 
            
            //+= /= -= 测试
            case 2:
                var k1 = 1;
                k1+= 1;
                break;
            case 3:
                var k2 = 1;
                k2-= 1;
                break;
            default:
                var k3 = 1;
                k3/= 1;
                break;
        }
        
        //返回值
        return testClass.lists.Count;
    }

    public void Show()
    {
    }
    public int TestFunc(int num, string name)
    {
        return 100;
    }


    public static int ShowTest(int num)
    {
        return 100;
    }


    public static void StShow()
    {
    }

    public int GetMaxAttack(int num)
    {
        return 100;
    }
}