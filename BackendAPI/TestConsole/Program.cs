namespace GenericDelegateExample
{
    // 定义一个泛型委托类型
    public delegate TResult MyDelegate<T, TResult>(T arg);

    class Program
    {
        static void Main(string[] args)
        {
            // 声明一个将字符串转换为大写的委托
            MyDelegate<string, string> toUpper = s => s.ToUpper();

            // 声明一个将字符串转换为小写的委托
            MyDelegate<string, string> toLower = s => s.ToLower();

            // 使用委托将字符串列表中的所有元素转换为大写形式
            List<string> strings = new List<string> { "Hello", "World", "!" };
            List<string> upperCaseStrings = Map(strings, toUpper);
            Console.WriteLine(string.Join(" ", upperCaseStrings));

            // 使用委托将字符串列表中的所有元素转换为小写形式
            List<string> lowerCaseStrings = Map(strings, toLower);
            Console.WriteLine(string.Join(" ", lowerCaseStrings));
        }

        // 定义一个通用的映射方法，使用泛型委托允许动态指定转换逻辑
        public static List<TResult> Map<T, TResult>(IList<T> source, MyDelegate<T, TResult> selector)
        {
            var result = new List<TResult>();

            foreach (var item in source)
            {
                result.Add(selector(item));
            }

            return result;
        }
    }
}
