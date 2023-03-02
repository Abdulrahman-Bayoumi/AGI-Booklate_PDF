using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for UpdatePageText.xaml
    /// </summary>
    public partial class UpdatePageText : Window
    {
        internal TextInformatiom viewitem; 
        public UpdatePageText(TextInformatiom viewitemm)
        {
            InitializeComponent();
            this.viewitem= viewitemm;
            pagetextBox.Text = viewitem.Pages;
            PositiontextBox.Text= viewitem.Position;
            FontSizetextBox.Text= Convert.ToString( viewitem.FontSize);
            FontcolorBox.Text = viewitem.Fontcolor;
            FontTypecomboBox.Text = viewitem.FontType;

        }

        private void updatetBtn_Click(object sender, RoutedEventArgs e)
        {
            viewitem.Position = PositiontextBox.Text;
            viewitem.Pages = pagetextBox.Text;
            viewitem.FontType = FontTypecomboBox.Text;
            viewitem.FontSize = int.Parse(FontSizetextBox.Text);
            viewitem.Fontcolor = FontcolorBox.Text;
            this.Close();

        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
