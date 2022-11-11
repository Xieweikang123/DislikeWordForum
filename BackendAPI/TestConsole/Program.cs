using System.Linq.Expressions;

using System.Speech;
using System.Speech.Synthesis;

public class Program
{

    public static void Main()
    {


        string message = "  在人工智能时代，文字转语音是现在人工智能比较热门的功能，各大公司都有这方面的业务，可以可以通过接口对各种文字转语音，甚至能模拟真人，非常的强大,.NET东家微软其实也有这方面的服务。如果大家对语言转文字的要求不高，可以用微软自己的语音转换类库，而且实现很简单，只需要5行代码实现，本文将介绍如何使用";


        for (var i = 0; i < 1; i++)
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






