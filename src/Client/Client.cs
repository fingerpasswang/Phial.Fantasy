using System;
using System.Threading;
using Auto.Client;
using Config;
using RPCBase;
using RPCBase.Client;

namespace ClientTest
{
    public enum Method
    {
        AskChangeName = 1,
        AskAddMoney = 2,
        AskLearnSkill = 3,
        AskMove = 4,
    }
    class Client
    {
        private MqttAdaptor MqttAdaptor;
        private LogicClientServiceDelegate LogicService;
        private LoginClientServiceDelegate LoginService;

        private GateAdaptor GateAdaptor;
        private SceneClientServiceDelegate SceneService;

        private ulong pid;

        public Client()
        {
            AutoInit.Init();
            MqttAdaptor = new MqttAdaptor(ConfigManager.MQIP(), ConfigManager.MQTTPort(),
                ConfigManager.MQUser(), ConfigManager.MQPass());
            GateAdaptor = new GateAdaptor("127.0.0.1", 6888);

            MqttAdaptor.Connected += () =>
            {
                BeginTest();
            };
            MqttAdaptor.BeginReceive();
            GateAdaptor.BeginReceive();
        }

        public void MainLoop()
        {
            MqttAdaptor.Poll();
            GateAdaptor.Poll();
        }

        public void BeginTest()
        {
            Console.WriteLine(" MQTT connected,  BeginTest....");

            LoginService = new LoginClientServiceDelegate(MqttAdaptor);

            AskLogin();

            Console.WriteLine("Gate connected,  BeginTest....");

            RoutingRule.DistrictsName = "test";

            SceneService = new SceneClientServiceDelegate(GateAdaptor);
            ServiceImplementStub.Bind<IClientSceneImpl>(GateAdaptor, new SceneServiceImpl());

            Interoperate();
        }

        private void AskLogin()
        {
            string username;
            string password;
            while (true)
            {
                Console.WriteLine("Enter UserName: {1,2}\n");
                username = Console.ReadLine();
                Console.WriteLine("Enter Password: \n");
                password = Console.ReadLine();

                break;
            }

            LoginService.AskLogin(username, password, MqttAdaptor.GetUuid().ToByteArray()).Callback = askLoginOperation =>
             {
                 if (!askLoginOperation.IsComplete)
                 {
                     Console.WriteLine("LoginService.AskLogin failed");
                     return;
                 }

                 ServerList list = askLoginOperation.Result;

                 Console.WriteLine(list);

                 string prompt = string.Format("Chose one server:\n{0}\n"
                 , string.Join("\n", list.Servers.ToArray()));

                 if (list.Servers.Count == 0)
                 {
                     Console.WriteLine("No valid server....");
                     return;
                 }
                 while (true)
                 {
                     Console.WriteLine(prompt);
                     string s = Console.ReadLine();
                     if (list.Servers.Contains(s))
                     {
                         RoutingRule.DistrictsName = s;
                         break;
                     }
                     else if (s == "")
                     {
                         RoutingRule.DistrictsName = list.Servers[0];
                         break;
                     }

                 }

                 pid = list.Pid;
                 Console.WriteLine(string.Format("------------we are in {0}----------------", RoutingRule.DistrictsName));
                 LogicService = new LogicClientServiceDelegate(MqttAdaptor);
                 ServiceImplementStub.Bind<IClientLogicImpl>(MqttAdaptor, new ClientLogicServiceImpl());
                 Interoperate();
             };
        }

        private void Interoperate()
        {
            LogicService.RequestPlayerInfo(pid).Callback = RequestReturn;
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
                    if (!int.TryParse(inputs[0], out method) || (method < 0 || method > 4))
                    {
                        Console.WriteLine("1:AskChangeName 2:AskAddMoney 3:AskLearnSkill 4:AskMove");
                        continue;
                    }

                    SceneService.AskMoveTo(1, 1);

                    switch ((Method)method)
                    {
                        case Method.AskChangeName:
                            {
                                //var a = new TestBaseClass() {baseId = 1};
                                //var b = new TestDerived1Class() {baseId = 2, derived1Id = 3,};
                                //var c = new TestDerived2Class() {baseId = 4, derived2Id = 5,};
                                //var e = new TestDerived11Class() {baseId = 6, derived1Id = 7, derived11Id = 8};

                                LogicService.AskChangeName(pid, inputs[1]).Callback = operation =>
                                {
                                    if (!operation.IsComplete)
                                    {
                                        Console.WriteLine("LogicService.AskChangeName failed");
                                        return;
                                    }

                                    Console.WriteLine("LogicService.AskChangeName return ret={0}", operation.Result);
                                    LogicService.RequestPlayerInfo(pid).Callback = RequestReturn;
                                };
                                break;

                            }

                        case Method.AskAddMoney:
                            {
                                uint money = 0;
                                uint.TryParse(inputs[1], out money);

                                LogicService.AskAddMoney(pid, money).Callback = operation =>
                                {
                                    if (!operation.IsComplete)
                                    {
                                        Console.WriteLine("LogicService.AskAddMoney failed");
                                        return;
                                    }

                                    Console.WriteLine("LogicService.AskAddMoney return ret={0}", operation.Result);
                                    LogicService.RequestPlayerInfo(pid).Callback = RequestReturn;
                                };
                            }
                            break;
                        case Method.AskLearnSkill:
                            {
                                uint skillId = 0;
                                uint.TryParse(inputs[1], out skillId);

                                LogicService.AskLearnSkill(pid, skillId).Callback = operation =>
                                {
                                    if (!operation.IsComplete)
                                    {
                                        Console.WriteLine("LogicService.AskLearnSkill failed");
                                        return;
                                    }

                                    Console.WriteLine("LogicService.AskLearnSkill return ret={0}", operation.Result);
                                    LogicService.RequestPlayerInfo(pid).Callback = RequestReturn;
                                };
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
           ).Start();
        }

        private void RequestReturn(InvokeOperation<PlayerInfo> handle)
        {
            if (!handle.IsComplete)
            {
                Console.WriteLine("RequestReturn failed");
                return;
            }

            Console.WriteLine("RequestReturn info:");
            Console.WriteLine(handle.Result.ToString());
        }
    }
}
