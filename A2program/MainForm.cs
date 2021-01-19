using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using A1program;
using CommonClasses;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;

namespace A2program
{
    public partial class A2MainForm : Form
    {
        List<string> allocations;
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        readonly object AllocationsLock = new object();
        int calls;
        int completedCalls;
        int WcfsCallsTimedout;

        public A2MainForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
                                                   
            ConfigReader.processors.Clear();
            ConfigReader.tasks.Clear();

            TestFunction test = new TestFunction();
            ValidateFile validate = new ValidateFile();
            PrepData predata = new PrepData();
            
            string filename = comboBox1.Text;


            // Checks to see if textbox is empty, if it is do nothing.
            if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrWhiteSpace(comboBox1.Text))
            {

            }
            // If check box isnt empty, attempts to validate the config file at the provided address.
            else
            {
                if (validate.Validate(filename))
                {                   
                    MessageBox.Show(test.OutputLines(filename));

                    // Executes the GenerateAllocations function on another Thread.
                    System.Threading.Tasks.Task.Run(() => CallWcfOperations(predata));

                    // GUI waits for 5 minutes or until all responses have finalised.
                    autoResetEvent.WaitOne(300000);

                    List<string> earlyAllocations = new List<string>(allocations);

                    MessageBox.Show("There were " + earlyAllocations.Count + " allocations generated in the given" +
                        "time frame.");
                    lock (AllocationsLock)
                    {
                        if (allocations.Count == 0)
                        {
                            richTextBox1.Text = "No allocations were found! All operations timed out!";
                        }
                        else
                        {
                            foreach (string response in earlyAllocations)
                            {
                                richTextBox1.Text = "\n" + response;
                            }
                        }
                        
                    }                                                                                                                                                                                                                                                       
                    
                                                                   
                }
                else
                {
                    MessageBox.Show("The configuration file is invalid, please see error list!");
                }
            }
            
        }
      
        private void CallWcfOperations(PrepData predata)
        {
            WCFSlocal1.ServiceClient wcfs2 = new WCFSlocal1.ServiceClient();
            AWSnanoAT2.ServiceClient nanoAT2 = new AWSnanoAT2.ServiceClient();
            AWSmicroAT2.ServiceClient microAT2 = new AWSmicroAT2.ServiceClient();
            AWSsmallAT2.ServiceClient smallAT2 = new AWSsmallAT2.ServiceClient();

            
            //wcfs2.GenerateAllocationsCompleted += Wcfs2_GenerateAllocationsCompleted1;
            nanoAT2.GenerateAllocationsCompleted += NanoAT2_GenerateAllocationsCompleted;
            microAT2.GenerateAllocationsCompleted += MicroAT2_GenerateAllocationsCompleted;
            smallAT2.GenerateAllocationsCompleted += SmallAT2_GenerateAllocationsCompleted;


            lock (AllocationsLock)
            {
                calls = 4;
                completedCalls = 0;
                int WCFServiceID = 1;
                allocations = new List<string>();

                for (int x = 0; x < 4; x++)
                {
                    //wcfs2.GenerateAllocationsAsync(predata.PrepareData(), WCFServiceID, 30000);
                    //WCFServiceID++;
                    nanoAT2.GenerateAllocationsAsync(predata.PrepareData(), WCFServiceID, 300000);
                    WCFServiceID++;
                    microAT2.GenerateAllocationsAsync(predata.PrepareData(), WCFServiceID, 300000);
                    WCFServiceID++;
                    smallAT2.GenerateAllocationsAsync(predata.PrepareData(), WCFServiceID, 300000);
                    WCFServiceID++;
                }
            }
            
        }

        private void SmallAT2_GenerateAllocationsCompleted(object sender, AWSsmallAT2.GenerateAllocationsCompletedEventArgs e)
        {
            // REMOTE SMALL CODE FOR TESTING.
            try
            {
                WcfsCompleted(allocations, e.Result);
            }
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                // local timeout.
                WcfsCallsTimedout++;
                WcfsCompletedAfter();
            }
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                // remote timeout.
                if (fex.Message.Equals("Algorithm timed out."))
                {
                    WcfsCallsTimedout++;
                }
                WcfsCompletedAfter();
            }
        }

        private void MicroAT2_GenerateAllocationsCompleted(object sender, AWSmicroAT2.GenerateAllocationsCompletedEventArgs e)
        {
            // REMOTE MICRO CODE FOR TESTING.
            try
            {
                WcfsCompleted(allocations, e.Result);
            }
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                // local timeout.
                WcfsCallsTimedout++;
                WcfsCompletedAfter();
            }
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                // remote timeout.
                if (fex.Message.Equals("Algorithm timed out."))
                {
                    WcfsCallsTimedout++;
                }
                WcfsCompletedAfter();
            }
        }

        private void NanoAT2_GenerateAllocationsCompleted(object sender, AWSnanoAT2.GenerateAllocationsCompletedEventArgs e)
        {
            // REMOTE NANO CODE FOR TESTING.
            try
            {
                WcfsCompleted(allocations, e.Result);
            }
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                // local timeout.
                WcfsCallsTimedout++;
                WcfsCompletedAfter();
            }
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                // remote timeout.
                if (fex.Message.Equals("Algorithm timed out."))
                {
                    WcfsCallsTimedout++;
                }
                WcfsCompletedAfter();
            }
        }

        private void Wcfs2_GenerateAllocationsCompleted1(object sender, WCFSlocal1.GenerateAllocationsCompletedEventArgs e)
        {
            // LOCAL WCF CODE FOR TESTING.
            try
            {
                WcfsCompleted(allocations, e.Result);
            }
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                // local timeout.
                WcfsCallsTimedout++;
                WcfsCompletedAfter();
            }
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                // remote timeout.
                if (fex.Message.Equals("Algorithm timed out."))
                {
                    WcfsCallsTimedout++;
                }
                WcfsCompletedAfter();
            }
        }

        public void WcfsCompleted(List<string> allocations, string allocation)
        {
            
            lock (AllocationsLock)
            {
                // add responses from the GenerateAllocations function from
                // the server side to our allocations list.
                completedCalls++;
                allocations.Add(allocation);
                // once all GenerateAllocations calls have completed,
                // stop GUI thread from waiting.
                if (completedCalls == calls)
                {
                    autoResetEvent.Set();
                }
            }
            
        }

        public void WcfsCompletedAfter()
        {
            // function to count responses after 5 minute timeout.
            lock (AllocationsLock)
            {
                completedCalls++;
                allocations.Add("late result - sendTimeout.");
                if (completedCalls == calls)
                {
                    autoResetEvent.Set();
                }
            }
        }

        private void Wcfs2_GenerateAllocationsCompleted(object sender, WCFSlocal1.GenerateAllocationsCompletedEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
        }
    }
}
