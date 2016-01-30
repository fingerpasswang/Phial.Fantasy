using System.Threading;

namespace ClientTest
{
    class Program
    {
        static void Main()
        {
            var client = new Client();
            while (true)
            {
                client.MainLoop();
                Thread.Sleep(100);
            }
        }
    }
}
