using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace FolderGenViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddChildFileClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu contextMenu = item.Parent as ContextMenu;
            TextBlock textBlock = contextMenu.PlacementTarget as TextBlock;

            XmlElement element = textBlock.DataContext as XmlElement;
            XmlElement child = element.OwnerDocument.CreateElement("file");
            XmlAttribute attribute = child.OwnerDocument.CreateAttribute("name");
            attribute.Value = "File.txt";
            child.Attributes.Append(attribute);

            element.AppendChild(child);
            Debug.Print(element.OwnerDocument.OuterXml);
            Debug.Print(ViewModel.XmlData.Source.LocalPath);

            Debug.Print(element.ToString());
            //StackPanel stackPanel = textBlock.Parent as StackPanel;
            //Debug.Print(stackPanel.ToString());
            Debug.Print(sender.ToString());
            Debug.Print(e.RoutedEvent.ToString());
        }

        private void AddChildFolderClick(object sender, RoutedEventArgs e)
        {
            Debug.Print(sender.ToString());
        }

        private void AddChildReferenceClick(object sender, RoutedEventArgs e)
        {
            Debug.Print(sender.ToString());
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            Debug.Print(sender.ToString());
        }
    }

    public class ViewModel
    {
        public static XmlDataProvider XmlData { get; set; }

        public ViewModel()
        {
            XmlData = new XmlDataProvider();
            XmlDocument document = new XmlDocument();
            document.InnerXml = Properties.Resources.XMLFolderStructure;
            XmlData.Document = document;
            //XmlData.Source = new Uri("pack://siteoforigin:,,,/Resources/PythonXMLBasedGen/XMLFolderStructure.xml");
            //Debug.Print(File.Exists(XmlData.Source.LocalPath).ToString());
            XmlData.XPath = "folder";
        }
    }

    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class BitmapSourceConverter : IValueConverter
    {
        public BitmapSource File { get { return GetFromResources("file"); } }

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string resourceName = string.Empty;

            if (value.ToString().Contains('.'))
            {
                resourceName = value.ToString().Split('.')[1];
            }
            else
            {
                resourceName = value.ToString();
            }

            return GetFromResources(resourceName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        private BitmapSource GetFromResources(string resourceName)
        {
            Bitmap image = Properties.Resources.ResourceManager.GetObject(resourceName) as Bitmap;

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap
            (
                image.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            return bitmapSource;
        }
        #endregion
    }
}
