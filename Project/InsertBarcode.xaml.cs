using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Project
{
    /// <summary>
    /// Interaction logic for InsertBarcode.xaml
    /// </summary>
    public partial class InsertBarcode : Window
    {
        internal Barcode barcodeinfo;
        List<string>Types1D=new List<string>();
        List<string> Types2D = new List<string>();

        public InsertBarcode()
        {
            InitializeComponent();
            Types1D.Add("CODE_128");
            Types1D.Add("EAN_13");
            Types1D.Add("CODE_39");
            Types2D.Add("QR_CODE");
            Types2D.Add("DATA_MATRIX");
            Types2D.Add("AZTEC");


        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            barcodeinfo = new Barcode()
            {
                id = 0,
                Position = PositiontextBox.Text,
                Pages = pagetextBox.Text,
                BarcodeType = BarcodeTypecomboBox.Text,
                Barcode1D2D = Barcode1D2DcomboBox.Text,
                IsDrowText = RbNumberMood.IsChecked.Value,
            };

            this.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void Barcode1D2DcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem select =(ComboBoxItem) Barcode1D2DcomboBox.SelectedItem;
            
            if (select.Content.ToString() == "1D")
            {

                BarcodeTypecomboBox.ItemsSource =Types1D ;
            }
            if (select.Content.ToString() == "2D")
            {

                BarcodeTypecomboBox.ItemsSource = Types2D;
            }
        }
    }
}
