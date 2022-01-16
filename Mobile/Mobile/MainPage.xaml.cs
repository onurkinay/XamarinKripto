using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
namespace Mobile
{
    public partial class MainPage : ContentPage
    {
        double yaklasikToplamTRY = 0;
        double yaklasikToplamFark = 0;
        JsonOps json;
        bool _isRunning = false;
        ObservableCollection<Kripto> ListKriptolar = new ObservableCollection<Kripto>();
        public ObservableCollection<Kripto> Kriptolar { get { return ListKriptolar; } }
        public MainPage()
        {
            InitializeComponent();
            Stream stream = this.GetType().Assembly.GetManifestResourceStream("Mobile.data.json");
            string jsonText = "";

            using (StreamReader sr = new StreamReader(stream))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    jsonText += line;
            }

            json = new JsonOps(jsonText);
            lbKriptolar.ItemsSource = ListKriptolar;

            Task.Run(asyncRun);


            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (_isRunning)
                    Task.Run(asyncRun);

                return true;
            });

        }

        public Task<bool> asyncRun()
        {
            _isRunning = false;
            Dictionary<string, object> sonuc = kriptolariGetir();


            Device.BeginInvokeOnMainThread(() =>
            {
                if (sonuc != null)
                {
                    ListKriptolar.Clear();  
                    ((List<Kripto>)sonuc["kriptoListesi"]).ForEach(ListKriptolar.Add);

                    txtMiktar.Text = "Toplam ₺: " + string.Format("{0:N4}", sonuc["yaklasikToplamTRY"]);
                    txtFark.Text = "Toplam Fark: " + string.Format("{0:N4}", sonuc["yaklasikToplamFark"]);

                    loading.IsVisible = false;
                    txtDurum.IsVisible = false;
                }
                else
                {
                    ListKriptolar.Clear();
                    loading.IsVisible = true;
                    txtDurum.IsVisible = true;
                    txtDurum.Text = "Internet bağlantınızı kontrol ediniz...";

                }
            });

            _isRunning = true;
            return Task.FromResult(_isRunning);
        }

        Dictionary<string, object> kriptolariGetir()
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            List<Kripto> kriptoListesi = new List<Kripto>();

            yaklasikToplamTRY = 0;
            yaklasikToplamFark = 0;

            Calculate kriptolar = new Calculate(json.liste);
            if (kriptolar.getCurrency("BTCTRY") == -1) return null;

            Device.BeginInvokeOnMainThread(() =>
            {
                txtDurum.Text = "Portfolyonuz hesaplanıyor. Lütfen Bekleyiniz...";
            });

            bool hepsiMi = kriptolar.KriptoEkle();
            kriptolar.ProfitAndLoss();

            foreach (string row in kriptolar.dic.Keys)
            {
                kriptoListesi.Add(kriptolar.dic[row]);
                yaklasikToplamTRY += kriptolar.dic[row].ToplamKripto * kriptolar.dic[row].AnlikFiyat;
                yaklasikToplamFark += kriptolar.dic[row].Fark;
            }

            int kriptoCount = json.liste.Select(l => l.Cift).Distinct().Count();
            if (kriptoListesi.Count != kriptoCount)
            {
                kriptoListesi.Clear();
                  
                System.Diagnostics.Debug.WriteLine("##Hata##1");
                System.Diagnostics.Debug.WriteLine(kriptoCount + " kadar vardı");
                System.Diagnostics.Debug.WriteLine(kriptoListesi + " kadar vardı listede");
                return null;

            } 

            list.Add("kriptoListesi", kriptoListesi);
            list.Add("yaklasikToplamTRY", yaklasikToplamTRY);
            list.Add("yaklasikToplamFark", yaklasikToplamFark);

            return list;

        } 
        private async void lbKriptolar_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)//hazır özel varsa yenisini oluşturma
            {
                Kripto kripto = e.Item as Kripto;
                await App.Current.MainPage.DisplayAlert(kripto.Adi, String.Format("Adı: {0}\nOrtalama Fiyatı: {1}\nİşlem Sayısı: {2}\nAnlık Fiyat: {3}\nToplam Kripto {4}\n" +
                    "Toplam TRY: {5}\nDurum: {6}\nFark: {7}",
                    kripto.Adi, kripto.OrtalamaFiyat, kripto.IslemSayisi,
                    kripto.AnlikFiyat, kripto.ToplamKripto, kripto.ToplamTRY, kripto.Durum, kripto.Fark), "Cancel");
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
        public string Renk { get; set; }
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
