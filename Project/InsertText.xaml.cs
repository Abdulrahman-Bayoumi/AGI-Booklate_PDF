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
    /// Interaction logic for InsertText.xaml
    /// </summary>
    public partial class InsertText : Window
    {
        internal TextInformatiom textinfo;

        public InsertText()
        {
            InitializeComponent();
          
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            textinfo = new TextInformatiom()
            {
                id = 0,
                Position = PositiontextBox.Text,
                Pages = pagetextBox.Text,
                FontType = FontTypecomboBox.Text,
                FontSize = int.Parse(FontSizetextBox.Text),
                Fontcolor = FontcolorBox.Text,

            };

            this.Close();
        }
    }
}
