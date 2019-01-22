using System.Collections.Generic;
[Test]
public class TestClass
{
    public static string Name = "MyName";
    public const int Max = 100;
    public readonly bool OnlyFlg = false;
    public int Attack;

    public Dictionary<string, int> d;
    public int Defense;

    public bool Flg;

    public int[] Strs;

    public static TestClass Init()
    {
        TestClass testClass = new TestClass();
        testClass.Attack = 10;
        testClass.Defense = 20;
        testClass.Flg = false;
        return testClass;
    }


    public int GetHurt(TestClass testClass, int callNum)
    {
        Show();
        StShow();
        int num = 100;
        TestClass test = new TestClass();
        int dd = num * 100 + 100;
        int num3 = num + 1;

        int len = test.Strs.Length;
        int num1 = 101;
        for (var i = 10; i < 100; i++)
        {
            num1++;
        }
        switch (num1)
        {
           case 1:
               num1+=1;
               break;
           
           case 2:
               num1+=2;
               break;
           case 3:
               num1+=3;
               break;
           default:
               num1+=10;
               break;
        }
    

        return testClass.Strs.Length;
    }


    public int TestFunc(int num, string name)
    {
        return 100;
    }


    public static int ShowTest(int num)
    {
        return 100;
    }

    public void Show()
    {
    }

    public static void StShow()
    {
    }

    public int GetMaxAttack()
    {
        return 100;
    }
}