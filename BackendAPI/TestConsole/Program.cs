// See https://aka.ms/new-console-template for more information





using System.Linq;


using BackendAPI.Core.Entities;
using System.Reflection;
using TestConsole;

//var engli = new EnglishWord();

//Type ss1 = engli.GetType();
//var t1 = typeof(EnglishWord);

//BindingFlags flag = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance;

//var ss12 = ss1.GetProperty("word", flag);

//var ss = engli.GetType().GetProperty("word", flag).Name;
//var ss2 = engli.GetType().GetProperty("Word", BindingFlags.IgnoreCase);


var wordList = new List<EnglishWord>();

var ss= wordList.OrderBy("", true);
//wordList.Where(x=>x.)



Console.WriteLine("Hello, World!");
