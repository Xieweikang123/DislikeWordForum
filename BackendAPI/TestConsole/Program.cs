using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq.Expressions;

using System.Speech;
using System.Speech.Synthesis;

public class Program
{

    public static void Main()
    {


        string message = "  potentially  propagation";


        for (var i = 0; i < 10; i++)
        {

            ReadAloud(message);
        }


        Console.WriteLine("ok");
        Console.ReadKey();


    }
    private static void ReadAloud(string message)
    {
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        speechSynthesizer.Volume = 100;
        speechSynthesizer.Rate = 3;
        speechSynthesizer.Speak(message);
    }

}






