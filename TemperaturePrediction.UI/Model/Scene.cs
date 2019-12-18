using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.UI.Model
{
    public class Scene
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string Path { get; set; }

        public string Cloudity { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
