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

        public InsertBarcode()
        {
            InitializeComponent();
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
    }
}
