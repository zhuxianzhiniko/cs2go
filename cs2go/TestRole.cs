    using System.Collections.Generic;
    using static TestRole;

    [Test]
public class TestRole
    {
        public int Attack;
        public int Defense;
        public  bool Flg;
        public static   string Name = "MyName";
//readonly
        public int[] Strs;

        public Dictionary<string, int> d;
        

        
//        public static int[] s_trss;
        
        public int GetHurt(TestRole testRole,int callNum)
        {
           Show();
           QQQShow();
           int num = 100;
         
            /*int dd = num * 100 + 100;
            int num3 = num + num1;
            TestRole role = new TestRole();
            return callNum - testRole.Defense;*/
            
         

            int num1 = 101;
            for (int i = 0; i < testRole.Strs.Length; i++)
            {
                num1++;
            }
            return num;
        }

      
        public  int TestFunc(int num, string name)
        {
            return 100;
        }

       
        public static  void ShowTest(int num)
        {
          
        }
        public  void Show()    
        {
           
        }
     
        public static void QQQShow()
        {
            
        }
    }
