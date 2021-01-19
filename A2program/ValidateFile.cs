using A1program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2program
{
    class ValidateFile
    {
        public Boolean Validate(string filename)
        {
            //This function accepts a file as a parameter and attempts
            //to validate it using the AT1 Validate Cloud File method.

            bool valid;

            if (ConfigReader.ValidateCloudFile(filename))
            {
                valid = true;
            }
            else
            {
                valid = false;
            }

            return valid;
        }
    }
}
