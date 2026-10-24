using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Logger
{
    public class FileStreamLogger
    {
        public void AuthenticateAndWrite(string path, string remarks)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    string animal = "cat";
                    int size = 12;
                    // Use string interpolation syntax to make code clearer.
                    writer.WriteLine($"The {animal} is {size} pounds.");
                }
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }
    }
}
