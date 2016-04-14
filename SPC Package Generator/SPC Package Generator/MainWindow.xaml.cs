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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SPC_Package_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> filepaths = new List<String>();

        public MainWindow()
        {
            InitializeComponent();

            //Add data types to combo box.
            foreach (var item in Enum.GetValues(typeof(data_types)))
            {
                dataTypeComboBox.Items.Add(item);
            }
        }

        private void StackPanelDropFile_Drop(object sender, DragEventArgs e)
        {
            filepaths.Clear();
            wrapPanelFiles.Children.Clear();

            //TODO: add filter for just csv
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // File handling code
                foreach (var file in files) {
                    filepaths.Add(file);
                }

                listFilesInView();
                labelStep3.Visibility = System.Windows.Visibility.Visible;
                buttonStep3.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void listFilesInView(){

            foreach (var file in filepaths)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;

                Uri uri = new Uri("pack://application:,,,/SPC Package Generator;component/csvIcon.png");

                Image img = new Image();

                BitmapImage btImg = new BitmapImage();
                btImg.BeginInit();
                btImg.UriSource = uri;
                btImg.DecodePixelHeight = 18;
                btImg.EndInit();
                img.Source = btImg;

                TextBlock txtb = new TextBlock();
                txtb.Text = file;
                txtb.TextAlignment = TextAlignment.Center;
                txtb.Foreground = Brushes.White;
                txtb.TextWrapping = TextWrapping.Wrap;
                txtb.Margin = new Thickness(0, 4, 0, 0);

                sp.Margin = new Thickness(0, 20, 0, 0);
                sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                sp.Children.Add(img);
                sp.Children.Add(txtb);
                sp.Background = Brushes.LightSlateGray;

                wrapPanelFiles.Children.Add(sp);
            }
        }
        
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            String flnm = "";

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    //flnm = (window as MainWindow).filenameTextBox.Text;
                }
            }
        }

        private void ButtonDisplay_Click(object sender, RoutedEventArgs e)
        {
            //textBoxResults.Text = ldb.displayVariables();
        }

        private void dataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            borderDragDrop.BorderBrush = Brushes.CornflowerBlue;
            textBoxDragDrop.Background = Brushes.CornflowerBlue;
            textBoxDragDrop.Text = "Step 2) Please drag and drop relevant data set here.";
        }

        private void buttonStep3_Click(object sender, RoutedEventArgs e)
        {
            List<String> filenames = new List<String>();

            string schema = dataTypeComboBox.Text;

            if (filepaths.Count > 0)
            {
                string sPattern = @"[a-zA-Z0-9_ ]*\.csv";

                foreach (string file in filepaths)
                {
                    Regex regex = new Regex(sPattern);
                    Match match = regex.Match(file);
                    if (match.Success)
                    {
                        filenames.Add(match.Value);
                    }
                }

                MessageBox.Show("Success");
            }

            //Create Consolidate Holdings/Static Script

            CreateHoldingsPackage();
            CreateStaticPackage();

            //takes a list of all the files and builds the actual first stage package
            LoadDataBuilder ld = new LoadDataBuilder();
            ld.Build_Package(filepaths, filenames, schema);
        }

        //##### Create Consolidate Holdings Package #######
        private void CreateHoldingsPackage()
        {
            string consolHolpackage = ConsolidateHoldingsStrings.consolHoldingsPackageStart +
            @"
    <<NEW IMPORT>>


    <<FIX HOLDINGS>>

    <<TASK END>>

            " + ConsolidateHoldingsStrings.consolHoldingsPackageEnd;


            System.IO.File.WriteAllText(@"C:\Test\consolidate-holdings.dspkg", consolHolpackage);
        }

        //##### Create Consolidate Holdings Package #######
        private void CreateStaticPackage()
        {
            System.IO.File.WriteAllText(@"C:\Test\consolidate-static.dspkg", ConsolidateStaticStrings.consolStaticStart);
        }
    }
}
