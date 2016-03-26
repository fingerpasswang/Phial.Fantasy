using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Auto.Server;
using RPCBase;
using RPCBase.Server;

namespace Test.SceneServer
{
    class Program
    {
        public static GateAdaptor GateAdaptor;
        public static Scene2CliServiceDelegate Scene2CliService;

        static void InitService()
        {
            AutoInit.Init();

            RoutingRule.DistrictsName = "test";

            GateAdaptor = new GateAdaptor("127.0.0.1", 6999);

            ServiceImplementStub.Bind<ICli2SceneImpl>(GateAdaptor, new SceneClientServiceImpl());

            Scene2CliService = new Scene2CliServiceDelegate(GateAdaptor);

            GateAdaptor.BeginReceive();
            //Heartbeat();
        }

        private static void Heartbeat()
        {
            TimerCallback cb = state =>
            {
                //Console.WriteLine(string.Format("LogicServer.Heartbeat: {0}", ServiceMetaInfo.DistrictsName));
            };
            new Timer(cb, null, (int)(new Random().NextDouble() * 5), 5000);
        }

        static void Main(string[] args)
        {
            InitService();

            Interoperate();

            while (true)
            {
                GateAdaptor.Poll();
            }
        }

        static void Interoperate()
        {
            new Thread(() =>
            {
                while (true)
                {
                    var inputs = Console.ReadLine().Split(' ');
                    int method = 1;

                    if (inputs.Length < 2)
                    {
                        Console.WriteLine("method arg");
                        continue;
                    }
                    if (!int.TryParse(inputs[0], out method) || (method < 0 || method > 3))
                    {
                        Console.WriteLine("1:AskChangeName 2:AskAddMoney 3:AskLearnSkill");
                        continue;
                    }

                    switch (method)
                    {
                        case 1:
                            {
                                var uuid = new Guid(inputs[1]);

                                GateAdaptor.Subscribe<ICli2SceneImpl>(uuid.ToByteArray());

                                break;
                            }
                        case 2:
                            {
                                var uuid = new Guid(inputs[1]);

                                GateAdaptor.AddForward<ICli2SceneImpl>(uuid.ToByteArray(), 1);

                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
           ).Start();
        }
    }
}
