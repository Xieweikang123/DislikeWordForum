using System;
using System.ComponentModel.DataAnnotations;


// Define a class with some data annotations
public class Person
{
    [Required]
    [StringLength(10)]
    public string Name { get; set; }

    [Range(0, 120)]
    public int Age { get; set; }
}



class Program
{
    static void Main(string[] args)
    {


        // Create an instance of the class
        Person p = new Person();
        p.Name = "Alice";
        p.Age = 125;

        // Create a validation context and a list of validation results
        ValidationContext context = new ValidationContext(p);
        List<ValidationResult> results = new List<ValidationResult>();

        // Call TryValidateObject to validate the object
        bool isValid = Validator.TryValidateObject(p, context, results, true);

        // Print the validation result
        Console.WriteLine($"Is valid: {isValid}");
        foreach (var result in results)
        {
            Console.WriteLine(result.ErrorMessage);
        }


        // Wait for user input
        Console.Write("Press any key to exit...");
        Console.ReadKey();
    }
}
