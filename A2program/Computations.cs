using A1program;
using CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A2program
{
    class Computations
    {
        double[,] runtimes = new double[ConfigReader.programProcessorAmount, ConfigReader.programTaskAmount];
        public double[] ComputeRuntimes()
        {
            // this function computes the runtimes of all tasks running
            // on all processors, stores these data values into a 2 dimensional
            // array of doubles and then converts into a single dimensional array
            // and returns that array.
            ViewAllocations viewalloc = new ViewAllocations();
            
            
            for (int i = 0; i < ConfigReader.programProcessorAmount; i++)
            {
                for (int j = 0; j < ConfigReader.programTaskAmount; j++)
                {         
                     runtimes[i, j] = viewalloc.RuntimeCalc(ConfigReader.tasks[j].Runtime, ConfigReader.referenceFreq, ConfigReader.processors[i].Frequency);                  
                }
            }                   
            return (TransformTo1D(runtimes));
        }
        
        public double[] ComputerEnergies()
        {
            // this function computes the energies of all tasks running on all processors,
            // stores these data values into a 2 dimensional array of doubles and then converts 
            // that array into a 1 dimensional array and returns that array.

            ViewAllocations viewalloc = new ViewAllocations();
            double[,] energies = new double[ConfigReader.programProcessorAmount, ConfigReader.programTaskAmount];

            for (int i = 0; i < ConfigReader.programProcessorAmount; i++)
            {
                for (int j = 0; j < ConfigReader.programTaskAmount; j++)
                {
                    energies[i, j] = viewalloc.TaskEnergyCalcAT2(ConfigReader.processors[i].Coefficients[2], ConfigReader.processors[i].Coefficients[1], ConfigReader.processors[i].Coefficients[0], ConfigReader.processors[i].Frequency);
                }
            }
            for (int i = 0; i < ConfigReader.programProcessorAmount; i++)
            {
                for (int j = 0; j < ConfigReader.programTaskAmount; j++)
                {
                    energies[i, j] *= runtimes[i, j];
                }
            }
          
            return (TransformTo1D(energies));
        }        
        
        public double[] TransformTo1D(double[,] data)
        {
            // this function converts a 2 dimensional array of runtimes values to
            // a 1 Dimensional version where the length is the sum of multiplying the
            // dimensions of the 2D array.

            int size = data.Length;            
            double[] data1D = new double[size];

            int write = 0;
            for (int i = 0; i <= data.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= data.GetUpperBound(1); j++)
                {
                    data1D[write++] = data[i, j];
                }
            }

            return (data1D);

        }
    }
}
