    using System.Collections.Generic;
    using static TestRole;

    [Test]
public class TestRole
    {
        public int Attack;
        public int Defense;
        public bool Flg;
        public static string Name = "MyName";

        public int[] Strs;

        public Dictionary<string, int> d;
        

        
//        public static int[] s_trss;
        
        public int GetHurt(TestRole testRole,int callNum)
        {
            Show();
            QQQShow();
            int num = 100;
            /*int num1 = 101;
            num += 1;
            int dd = num * 100 + 100;
            int num3 = num + num1;
            TestRole role = new TestRole();
            return callNum - testRole.Defense;*/
            return 0;
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

     
        public  void QQQShow()
        {
            
        }
    }
