using System.Linq.Expressions;

public class Student
{
    public int StudentID { get; set; }
    public string StudentName { get; set; }
    public int Age { get; set; }
}


public class Program
{

    public static void Main()
    {


        int readonlyArgument = 44;
        InArgExample(readonlyArgument);
        Console.WriteLine(readonlyArgument);     // value is still 44

        void InArgExample(in int number)
        {
            // Uncomment the following line to see error CS8331
            //number = 19;
        }
    }

}






