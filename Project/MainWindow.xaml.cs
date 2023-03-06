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
using Size = GemBox.Document.Size;
using SkiaSharp;
using ZXing;
using System.Drawing;
using System.ServiceModel.Syndication;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// Bayoumi
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
                id = 1,
                Position = "30-40",
                Pages = "1-2",
                BarcodeType = "CODE_128",
                Barcode1D2D = "1D",
                IsDrowText = true,
            });
            Texts.Add(new TextInformatiom()
            {
                id = 1,
                Position = "30-40",
                Pages = "1-2",
                Fontcolor = "#0000ff",
                FontSize = 10,
                FontType = "Times New Roman",
                IsOmrFont= true,
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

                    textPathexal = dlg.FileName;

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
            bool? RbNumberMoodVl = false;
            int from = 0;
            int to = 0;
            string path = null;
            int marged = 0;
            int cou = 0;
            this.Dispatcher.Invoke((() =>
            {
                RbNumberMoodVl = RbNumberMood.IsChecked;
                if (RbNumberMoodVl == true)
                {
                    from = int.Parse(txtFrom.Text);
                    to = int.Parse(txtTo.Text);
                }
                path = txtPath.Text;
                marged = int.Parse(txtmerged.Text);

            }));
            string Directory = System.IO.Path.GetDirectoryName(path);

            if (RbNumberMoodVl == true)
            {
                //list of all number from num1 to num2
                for (int xxx = from; xxx <= to; xxx++)
                {
                    listoftextbarcode.Add(Convert.ToString(xxx));
                  
                }
            }
            int total = listoftextbarcode.Count * 3;
            Directory = Directory + "Resulte";

            GemBox.Pdf.ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            var copydocument = GemBox.Pdf.PdfDocument.Load(path);
            
            for (int xxx = 1; xxx <= listoftextbarcode.Count; xxx++)
            {
                for (int index = 0; index < copydocument.Pages.Count; index++)
                {
                    var page = copydocument.Pages[index];
                    using (var formattedText = new PdfFormattedText())
                    {
                        foreach (var text in Barcodes)
                        {
                            string[] numbers = text.Pages.Split('-');
                            string[]size = text.Position.Split("-");
                            string d=text.Barcode1D2D.ToString();
                            string type = text.BarcodeType.ToString();
                            if (Enumerable.Range(int.Parse(numbers[0]) - 1, int.Parse(numbers[1])).Contains(index))
                            {
                                var format = new BarcodeFormat();
                                if (d == "1D")
                                {
                                    if (type == "CODE_128")
                                    {
                                        format = BarcodeFormat.CODE_128;

                                    }
                                    else if (type == "DATA_MATRIX")
                                    {
                                        format = BarcodeFormat.EAN_13;
                                    }
                                    else
                                    {
                                        format = BarcodeFormat.CODE_39;

                                    }
                                }
                                else if (d == "2D")
                                {
                                    if(type== "QR_CODE")
                                    {
                                        format = BarcodeFormat.QR_CODE;

                                    }else if(type == "DATA_MATRIX")
                                    {
                                        format = BarcodeFormat.DATA_MATRIX;
                                    }
                                    else
                                    {
                                        format = BarcodeFormat.AZTEC;

                                    }
                                }
                               
                              
                                var barcodeWriter = new ZXing.SkiaSharp.BarcodeWriter()
                                {

                                    Format = format,
                                    Options = new ZXing.Common.EncodingOptions
                                    {
                                        Height = 50,
                                        Width = 150,
                                        PureBarcode= !(text.IsDrowText),
                                    },

                                };

                                var bm = barcodeWriter
                                .Write(listoftextbarcode[xxx-1]);

                                using (var data = bm.Encode(SKEncodedImageFormat.Png, 80))
                                using (var stream = File.OpenWrite("out.jpg"))
                                {
                                    // save the data to a stream
                                    data.SaveTo(stream);
                                }
                                var img = PdfImage.Load("out.jpg");

                               
                                double x =double.Parse(size[0]) , y = page.CropBox.Top - double.Parse( size[1] ) - img.Size.Height;

                                // Draw the image to the page.
                                page.Content.DrawImage(img, new PdfPoint(x, y));
                           
                            }
                        }

                    }
                }
                System.IO.Directory.CreateDirectory(Directory);
                copydocument.Save(Directory +"\\"+ xxx + ".pdf");
                copydocument.Close();
                cou++;
                int percents = (cou * 100) / total;

                worker.ReportProgress(100, percents);
            }
            for (int xxx = 1; xxx <= listoftextbarcode.Count; xxx++)
            {
                copydocument = GemBox.Pdf.PdfDocument.Load(Directory +"\\"+ xxx + ".pdf");

                for (int index = 0; index < copydocument.Pages.Count; index++)
                {
                    var page = copydocument.Pages[index];
                    using (var formattedText = new PdfFormattedText())
                    {
                        foreach (var text in Texts)
                        { 
                            string[] numbers = text.Pages.Split('-');
                            string[] position = text.Position.Split('-');
                            var rgb = System.Drawing.ColorTranslator.FromHtml(text.Fontcolor);
                            if (text.IsOmrFont == true && int.TryParse(listoftextbarcode[xxx - 1], out int value))
                            {
                                string newvalue=string.Empty;
                                value = int.Parse(listoftextbarcode[xxx - 1]);
                                string binary = Convert.ToString(value, 2);

                                if (text.FontType== "Arabic OMR")
                                {
                                    newvalue = binary.Replace("0", "¹ ");
                                    newvalue = newvalue.Replace("1", "² ");
                                    if (Enumerable.Range(int.Parse(numbers[0]) - 1, int.Parse(numbers[1])).Contains(index))
                                    {
                                        formattedText.Color = PdfColor.FromRgb(rgb.R, rgb.G, rgb.B);

                                        formattedText.FontFamily = new PdfFontFamily("Resources",text.FontType);
                                        formattedText.FontSize = text.FontSize;
                                        double x = double.Parse(position[0]), y = page.CropBox.Top - double.Parse(position[1]) - formattedText.Height;
                                        formattedText.AppendLine(newvalue);

                                        page.Content.DrawText(formattedText, new PdfPoint(x, y));

                                    }


                                }
                                else if (text.FontType== "OMRBubblesLCextended")
                                {
                                    newvalue = binary.Replace("0", "` ");
                                    newvalue = newvalue.Replace("1", "~ ");
                                    if (Enumerable.Range(int.Parse(numbers[0]) - 1, int.Parse(numbers[1])).Contains(index))
                                    {
                                        formattedText.Color = PdfColor.FromRgb(rgb.R, rgb.G, rgb.B);

                                        formattedText.FontFamily = new PdfFontFamily("Resources",text.FontType);
                                        formattedText.FontSize = text.FontSize;
                                        double x = double.Parse(position[0]), y = page.CropBox.Top - double.Parse(position[1]) - formattedText.Height;
                                        formattedText.AppendLine(newvalue);

                                        page.Content.DrawText(formattedText, new PdfPoint(x, y));

                                    }

                                }
                                else
                                {
                                    if (Enumerable.Range(int.Parse(numbers[0]) - 1, int.Parse(numbers[1])).Contains(index))
                                    {
                                        formattedText.Color = PdfColor.FromRgb(rgb.R, rgb.G, rgb.B);

                                        formattedText.FontFamily = new PdfFontFamily(text.FontType);
                                        formattedText.FontSize = text.FontSize;
                                        double x = double.Parse(position[0]), y = page.CropBox.Top - double.Parse(position[1]) - formattedText.Height;
                                        formattedText.AppendLine(listoftextbarcode[xxx - 1]);

                                        page.Content.DrawText(formattedText, new PdfPoint(x, y));

                                    }

                                }

                            }
                            else
                            {
                                if (Enumerable.Range(int.Parse(numbers[0]) - 1, int.Parse(numbers[1])).Contains(index))
                                {
                                    formattedText.Color = PdfColor.FromRgb(rgb.R, rgb.G, rgb.B);

                                    formattedText.FontFamily = new PdfFontFamily(text.FontType);
                                    formattedText.FontSize = text.FontSize;
                                    double x = double.Parse(position[0]), y = page.CropBox.Top - double.Parse(position[1]) - formattedText.Height;
                                    formattedText.AppendLine(listoftextbarcode[xxx - 1]);

                                    page.Content.DrawText(formattedText, new PdfPoint(x, y));

                                }

                            }

                        }

                    }
                }
                copydocument.Save();
                copydocument.Close();
                cou++;
                int percents = (cou * 100) / total;

                worker.ReportProgress(100, percents);
            }
            for (int k = 1; k <= listoftextbarcode.Count; k = k + marged)
            {
                using (PdfDocument outPdf = new PdfDocument())
                {
                    for (int p = k; p < k + marged; p++)
                    {
                        using (PdfDocument one = PdfReader.Open(Directory+"\\" + p + ".pdf", PdfDocumentOpenMode.Import))

                            CopyPages(one, outPdf);
                    }


                    outPdf.Save(Directory+"\\"  + k + " new_filemarged.pdf");
                }
                cou++;
                int percents = (cou * 100) / total;

                worker.ReportProgress(100, percents);
            }


            void CopyPages(PdfDocument from, PdfDocument to)
            {
                for (int i = 0; i < from.PageCount; i++)
                {
                    to.AddPage(from.Pages[i]);
                }
            }

            for(int y = 1; y <= listoftextbarcode.Count; y++)
            {
                System.IO.File.Delete(Directory + "\\" + y + ".pdf");
            }

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                LoadingText.Foreground = new SolidColorBrush( System.Windows.Media.Color.FromRgb(0, 128, 0));
                LoadingText.Content = "Completing";

                //LoadingText.Visibility = Visibility.Collapsed;
                LoadingShape.Visibility = Visibility.Collapsed;

            }));
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                LoadingText.Content = "Generating " + e.UserState + " : " + e.ProgressPercentage;


            }));
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
            LoadingText.Visibility = Visibility.Visible;
            LoadingShape.Visibility = Visibility.Visible;
           
            worker.RunWorkerAsync();


        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            RbNumberMood.IsChecked = true;

        }


        private void btnInsert_Click_1(object sender, RoutedEventArgs e)
        {
            InsertBarcode Ipage = new InsertBarcode();
            Ipage.ShowDialog();
            var item = Ipage.barcodeinfo;
            if(item != null)
            {
                if(Barcodes.Count> 0)
                {
                    item.id = Barcodes.LastOrDefault().id + 1;

                }
                else
                {
                    item.id = 1;

                }
                Barcodes.Add(item);
              
            }
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
            if (itemm != null)
            {
                Barcode Member = Barcodes.Where(m => m.id == item.id).Single();
                Member.Position = itemm.Position;
                Member.Barcode1D2D = itemm.Barcode1D2D;
                Member.IsDrowText = itemm.IsDrowText;
                Member.BarcodeType = itemm.BarcodeType;
                Member.Pages = itemm.Pages;

            }

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
            if (itemm != null)
            {
                TextInformatiom Member = Texts.Where(m => m.id == item.id).Single();
                Member.Position = itemm.Position;
                Member.Fontcolor = itemm.Fontcolor;
                Member.FontType = itemm.FontType;
                Member.FontSize = itemm.FontSize;
                Member.Pages = itemm.Pages;
            }

                
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
            if (item !=null)
            {
                if (Texts.Count == 0)
                {
                    item.id = 1;

                }
                else
                {
                    item.id = Texts.LastOrDefault().id + 1;
                }
                Texts.Add(item);
            }
           
            TextDataGrid.ItemsSource = null;
            TextDataGrid.ItemsSource = Texts;
        }

        private void btnTemplate_Click(object sender, RoutedEventArgs e)
        {
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            string pathfile = System.IO.Path.Combine(startupPath, "template.xlsx");
            var dlg = new Microsoft.Win32.SaveFileDialog();
            if (dlg.ShowDialog() == true)
            {
                File.Copy(pathfile, dlg.FileName, true);
            }
        }
    }
}
