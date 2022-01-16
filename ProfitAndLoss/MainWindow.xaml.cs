using Binance.Spot;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;
using System.Reflection; 
namespace ProfitAndLoss
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {

        string dosyaYolu = "";
        double yaklasikToplamTRY = 0;
        double yaklasikToplamFark = 0;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        ExcelOps excel;
        public MainWindow()
        {
            InitializeComponent(); 

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 15);

            if (File.Exists("kripto.xls"))
            {
                dosyaYolu = System.IO.Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString(),"kripto.xls");
                excel = new ExcelOps(dosyaYolu);
                Thread thread1 = new Thread(new ThreadStart(kriptolariGetir));
                thread1.Start();

                dispatcherTimer.Start();
                Yenile.IsEnabled = false;
                ExcelAc.IsEnabled = false;
                loading.Visibility = Visibility.Visible;
            }
         
        }

        private int _selectedBookIndex;
        public int SelectedBookIndex
        {
            get { return _selectedBookIndex; }
            set
            {
                _selectedBookIndex = value;
                Debug.WriteLine("Selected Book Index=" + _selectedBookIndex);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Excel Files (*.xls)|*.xls";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog()==true)
            {
                loading.Visibility = Visibility.Visible;
                dosyaYolu = openFileDialog1.FileName;
                Thread thread1 = new Thread(new ThreadStart(kriptolariGetir)); 
                thread1.Start();

                dispatcherTimer.Start(); 
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(new ThreadStart(kriptolariGetir));
            thread1.Start();
        }

        private void Yenile_Click(object sender, RoutedEventArgs e)
        {
            if (Yenile.Content.ToString().Contains("AÇIK"))
            {
                dispatcherTimer.Stop();
                Yenile.Content = "Oto Yenileme KAPALI";
            }
            else
            {
                dispatcherTimer.Start();
                Yenile.Content = "Oto Yenileme AÇIK";

            }
        }

        void kriptolariGetir()
        {
            yaklasikToplamTRY = 0;
            yaklasikToplamFark = 0;
             
            
            Calculate kriptolar = new Calculate(excel.liste);
            kriptolar.KriptoEkle();
            kriptolar.ProfitAndLoss();
            Application.Current.Dispatcher.Invoke(delegate
            {
                lbKriptolar.Items.Clear();
            });
            foreach (String row in kriptolar.dic.Keys)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    lbKriptolar.Items.Add(new ListBoxItem { Content = kriptolar.dic[row] });
                });
               
                yaklasikToplamTRY += kriptolar.dic[row].ToplamKripto * kriptolar.dic[row].AnlikFiyat;
                yaklasikToplamFark += kriptolar.dic[row].Fark;
            }
            Application.Current.Dispatcher.Invoke(delegate
            {
                txtMiktar.Text = "Toplam TRY: " + yaklasikToplamTRY.ToString();
                txtFark.Text = "Toplam Fark: " + yaklasikToplamFark.ToString();
                loading.Visibility = Visibility.Hidden;
                Yenile.IsEnabled = true;
                
            });

            
        }

        private void lbKriptolar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbKriptolar.SelectedItem != null)//hazır özel varsa yenisini oluşturma
            {
                Kripto? kripto = ((ListBoxItem)lbKriptolar.SelectedItem).Content as Kripto;
                MessageBox.Show(String.Format("Adı: {0}\nOrtalama Fiyatı: {1}\nİşlem Sayısı: {2}\nAnlık Fiyat: {3}\nToplam Kripto {4}\n" +
                    "Toplam TRY: {5}\nDurum: {6}\nFark: {7}",
                    kripto.Adi, kripto.OrtalamaFiyat, kripto.IslemSayisi,
                    kripto.AnlikFiyat, kripto.ToplamKripto, kripto.ToplamTRY, kripto.Durum, kripto.Fark));
            }
        }
    }
    public class Kripto
    {
        public string Adi { get; set; }
        public double OrtalamaFiyat { get; set; }
        public int IslemSayisi { get; set; }
        public double AnlikFiyat { get; set; }
        public double ToplamKripto { get; set; }
        public double ToplamTRY { get; set; }
        public string Durum { get; set; }
        public double Fark { get; set; }

        public Kripto(string Adi)
        {
            this.Adi = Adi;
            OrtalamaFiyat = 0;
            AnlikFiyat = 0;
            ToplamKripto = 0;
            IslemSayisi = 0;
            ToplamTRY = 0;
            Fark = 0;
            Durum = "Unknown";

        }
    }
}
