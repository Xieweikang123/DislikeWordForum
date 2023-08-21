using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
     

    static void Main()
    {
        MyDelegate d1 = SayEnglish;
        string s1 = d1(3);
        Console.WriteLine(s1);
        d1 = SayChinese;
        string s2 = d1(5);
        Console.WriteLine(s2);

        Console.ReadLine();
    }



    static string SayEnglish(int age)
    {
        return $"Hello {age}";
    }
    static string SayChinese(int age)
    {
        return $"你好 {age}";
    }
    delegate string MyDelegate(int n);

}
