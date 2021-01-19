using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    public class ConfigurationData
    {
        public Double Duration { get; set; }
        public int NumberOfTasks { get; set; }
        public int NumberOfProcessors { get; set; }
        public Double[] TaskRam { get; set; }
        public Double[] ProcessorRam { get; set; }
        public Double[] Runtimes { get; set; }
        public Double[] Energies { get; set; }
        public Double[] LocalCommunication { get; set; }
        public Double[] RemoteCommunication { get; set; }
    }
}
