using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Mvvm.POCO;

namespace MapDemo {
    public partial class WorldWeather : MapDemoModule, INotifyPropertyChanged {
        object selectedItem;

        public event PropertyChangedEventHandler PropertyChanged;

        public OpenWeatherMapService OpenWeatherMapService { get; set; }
        public object SelectedItem {
            get { return selectedItem; }
            set {
                if (selectedItem != value) {
                    selectedItem = value;
                    CityWeather cityWeatherInfo = selectedItem as CityWeather;
                    if (cityWeatherInfo != null && cityWeatherInfo.Forecast == null)
                        OpenWeatherMapService.GetForecastForCityAsync(cityWeatherInfo);
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
                }
            }
        }

        public WorldWeather() {
            InitializeComponent();
            OpenWeatherMapService = ViewModelSource.Create(() => new OpenWeatherMapService());
            DataContext = this;
            OpenWeatherMapService.GetWeatherAsync();
        }

        void lbUnitType_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e) {
            OpenWeatherMapService.SetCurrentTemperatureType((TemperatureScale)e.NewValue);
        }
    }

    public class NullObjectToVisibiltyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value == null) ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class SegmentColorizerConverter : MarkupExtension, IValueConverter {
        const string Fahrenheit = "Weather.FahrenheitTemperature";
        const string Celsius = "Weather.CelsiusTemperature";

        readonly DoubleCollection celsiusRangeStops = new DoubleCollection() { -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45 };
        readonly DoubleCollection fahrenheitRangeStops = new DoubleCollection() { -40, -31, -22, -13, -4, 5, 14, 23, 32, 41, 50, 59, 68, 77, 86, 94, 103, 112 };

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value.ToString() == Fahrenheit)
                return fahrenheitRangeStops;
            if (value.ToString() == Celsius)
                return celsiusRangeStops;
            return value;
        }
        object IValueConverter.ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
