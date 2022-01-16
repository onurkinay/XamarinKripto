using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mobile
{
    internal class Calculate
    {
        List<AlimSatimBilgi> liste;
        public Dictionary<string, Kripto> dic = new Dictionary<string, Kripto>();
        public Calculate(List<AlimSatimBilgi> liste)
        {
            this.liste = liste;
        }

        public bool KriptoEkle()
        {
            dic.Clear();

            //USDT TRY birimi alır  
            double usdtTry = getCurrency("USDTTRY");
            if (usdtTry == -1) return false;
            foreach (AlimSatimBilgi row in liste)
            {
                if (!dic.Keys.Contains(row.Cift.Split('/')[0]))
                {
                    //KRIPTO USDT kuru alır 
                    double usdtK = getCurrency(row.Cift.Split('/')[0].ToUpper().Trim() + "USDT");
                    if (usdtK == -1) return false;

                    Kripto kripto = new Kripto(row.Cift.Split('/')[0]);
                    kripto.ToplamKripto += double.Parse(row.Gerceklesti, CultureInfo.InvariantCulture);
                    kripto.ToplamTRY += double.Parse(row.Toplam.Replace("TRY", ""), CultureInfo.InvariantCulture);
                    kripto.OrtalamaFiyat += double.Parse(row.Fiyat, CultureInfo.InvariantCulture);
                    kripto.IslemSayisi++;

                    kripto.AnlikFiyat = usdtK * usdtTry;
                    dic.Add(row.Cift.Split('/')[0], kripto);

                }
                else
                {
                    Kripto kripto = dic[row.Cift.Split('/')[0]];
                    kripto.ToplamKripto += Double.Parse(row.Gerceklesti, CultureInfo.InvariantCulture);//Double.Parse(row.Toplam.Replace("TRY", ""), CultureInfo.InvariantCulture)
                    kripto.ToplamTRY += Double.Parse(row.Toplam.Replace("TRY", ""), CultureInfo.InvariantCulture);
                    kripto.OrtalamaFiyat += Double.Parse(row.Fiyat, CultureInfo.InvariantCulture);
                    kripto.IslemSayisi++;
                }
            } 
            return true;
        }

        public void ProfitAndLoss()
        {//kar ve zarar hesabını yapar
            foreach (String row in dic.Keys)
            {
                Kripto kripto = dic[row];
                kripto.OrtalamaFiyat = kripto.ToplamTRY / kripto.ToplamKripto;
                kripto.Durum = (kripto.OrtalamaFiyat < kripto.AnlikFiyat) ? "Kar" : "Zarar";
                kripto.Renk = (kripto.OrtalamaFiyat < kripto.AnlikFiyat) ? "Green" : "Red";
                double yeniToplamTRY = kripto.AnlikFiyat * kripto.ToplamKripto;
                kripto.Fark = yeniToplamTRY - kripto.ToplamTRY;
            }

        }
        public double getCurrency(string currency)
        {
            try
            {
                string str = new WebClient().DownloadString("https://www.binance.com/api/v3/avgPrice?symbol=" + currency);
                JObject usdtKripto = JObject.Parse(str);
                return double.Parse((string)usdtKripto["price"], CultureInfo.InvariantCulture);
            }
            catch (WebException exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                return -1;
            }
        }

    }
}
