using System.Linq.Expressions;
using System.Net;
using System.Speech;
using System.Speech.Synthesis;

public class Program
{

    public static void Main()
    {



        string str = "abcd";

        //IPHostEntry hostInfo = Dns.GetHostEntry("www.59yangzhi.com");
        IPHostEntry hostInfo = Dns.GetHostEntry("www.github.com");

        var curDnsHostName= Dns.GetHostName();






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






