using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CommonClasses;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
    public string GenerateAllocations(ConfigurationData configData, int WCFServiceID, int deadline)
    {
        Stopwatch watch1 = new Stopwatch();
        watch1.Start();
        
        List<string> validAllocations = new List<string>();
        string bestAllocations;
        int AllocationID = 1;

        do
        {
            if (watch1.ElapsedMilliseconds >= deadline)
            {
                throw (new TimeoutException("No allocations were computed within the allowed time window, and the server timed out"));
            }
            else
            {
                // 2d runtimes array that is the result from converting the 1d runtimes back to 2d.
                double[,] runtimes = TransformTo2D(configData.Runtimes, configData.NumberOfProcessors, configData.NumberOfTasks);
                // sorts the runtimes array based on a random generated map for easier processing.
                double[,] sortedRuntimes = GenerateMap(runtimes, configData.NumberOfProcessors, configData.NumberOfTasks, configData.ProcessorRam, configData.TaskRam);
                // passing in our sorted runtimes array to our alogrithm functions to generate an allocation based on processor count.
                double[,] allocatedTasks;
                if (configData.NumberOfProcessors == 4)
                {
                    allocatedTasks = GreedyAlgoSmall(sortedRuntimes, configData.NumberOfProcessors, configData.NumberOfTasks, configData.Duration, configData.ProcessorRam, configData.TaskRam, runtimes);
                }
                else if (configData.NumberOfProcessors == 5)
                {
                    //allocatedTasks = GreedyAlgoLarge(sortedRuntimes, configData.NumberOfProcessors, configData.NumberOfTasks, configData.Duration, configData.ProcessorRam, configData.TaskRam, runtimes);
                    allocatedTasks = HeuristicAlgoLarge(sortedRuntimes, configData.NumberOfProcessors, configData.NumberOfTasks);
                }
                else
                {
                    allocatedTasks = GreedyAlgoLarge(sortedRuntimes, configData.NumberOfProcessors, configData.NumberOfTasks, configData.Duration, configData.ProcessorRam, configData.TaskRam, runtimes);
                }
                // passing in our generated allocation array to check if it satifys the limits of the config file.
                if (IsAllocationValid(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.TaskRam, configData.ProcessorRam))
                {
                    if (configData.NumberOfProcessors == 4)
                    {
                        double totalAllocationEnergy = AllocationEnergyCalc(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.Energies, configData.LocalCommunication, configData.RemoteCommunication);
                        double[] runtimesOfAllocation = AllocationRuntimes(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks);
                        if (totalAllocationEnergy < 500)
                        {
                            double[] allocationRamInfo = RAMinformationCalc(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.TaskRam, configData.ProcessorRam);
                            string onesAndZeroes = OnesAndZeroes(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks);
                            validAllocations.Add(AllocationStringBuilder(configData.NumberOfProcessors, totalAllocationEnergy, onesAndZeroes, allocationRamInfo, runtimesOfAllocation, configData.ProcessorRam, WCFServiceID, AllocationID));
                        }
                    }
                    else if (configData.NumberOfProcessors == 5)
                    {
                        double totalAllocationEnergy = AllocationEnergyCalc(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.Energies, configData.LocalCommunication, configData.RemoteCommunication);
                        double[] runtimesOfAllocation = AllocationRuntimes(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks);

                        if (totalAllocationEnergy < 1400)
                        {
                            double[] allocationRamInfo = RAMinformationCalc(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.TaskRam, configData.ProcessorRam);
                            string onesAndZeroes = OnesAndZeroes(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks);
                            validAllocations.Add(AllocationStringBuilder(configData.NumberOfProcessors, totalAllocationEnergy, onesAndZeroes, allocationRamInfo, runtimesOfAllocation, configData.ProcessorRam, WCFServiceID, AllocationID));
                        }
                    }
                    else if (configData.NumberOfProcessors == 6)
                    {
                        double totalAllocationEnergy = AllocationEnergyCalc(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.Energies, configData.LocalCommunication, configData.RemoteCommunication);
                        double[] runtimesOfAllocation = AllocationRuntimes(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks);

                        if (totalAllocationEnergy < 300)
                        {
                            double[] allocationRamInfo = RAMinformationCalc(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks, configData.TaskRam, configData.ProcessorRam);
                            string onesAndZeroes = OnesAndZeroes(allocatedTasks, configData.NumberOfProcessors, configData.NumberOfTasks);
                            validAllocations.Add(AllocationStringBuilder(configData.NumberOfProcessors, totalAllocationEnergy, onesAndZeroes, allocationRamInfo, runtimesOfAllocation, configData.ProcessorRam, WCFServiceID, AllocationID));
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                else
                {
                    continue;
                }
                AllocationID++;
            }

            

        } while (validAllocations.Count != 2);

        if (validAllocations.Count != 0)
        {
                bestAllocations = String.Join(Environment.NewLine, validAllocations);
        }
        else
        {
            bestAllocations = "There were no allocations computed within the allowed time frame!";
        }
                  
        

        
        return (bestAllocations);
    }
    public double[,] TransformTo2D(double[] runtimes, int rows, int columns)
    {
        int i = 0;
        double[,] runtimes2D = new double[rows, columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                runtimes2D[y, x] = runtimes[i];
                i++;
            }
        }
        return (runtimes2D);
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
    public void Randomise(double[,] map, int rows, int columns, double[] processorRam, double[] taskRam)
    {
        Random random = new Random((int)DateTime.Now.Ticks);

        // check to see if task has higher RAM requirement than
        // what the processor allows.
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                if (taskRam[columnIndex] > processorRam[rowIndex])
                {
                    map[rowIndex, columnIndex] = 1;
                }
            }
        }

        // function to randomise allowed task allocation positions.      
        int count = 0;
        while (count < rows * columns * 0.40)
        {
            int randomRow = random.Next(0, rows);
            int randomColumn = random.Next(0, columns);
            if (map[randomRow, randomColumn] == 0)
            {
                map[randomRow, randomColumn] = 2;
                count++;
            }
        }
              
        
    }
    public bool IsAllocationValid(double[,] allocatedTasks, int rows, int columns, double[] taskRam, double[] processorRam)
    {
        // this function checks a potential allocation to see if the RAM contraints are valid
        // as-well as if all the tasks have been allocated to a processor.
        
        bool valid = true;


        // this for loop block check to see if the tasks running on a given processor satisfy
        // the ram constraints of that processor.
        double processorRamUsage = 0;
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            processorRamUsage = 0;
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {               
                if (columnIndex == columns - 1)
                {
                    if (allocatedTasks[rowIndex, columnIndex] != 0)
                    {
                        processorRamUsage += taskRam[columnIndex];
                        if (processorRamUsage > processorRam[rowIndex])
                        {
                            valid = false;
                            return (valid);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (allocatedTasks[rowIndex, columnIndex] != 0)
                    {
                        processorRamUsage += taskRam[columnIndex];
                        if (processorRamUsage > processorRam[rowIndex])
                        {
                            valid = false;
                            return (valid);
                        }
                    }
                }
            }
        }

        // checks to see if all tasks have been allocated.
        int allTasksOk = 0;
        foreach (double allocation in allocatedTasks)
        {
            if (allocation != 0)
            {
                allTasksOk += 1;
            }
        }
        if (allTasksOk != columns)
        {          
            valid = false;
            return (valid);
        }




        return (valid);
    }
    public double[] RAMinformationCalc(double[,] data, int rows, int columns, double[] taskRam, double[] processorRam)
    {
        // array of strings that hold the Ram usage info of each processor for an allocation.
        double[] allocationRamInfo = new double[rows];

        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                if (data[rowIndex, columnIndex] == 0)
                {
                    continue;
                }
                else
                {
                    allocationRamInfo[rowIndex] += taskRam[columnIndex];
                }
            }
        }


        return (allocationRamInfo);
    }
    public double AllocationEnergyCalc(double[,] data, int rows, int columns, double[] energies, double[] localComms, double[] remoteComms)
    {
        double[,] localComms2D = TransformTo2D(localComms, rows, columns);
        double[,] remoteComms2D = TransformTo2D(remoteComms, rows, columns);
        double[,] energies2D = TransformTo2D(energies, rows, columns);
        double totalEnergy = 0;
        double[] taskEnergies = new double[columns * columns];
        double[] localEnergyValues = new double[columns * columns];
        double[] remoteEnergyValues = new double[columns * columns];

        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                if (data[rowIndex, columnIndex] == 0)
                {
                    energies2D[rowIndex, columnIndex] = 0;
                }
            }
        }
        taskEnergies = TransformTo1D(energies2D);
        double totalTaskEnergies = taskEnergies.Sum();


        //for loop block to calculate the local energy values for an allocation.
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                for (int columnIndex2 = 0; columnIndex2 < columns; columnIndex2++)
                {
                    if (data[rowIndex, columnIndex] == 1 && data[rowIndex, columnIndex2] == 1)
                    {
                        localEnergyValues[columnIndex] += localComms2D[rowIndex, columnIndex2];
                    }
                }
            }
        }
        double totalLocalEnergy = localEnergyValues.Sum();

        //for loop block to calculate the remote energy values for an allocation.
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                for (int columnIndex2 = 0; columnIndex2 < columns; columnIndex2++)
                {
                    if (data[rowIndex, columnIndex] == 1 && data[rowIndex, columnIndex2] == 0)
                    {
                        remoteEnergyValues[columnIndex] += remoteComms2D[rowIndex, columnIndex2];
                    }
                }
            }
        }
        double totalRemoteEnergy = remoteEnergyValues.Sum();

        totalEnergy = totalTaskEnergies + totalLocalEnergy + totalRemoteEnergy;

        return (totalEnergy);
    }
    public string OnesAndZeroes(double[,] data, int rows, int columns)
    {
        double[,] onesAndZeroes = new double[rows, columns];

        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                if (data[rowIndex, columnIndex] != 0)
                {
                    onesAndZeroes[rowIndex, columnIndex] = 1;
                }
            }
        }

        string onesAndZeroesString = "";
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                onesAndZeroesString += string.Format("{0}", onesAndZeroes[rowIndex, columnIndex]);
            }
            onesAndZeroesString += System.Environment.NewLine + System.Environment.NewLine;
        }

        return (onesAndZeroesString);
    }
    public double[,] HeuristicAlgoSmall(double[,] sortedRuntimes, int rows, int columns)
    {
        // this for loop block sorts the columns of the runtimes array by smallest value (heuristic).
        double[,] allocatedRuntimes = sortedRuntimes.Clone() as double[,];
        double min;
        int indexRow;
        int indexColumn;

        for (int column = 0; column < columns; column++)
        {
            min = 0;
            indexRow = 0;
            indexColumn = 0;
            for (int row = 0; row < rows; row++)
            {
                if (allocatedRuntimes[row, column] == 0)
                {
                    continue;
                }
                else if (allocatedRuntimes[row, column] != 0)
                {
                    if (min == 0)
                    {
                        min = allocatedRuntimes[row, column];                       
                        indexRow = row;
                        indexColumn = column;
                        continue;
                    }
                    else
                    {
                        if (allocatedRuntimes[row, column] > min)
                        {
                            allocatedRuntimes[row, column] = 0;
                            continue;
                        }
                        else if (allocatedRuntimes[row, column] < min)
                        {
                            min = allocatedRuntimes[row, column];
                            allocatedRuntimes[indexRow, indexColumn] = 0;
                        }
                    }
                }

            }
        }
      

        return (allocatedRuntimes);
    }
    public double[,] HeuristicAlgoLarge(double[,] sortedRuntimes, int rows, int columns)
    {
        // this for loop block sorts the columns of the runtimes array by smallest value (heuristic).
        double[,] allocatedRuntimes = sortedRuntimes.Clone() as double[,];
        double max;
        int indexRow;
        int indexColumn;

        for (int column = 0; column < columns; column++)
        {
            max = 0;
            indexRow = 0;
            indexColumn = 0;
            for (int row = 0; row < rows; row++)
            {
                if (allocatedRuntimes[row, column] == 0)
                {
                    continue;
                }
                else if (allocatedRuntimes[row, column] != 0)
                {
                    if (max == 0)
                    {
                        max = allocatedRuntimes[row, column];
                        indexRow = row;
                        indexColumn = column;
                        continue;
                    }
                    else
                    {
                        if (allocatedRuntimes[row, column] < max)
                        {
                            allocatedRuntimes[row, column] = 0;
                            continue;
                        }
                        else if (allocatedRuntimes[row, column] > max)
                        {
                            max = allocatedRuntimes[row, column];
                            allocatedRuntimes[indexRow, indexColumn] = 0;
                        }
                    }
                }

            }
        }


        return (allocatedRuntimes);
    }
    public double[,] GreedyAlgoSmall(double[,] sortedRuntimes, int rows, int columns, double programDuration, double[] processorRam, double[] taskRam, double[,] runtimes)
    {  
        double[,] runtimesToAllocate = sortedRuntimes.Clone() as double[,];
            
        double sumTotal = 0;
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = columns - 1; columnIndex >= 0; columnIndex--)
            {                            
                if ((sumTotal + runtimesToAllocate[rowIndex, columnIndex]) > programDuration)
                {
                    for (int x = columnIndex; x >= 0; x--)
                    {
                        runtimesToAllocate[rowIndex, x] = 0;
                    }
                    sumTotal = 0;
                    break;
                }
                else if (runtimesToAllocate[rowIndex, columnIndex] == 0)
                {
                    continue;
                }
                else
                {
                    sumTotal += runtimesToAllocate[rowIndex, columnIndex];
                    for (int y = rowIndex + 1; y < rows; y++)
                    {
                        runtimesToAllocate[y, columnIndex] = 0;
                    }

                }
            }
        }




            
                           
        return (runtimesToAllocate);
    }
    public double[,] GreedyAlgoLarge(double[,] sortedRuntimes, int rows, int columns, double programDuration, double[] processorRam, double[] taskRam, double[,] runtimes)
    {
        double[,] runtimesToAllocate = sortedRuntimes.Clone() as double[,];

        double sumTotal = 0;
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                if ((sumTotal + runtimesToAllocate[rowIndex, columnIndex]) > programDuration)
                {
                    for (int x = columnIndex; x < columns; x++)
                    {
                        runtimesToAllocate[rowIndex, x] = 0;
                    }
                    sumTotal = 0;
                    break;
                }
                else if (runtimesToAllocate[rowIndex, columnIndex] == 0)
                {
                    continue;
                }
                else
                {
                    sumTotal += runtimesToAllocate[rowIndex, columnIndex];
                    for (int y = rowIndex + 1; y < rows; y++)
                    {
                        runtimesToAllocate[y, columnIndex] = 0;
                    }

                }
            }
        }






        return (runtimesToAllocate);
    }
    public string AllocationStringBuilder(int numOfProcessors, double totalAllocationEnergy, string onesAndZeroes, double[] allocationRamInfo, double[] allocaitonRuntimes, double[] processorRam, int WCFServiceID, int AllocationID)
    {
        string data = null;

        if (numOfProcessors == 4)
        {
           
                data = string.Join(Environment.NewLine,
                  "WCF Service request ID: " + WCFServiceID,
                  "Allocation ID: " + AllocationID,
                  "Allocation is valid!",
                  "Processor 1 RAM usage was " + allocationRamInfo[0] + "/" + processorRam[0],
                  "Processor 2 RAM usage was " + allocationRamInfo[1] + "/" + processorRam[1],
                  "Processor 3 RAM usage was " + allocationRamInfo[2] + "/" + processorRam[2],
                  "Processor 4 RAM usage was " + allocationRamInfo[3] + "/" + processorRam[3],
                  " ",
                  onesAndZeroes,
                  "The runtime for processor 1 was: " + allocaitonRuntimes[0] + " seconds!",
                  "The runtime for processor 2 was: " + allocaitonRuntimes[1] + " seconds!",
                  "The runtime for processor 3 was: " + allocaitonRuntimes[2] + " seconds!",
                  "The runtime for processor 4 was: " + allocaitonRuntimes[3] + " seconds!",                 
                  "The total energy consumed by this allocation was: " + totalAllocationEnergy);

           
        }
        else if (numOfProcessors == 5)
        {
           
                data = string.Join(Environment.NewLine,
                  "WCF Service request ID: " + WCFServiceID,
                  "Allocation ID: " + AllocationID,
                  "Allocation is valid!",
                  "Processor 1 RAM usage was " + allocationRamInfo[0] + "/" + processorRam[0],
                  "Processor 2 RAM usage was " + allocationRamInfo[1] + "/" + processorRam[1],
                  "Processor 3 RAM usage was " + allocationRamInfo[2] + "/" + processorRam[2],
                  "Processor 4 RAM usage was " + allocationRamInfo[3] + "/" + processorRam[3],
                  "Processor 5 RAM usage was " + allocationRamInfo[4] + "/" + processorRam[4],
                  onesAndZeroes,
                  "The total energy consumed by this allocation was: " + totalAllocationEnergy);
           
        }
        else if (numOfProcessors == 6)
        {
           
                data = string.Join(Environment.NewLine,
                  "WCF Service request ID: " + WCFServiceID,
                  "Allocation ID: " + AllocationID,
                  "Allocation is valid!",
                  "Processor 1 RAM usage was " + allocationRamInfo[0] + "/" + processorRam[0],
                  "Processor 2 RAM usage was " + allocationRamInfo[1] + "/" + processorRam[1],
                  "Processor 3 RAM usage was " + allocationRamInfo[2] + "/" + processorRam[2],
                  "Processor 4 RAM usage was " + allocationRamInfo[3] + "/" + processorRam[3],
                  "Processor 5 RAM usage was " + allocationRamInfo[4] + "/" + processorRam[4],
                  "Processor 6 RAM usage was " + allocationRamInfo[5] + "/" + processorRam[5],
                  onesAndZeroes,
                  "The total energy consumed by this allocation was: " + totalAllocationEnergy);
            
        }
        return (data);
    }
    public int Testing(int milliseconds)
    {
        System.Threading.Thread.Sleep(milliseconds);
        return (milliseconds);
    }
    public double[,] GenerateMap(double[,] runtimes, int processors, int tasks, double[] processorRam, double[] taskRam)
    {
        // this function generates a random map of 0's, 1's and 2's
        // and then genrates a sorted runtimes array based on the generated map.
        double[,] map = new double[processors, tasks];
        Randomise(map, processors, tasks, processorRam, taskRam);

        double[,] runtimesToSort = runtimes.Clone() as double[,];

        // this double for loop block sorts the 2D runtimes array based on the random map
        // that was generated.
        for (int x = 0; x < processors; x++)
        {
            for (int y = 0; y < tasks; y++)
            {
                if (map[x, y] == 2)
                {
                    runtimesToSort[x, y] = 0;
                }
                else if (map[x, y] == 1)
                {
                    runtimesToSort[x, y] = 0;
                }
                else
                {
                    // dont change value as this runtime is potential.
                }
            }
        }

        return (runtimesToSort);
    }
    public double[] AllocationRuntimes(double[,] allocation, int processors, int tasks)
    {
        // this function adds up the runtimes of tasks running on processors for a  given allocation.

        double[] runtimesOfPRocessors = new double[processors];
        double totalRuntime = 0;
        for (int processor = 0; processor < processors; processor++)
        {           
            for (int task = 0; task < tasks; task++)
            {
                if (task == tasks - 1)
                {
                    totalRuntime += allocation[processor, task];
                    runtimesOfPRocessors[processor] = totalRuntime;
                    totalRuntime = 0;
                    break;
                }
                else if (allocation[processor, task] == 0)
                {
                    continue;
                }
                else
                {
                    totalRuntime += allocation[processor, task];
                }
            }
        }


        return (runtimesOfPRocessors);
    }

    }
        
