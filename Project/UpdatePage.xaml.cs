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
    /// Interaction logic for UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : Window
    {
       internal Barcode upitem;

        public UpdatePage(Barcode memberId)
        {
            InitializeComponent();
            upitem = memberId;
            pagetextBox.Text = upitem.Pages;
            PositiontextBox.Text= upitem.Position;
            Barcode1D2DcomboBox.Text = upitem.Barcode1D2D;
            BarcodeTypecomboBox.Text = upitem.BarcodeType;
            RbNumberMood.IsChecked = upitem.IsDrowText;
        }

        private void updatetBtn_Click(object sender, RoutedEventArgs e)
        {
            upitem.Position = PositiontextBox.Text;
            upitem.Pages = pagetextBox.Text;
            upitem.BarcodeType = BarcodeTypecomboBox.Text;
            upitem.Barcode1D2D = Barcode1D2DcomboBox.Text;
            upitem.IsDrowText = RbNumberMood.IsChecked.Value;
            this.Close();

        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
