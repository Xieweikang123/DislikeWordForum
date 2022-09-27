using Microsoft.AspNetCore.Mvc.ApplicationModels;
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


        var appModel = new ApplicationModel();

        appModel.ApiExplorer = new ApiExplorerModel(new ApiExplorerModel()
        {
            IsVisible = true,
            GroupName = "nnn"
        });

        //appModel.



    }

}






