using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherService
{
    class Watcher
    {
        public Guid Uuid { get; set; }
        public HashSet<string> SubjectiveDownSet { get; private set; }

        public Watcher()
        {
            SubjectiveDownSet = new HashSet<string>();
        }
    }
}
