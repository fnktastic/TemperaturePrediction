using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.UI.Model
{
    public class Scene : ViewModelBase
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string Path { get; set; }

        public string Cloudity { get; set; }

        public DateTime TimeStamp { get; set; } 

        public TimeSpan? Time { get; set; }

        public string Metadata { get; set; }

        public List<Area> Areas { get; set; }

        public List<string> MetadataList
        {
            get
            {
                return File.ReadAllLines(Metadata).ToList();
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
