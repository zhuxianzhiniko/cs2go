using System.Collections.Generic;

public class TestClass
{
    public enum MyTestEnum
     {
         One1,
         Tow1
     }
     /*public bool Flg;
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
    }*/
     public void Show()
     {
     }

    public int GetHurt(List<int> list, TestClass testClass, int callNum)
    {
       // int[] d = new int[] {1, 2};
        List<int> d1= new List<int>(){1,2,3,4};
       // var listLen = d1.Count;
    //    var arrLen = d.Length;
        Show();
        d1.RemoveAt(0);

        /*int num1 = 100;
        var ex = 100 + 2 - 10 * 100 / 2;
        var num = GetMaxAttack(callNum);
        StShow();
        var test = new TestClass();
        for (int i = 10; i < 100; i++)
        {
            num1++;
        }
        switch (100)
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
        }*/
        
        
       
        // Strs = new List<int>{1,2,3,4,5,6};


        /* Show();
         StShow();
         var num = 100;
         var test = new TestClass();
         var dd = num * 100 + 100;
         var num3 = num + 1;
   
      
       
 
 
         return testClass.Strs.Length;*/
        return d1.Count;
    }


   /* public int TestFunc(int num, string name)
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

    public int GetMaxAttack(int num)
    {
        return 100;
    }*/
}