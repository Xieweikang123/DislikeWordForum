class Program
{
    // 定义一个方法，它将作为 WaitCallback 委托的参数
    private static void MyTask(object state)
    {
        // 从 state 参数中获取需要的数据
        int num = (int)state;

        // 执行一些工作，例如打印信息
        Console.WriteLine("Hello from MyTask! Parameter = {0}", num);
    }

    static void Main(string[] args)
    {

        ThreadPool.QueueUserWorkItem(new WaitCallback(MyTask), 42);


        Console.WriteLine("ok");
    }
}