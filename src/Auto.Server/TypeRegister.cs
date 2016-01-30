using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace Auto.Server
{
    public static class TypeRegister
    {
        public static void Init()
        {
            var t = new Register4AltSerializer();

            t.RegisterAssembly(Assembly.GetAssembly(typeof(PlayerInfo)));

            t.Register(typeof(PlayerInfoSkill), 1);
            t.Register(typeof(PlayerInfoItem), 2);
            t.Register(typeof(TestBaseClass), 3);
            t.Register(typeof(TestDerived1Class), 4);
            t.Register(typeof(TestDerived2Class), 5);
            t.Register(typeof(TestDerived11Class), 6);

            // add type registeration above
            //register.RegisterEnum<PkMode>(23);
        }
    }
}
