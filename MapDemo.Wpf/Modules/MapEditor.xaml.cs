using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Dialogs;
using DevExpress.Xpf.Map;

namespace MapDemo {
    public partial class MapEditor : MapDemoModule {
        readonly Dictionary<Type, long> itemIndexes = new Dictionary<Type, long>();
        public MapEditor() {
            InitializeComponent();
            DataContext = this;
        }
        string GenerateName(MapItem item) {
            Type itemType = item.GetType();
            if(!itemIndexes.ContainsKey(itemType))
                itemIndexes[itemType] = 0;
            itemIndexes[itemType]++;
            return string.Format("{0} {1}", itemType.Name, itemIndexes[itemType]);
        }
        public void OnShapesLoaded(List<MapItem> items) {
            items[0].EnableHighlighting = false;
            items[0].IsHitTestVisible = false;
            Editor.ActiveItems = new ObservableCollection<MapItem>() { items[50] };
        }
        public void OnColorEditValueChanged() {
            foreach(MapItem item in Editor.ActiveItems)
                PrepareItem(item);
        }
        public void PrepareItem(MapItem item) {
            item.Attributes.Add(new MapItemAttribute() { Name = "name", Value = GenerateName(item) });
            MapShapeBase shape = item as MapShapeBase;
            if(shape != null) {
                shape.Fill = new SolidColorBrush(fillColorEdit.Color);
                shape.Stroke = new SolidColorBrush(strokeColorEdit.Color);
            }
        }
        public void Export() {
            using(var dialog = new DXSaveFileDialog()) {
                dialog.Filter = "KML files|*.kml";
                dialog.CreatePrompt = true;
                dialog.OverwritePrompt = true;
                if(dialog.ShowDialog().Value) {
                    Editor.ActiveLayer.ExportToKml(dialog.FileName);
                    DXMessageBox.Show(this, string.Format("Items successfully exported to {0} file", dialog.FileName), "Info",
                      MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }

    public class SelectedItemToPerimeterConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            MapShape shape = value as MapShape;
            return shape != null ? Math.Round(GeoUtils.CalculateStrokeLength(shape), 3) + " m" : string.Empty;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class SelectedItemToAreaConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            MapShape shape = value as MapShape;
            return shape != null ? Math.Round(GeoUtils.CalculateArea(shape), 3) + " m\xB2" : string.Empty;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class SelectedItemToDiameterConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            MapDot dot = value as MapDot;
            return dot != null ? (int)dot.Size + " dip" : string.Empty;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class FontHeightToRectangleSizeConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)((double)value / 2);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class EditorModeToToolTipEnabledConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class ItemToolTipTemplateConverter : MarkupExtension, IValueConverter {
        public DataTemplate ShapeTemplate { get; set; }
        public DataTemplate DotTemplate { get; set; }
        public DataTemplate PinTemplate { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is MapDot)
                return DotTemplate;
            return value is MapShape ? ShapeTemplate : PinTemplate;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
