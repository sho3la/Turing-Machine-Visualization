using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineSimulation
{
    class LoadFile
    {

        public List<string> commands;

       public LoadFile()
       {
           commands = new List<string>();
       }


        public void fileLoad(string name)
        {

            string[] lines = System.IO.File.ReadAllLines(name);

            for (int i = 0; i < lines.Length; ++i)
            {
                commands.Add(lines[i]);
            }

        }

    }
}
