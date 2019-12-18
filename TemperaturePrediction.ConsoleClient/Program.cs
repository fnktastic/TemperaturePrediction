using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Data;
using TemperaturePrediction.Data.Helper;

namespace TemperaturePrediction.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataFiller = new DataFiller();

            var scene = dataFiller.BuildScene();
        }
    }
}
