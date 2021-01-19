using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A2program
{
    public class TestFunction
    {
        public string OutputLines(string filename)
        {
            List<string> lines = new List<string>();
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(filename);
            StreamReader sr = new StreamReader(stream);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                lines.Add(line);
            }
            sr.Close();

            string combinedString = string.Join(System.Environment.NewLine, lines);
            return combinedString;
        }
    }
}
