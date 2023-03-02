using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using ExcelDataReader;
using DevExpress.ClipboardSource.SpreadsheetML;
using Bytescout.Spreadsheet;
using Bytescout.Spreadsheet.COM;
using DevExpress.XtraExport.Implementation;
using System.Diagnostics.Metrics;
using System.Windows.Forms;
using DevExpress.DocumentView;
using GemBox.Document;
using GemBox.Pdf.Objects;
using ComponentInfo = GemBox.Pdf.ComponentInfo;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using System.Runtime.CompilerServices;
using System.Threading;
using System.ComponentModel;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string textPathexal;
        public List<string> listoftextbarcode = new List<string>();
        public List<Barcode> Barcodes = new List<Barcode>();
        public List<TextInformatiom> Texts = new List<TextInformatiom>();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        int rowIndex;
        public MainWindow()
        {
            Barcodes.Add(new Barcode()
            {
                id=1,
                Position="30-40",
                Pages="1-2",
                BarcodeType="Barcode 128",
                Barcode1D2D="1D",
                IsDrowText=true,
            });
            Texts.Add(new TextInformatiom()
            {
                id = 1,
                Position = "30-40",
                Pages = "1-2",
                Fontcolor = "Gray",
                FontSize = 10,
                FontType = "Times New Roman",
            });

            InitializeComponent();
            BarcodeDataGrid.ItemsSource = Barcodes;
            TextDataGrid.ItemsSource = Texts;

            worker.WorkerReportsProgress = true;

            worker.WorkerSupportsCancellation = true;

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += backgroundWorker_ProgressChanged;
        }
     
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog(); 
           
            dlg.Filter = "Pdf Files|*.pdf";
            //Open the Pop-Up Window to select the file 
            if (dlg.ShowDialog() == true)
            {
                new FileInfo(dlg.FileName);
                using (Stream s = dlg.OpenFile())
                {
                    TextReader reader = new StreamReader(s);
                    string st = reader.ReadToEnd();

                    txtPath.Text = dlg.FileName;
                }
            }
        }

        private void btnBrowes_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            
            dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            //Open the Pop-Up Window to select the file 
            if (dlg.ShowDialog() == true)
            {
                new FileInfo(dlg.FileName);
                using (Stream s = dlg.OpenFile())
                {
                   
                    textPathexal=dlg.FileName;
                    
                }
                Spreadsheet document = new Spreadsheet();
                document.LoadFromFile(textPathexal);
                Bytescout.Spreadsheet.Worksheet worksheet = document.Workbook.Worksheets.ByName("sheet1");


                for (int j = 1; j < 11; j++)
                {
                    listoftextbarcode.Add(worksheet.Cell(j, 0).ToString());
                }

                document.Close();
            }
        }

        //excel read method
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 200; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                worker.ReportProgress(i);
                LoadingShape.Visibility = Visibility.Visible;
                LoadingText.Visibility = Visibility.Visible;
                Thread.Sleep(1000);
                e.Result = 1000;
            }
            
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
            LoadingText.Visibility = Visibility.Collapsed;
            LoadingShape.Visibility = Visibility.Collapsed;

            }));
        }
        static void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void RbNumberMod_Checked(object sender, RoutedEventArgs e)
        {
            if (!RbNumberMood.IsChecked == true)
            {
                btnBrowseExal.IsEnabled = true;
                btnTemplate.IsEnabled = true;
                txtFrom.IsEnabled = false;
                txtTo.IsEnabled = false;
            }
            else
            {
                btnBrowseExal.IsEnabled = false;
                btnTemplate.IsEnabled = false;
                txtFrom.IsEnabled = true;
                txtTo.IsEnabled = true;

            }
        }

        private void RbFileMood_Checked(object sender, RoutedEventArgs e)
        {
         
            

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }

        
       

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {

            worker.RunWorkerAsync();

            if (RbNumberMood.IsChecked == true)
            {
                //list of all number from num1 to num2
                for (int xxx = int.Parse(txtFrom.Text); xxx <= int.Parse(txtTo.Text); xxx++)
                {
                    listoftextbarcode.Add(Convert.ToString(xxx));

                } 
            }
            GemBox.Document.ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            var document = DocumentModel.Load(txtPath.Text, LoadOptions.PdfDefault);
               document.Save("temp.pdf");
            GemBox.Pdf.ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            var copydocument = GemBox.Pdf.PdfDocument.Load("temp.pdf");
            for (int xxx = 1;  xxx <= listoftextbarcode.Count; xxx++)
                {
                document = DocumentModel.Load(txtPath.Text, LoadOptions.PdfDefault);
                for (int index = 0; index < copydocument.Pages.Count; index++)
                    {
                        var page = copydocument.Pages[index];
                        using (var formattedText = new PdfFormattedText())
                        {
                           foreach (var text in Barcodes)
                           {
                            string[] numbers = text.Pages.Split('-');

                            if (Enumerable.Range(int.Parse(numbers[0])-1, int.Parse(numbers[1])).Contains(index))
                            {
                                var code128 = CreateBarcodeField(
                                document,
                                barcodeType: "Code128",
                                barcodeValue: listoftextbarcode[xxx-1],
                                showLabel:text.IsDrowText
                                //foregroundColor: "0xff7225",
                                //backgroundColor: "0x25b2ff"
                                );
                                document.Sections.Add(
                                       new GemBox.Document.Section(document,
                                         new GemBox.Document.Paragraph(document, "Abdulrahman Code 128:"),
                                        new GemBox.Document.Paragraph(document, code128)));
                            }
                                 }

                        }
                    }
                document.Save("E:\\Project\\copies\\" + xxx + ".pdf");
            }
            for (int xxx = 1; xxx <=listoftextbarcode.Count; xxx++)
            {
                copydocument = GemBox.Pdf.PdfDocument.Load("E:\\Project\\copies\\" + xxx + ".pdf");

                for (int index = 0; index < copydocument.Pages.Count; index++)
                {
                    var page = copydocument.Pages[index];
                    using (var formattedText = new PdfFormattedText())
                    {
                        foreach (var text in Texts)
                        {
                            string[] numbers = text.Pages.Split('-');
                            string[] position = text.Position.Split('-');
                            if (Enumerable.Range(int.Parse(numbers[0])-1, int.Parse(numbers[1])).Contains(index))
                            {
                                // Set font family and size.
                                // All text appended next uses the specified font family and size.

                                // Set the location of the bottom-left corner of the text.
                                // We want top-left corner of the text to be at location (100, 100)
                                // from the top-left corner of the page.
                                // NOTE: In PDF, location  (0, 0) is at the bottom-left corner of the page
                                // and the positive y axis extends vertically upward.
                                // Draw text to the page.
                                var c = text.Fontcolor;
                                formattedText.FontFamily = new PdfFontFamily(text.FontType);
                                formattedText.FontSize = text.FontSize;
                                double x = double.Parse(position[0]), y = page.CropBox.Top - double.Parse(position[1]) - formattedText.Height;
                                formattedText.AppendLine(listoftextbarcode[xxx-1]);
                                page.Content.DrawText(formattedText, new PdfPoint(x, y));

                                //formattedText.Color= PdfColor.FromRgb(text.Fontcolor); 
                            }
                        }

                    }
                }
                copydocument.Save();
                copydocument.Close();

            }

            static Field CreateBarcodeField(DocumentModel document, string barcodeType, string barcodeValue,
    int? heightInPoints = null, string foregroundColor = null,
    string backgroundColor = null, bool showLabel = false)
            {
                var instructionText = new StringBuilder();
                instructionText.Append(barcodeValue).Append(' ').Append(barcodeType);

                if (heightInPoints.HasValue)
                    instructionText.Append(" \\h ").Append(LengthUnitConverter.Convert(heightInPoints.Value, LengthUnit.Point, LengthUnit.Twip));
                if (foregroundColor != null)
                    instructionText.Append(" \\f ").Append(foregroundColor);
                if (backgroundColor != null)
                    instructionText.Append(" \\b ").Append(backgroundColor);
                if (showLabel)
                    instructionText.Append(" \\t");

                return new Field(document, FieldType.DisplayBarcode, instructionText.ToString());
            }
            for(int k=1;k<=listoftextbarcode.Count;k=k+int.Parse(txtmerged.Text))
            {
                using (PdfDocument outPdf = new PdfDocument())
                {
                    for(int p=k; p<k+ int.Parse(txtmerged.Text); p++)
                    {
                        using (PdfDocument one = PdfReader.Open("E:\\Project\\copies\\" + p + ".pdf", PdfDocumentOpenMode.Import))

                            CopyPages(one, outPdf);
                    }
                 

                    outPdf.Save("E:\\Project\\copies\\"+k+"newfilemarged.pdf");
                }
            }
           

            void CopyPages(PdfDocument from, PdfDocument to)
            {
                for (int i = 0; i < from.PageCount; i++)
                {
                    to.AddPage(from.Pages[i]);
                }
            }
           
            

        }



        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            RbNumberMood.IsChecked= true;
            
        }

     
        private void btnInsert_Click_1(object sender, RoutedEventArgs e)
        {
            InsertBarcode Ipage = new InsertBarcode();
            Ipage.ShowDialog();
            var item = Ipage.barcodeinfo;
            item.id=Barcodes.LastOrDefault().id+1;
            Barcodes.Add(item);
            BarcodeDataGrid.ItemsSource = null;
            BarcodeDataGrid.ItemsSource = Barcodes;

        }
        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button SelectedButton = (System.Windows.Controls.Button)sender;
            Barcode item = (Barcode)SelectedButton.DataContext;
            UpdatePage Upage = new UpdatePage(item);
            Upage.ShowDialog();
            Barcode itemm = Upage.upitem;
           Barcode Member = Barcodes.Where(m => m.id == item.id).Single();
            Member.Position = itemm.Position;
            Member.Barcode1D2D=itemm.Barcode1D2D;
            Member.IsDrowText=itemm.IsDrowText;
            Member.BarcodeType=itemm.BarcodeType;
            Member.Pages=itemm.Pages;

            BarcodeDataGrid.ItemsSource = null;

            BarcodeDataGrid.ItemsSource = Barcodes;
        }
        private void updatetextBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button SelectedButton = (System.Windows.Controls.Button)sender;
            TextInformatiom item = (TextInformatiom)SelectedButton.DataContext;
            UpdatePageText Upage = new UpdatePageText(item);
            Upage.ShowDialog();
            TextInformatiom itemm = Upage.viewitem;
            TextInformatiom Member = Texts.Where(m => m.id == item.id).Single();
            Member.Position = itemm.Position;
            Member.Fontcolor = itemm.Fontcolor;
            Member.FontType = itemm.FontType;
            Member.FontSize = itemm.FontSize;
            Member.Pages = itemm.Pages;
            TextDataGrid.ItemsSource = null;
            TextDataGrid.ItemsSource = Texts;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button SelectedButton = (System.Windows.Controls.Button)sender;
            Barcode item = (Barcode)SelectedButton.DataContext;
            
            //var deleteMember = Barcodes.Where(m => m.id == rowIndex).Single();
            Barcodes.Remove(item);
            BarcodeDataGrid.ItemsSource = null;

            BarcodeDataGrid.ItemsSource = Barcodes;
       

        }
        private void deletetextBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button SelectedButton = (System.Windows.Controls.Button)sender;
            TextInformatiom item = (TextInformatiom)SelectedButton.DataContext;

            //var deleteMember = Barcodes.Where(m => m.id == rowIndex).Single();
            Texts.Remove(item);
            TextDataGrid.ItemsSource = null;
            TextDataGrid.ItemsSource = Texts;


        }

        private void btnInserttext_Click(object sender, RoutedEventArgs e)
        {
            InsertText Ipage = new InsertText();
            Ipage.ShowDialog();
            var item = Ipage.textinfo;
            item.id = Texts.LastOrDefault().id + 1;
            Texts.Add(item);
            TextDataGrid.ItemsSource = null;
            TextDataGrid.ItemsSource = Texts;
        }
    }
}
