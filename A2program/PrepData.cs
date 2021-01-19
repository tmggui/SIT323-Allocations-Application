using A1program;
using CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A2program
{
    public class PrepData
    {
        public ConfigurationData PrepareData()
        {
            // creating an object of configdata dll file for data transfer to the AWS machines.
            Computations comp1 = new Computations();
            ConfigurationData configdata1 = new ConfigurationData();            
            // assigning the program duration value.
            configdata1.Duration = ConfigReader.programDuration;
            // assigning the program task amount.
            configdata1.NumberOfTasks = ConfigReader.programTaskAmount;
            // assigning the program processor amount.
            configdata1.NumberOfProcessors = ConfigReader.programProcessorAmount;
            // assigning the values of the TaskRam array using a for loop.
            configdata1.TaskRam = new double[configdata1.NumberOfTasks];
            for (int i = 0; i < ConfigReader.tasks.Count; i++)
            {
                configdata1.TaskRam[i] = ConfigReader.tasks[i].RamRequirement;
            }
            // assiging the values of the processor ram array using a for loop.
            configdata1.ProcessorRam = new double[configdata1.NumberOfProcessors];
            for (int i = 0; i < ConfigReader.processors.Count; i++)
            {
                configdata1.ProcessorRam[i] = ConfigReader.processors[i].RAM;
            }
            // assigning the values of the computed runtimes to the config data Runtimes array for transfer.
            configdata1.Runtimes = comp1.ComputeRuntimes();
            // assigning the vales of the computed energies to the config data Energies array for transfer.
            configdata1.Energies = comp1.ComputerEnergies();
            // transforming the 2d local comms array and assigning to the configdata local array for transfer.
            configdata1.LocalCommunication = comp1.TransformTo1D(ConfigReader.localCommunication);
            //Array.Copy(comp1.Transform(ConfigReader.localCommunication), configdata1.LocalCommunication, 0);
            // transforming the 2d remote comms array and assigning to the configdata remote array for transfer.
            configdata1.RemoteCommunication = comp1.TransformTo1D(ConfigReader.remoteCommunication);
           
            return (configdata1);
        }
     
    }
}
