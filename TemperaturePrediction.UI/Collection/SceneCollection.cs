using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.UI.Model;
using TemperaturePrediction.UI.ViewModel;

namespace TemperaturePrediction.UI.Collection
{
    public class SceneCollection : ObservableCollection<Scene>
    {
        public SceneCollection(IList<Scene> scenes) : base(scenes)
        {

        }
    }
}
