using System.Collections.Generic;

[Test]
public class TestClass
{
    public enum MyTestEnum
    {
        One1,
        Tow1
    }

    public const int Max = 100;
    public static string Name = "MyName";
    public readonly bool OnlyFlg = false;
    public int Attack;

    public Dictionary<string, int> d;
    public int Defense;

    public bool Flg;

    public int[] Strs;

    public static TestClass Init()
    {
        var testClass = new TestClass();
        testClass.Attack = 10;
        testClass.Defense = 20;
        testClass.Flg = false;
        return testClass;
    }


    public int GetHurt(TestClass testClass, int callNum)
    {
        Show();
        StShow();
        var num = 100;
        var test = new TestClass();
        var dd = num * 100 + 100;
        var num3 = num + 1;

        var len = test.Strs.Length;
        var num1 = 101;
        for (int i = 10; i < 100; i++)
        {
            num1++;
        }

        switch (num1)
        {
            case 1:
                num1 += 1;
                break;

            case 2:
                num1 += 2;
                break;
            case 3:
                num1 += 3;
                break;
            default:
                num1 += 10;
                break;
        }


        return testClass.Strs.Length;
        return 0;
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