using System.Collections.Generic;



[Test]
public class TestRole:ITestInterface
{
    public static string Name = "MyName";
    public int Attack;

    public Dictionary<string, int> d;
    public int Defense;

    public bool Flg;

    public int[] Strs;


    public int GetHurt(TestRole testRole, int callNum)
    {
        Show();
        StShow();
        int num = 100;
        TestRole role = new TestRole();
        int dd = num * 100 + 100;
        int num3 = num + 1;

        int len = role.Strs.Length;
        int num1 = 101;
        for (var i = 10; i < 100; i++)
        {
            num1++;
        }


        return testRole.Strs.Length;
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