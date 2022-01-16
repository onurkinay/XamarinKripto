using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Binance.Spot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProfitAndLoss
{
    internal class Calculate
    { 
        List<AlimSatimBilgi> liste;
        public Dictionary<string, Kripto> dic = new Dictionary<string, Kripto>();
        public Calculate(List<AlimSatimBilgi> liste)
        {
            this.liste = liste;

        }

        public void KriptoEkle()
        {
            dic.Clear();
            foreach (AlimSatimBilgi row in liste)
            {
                if (!dic.Keys.Contains(row.Cift.Split("/")[0]))
                {
                    var client = new HttpClient();

                    var webRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.binance.com/api/v3/avgPrice?symbol="+ row.Cift.Split("/")[0].ToUpper().Trim()+ "TRY");
                   
                    var response = client.Send(webRequest);

                    using var reader = new StreamReader(response.Content.ReadAsStream()); 
                    

                    Kripto kripto = new Kripto(row.Cift.Split("/")[0]);
                    kripto.ToplamKripto += Convert.ToDouble(row.Gerceklesti);
                    kripto.ToplamTRY += Convert.ToDouble(row.Toplam.Replace("TRY", ""));
                    kripto.OrtalamaFiyat += Convert.ToDouble(row.Fiyat);
                    kripto.IslemSayisi++;

                    JObject obj = JObject.Parse(reader.ReadToEnd());
                    kripto.AnlikFiyat = Convert.ToDouble((string)obj["price"]);
                    dic.Add(row.Cift.Split("/")[0], kripto);


                }
                else
                {
                    Kripto kripto = dic[row.Cift.Split("/")[0]];
                    kripto.ToplamKripto += Convert.ToDouble(row.Gerceklesti);
                    kripto.ToplamTRY += Convert.ToDouble(row.Toplam.Replace("TRY", ""));
                    kripto.OrtalamaFiyat += Convert.ToDouble(row.Fiyat);
                    kripto.IslemSayisi++;
                }
            }

        }

        public void ProfitAndLoss()
        {
            foreach (String row in dic.Keys)
            {
                Kripto kripto = dic[row];
                kripto.OrtalamaFiyat = kripto.ToplamTRY / kripto.ToplamKripto;
                kripto.Durum = (kripto.OrtalamaFiyat < kripto.AnlikFiyat) ? "Kar" : "Zarar";

                double yeniToplamTRY = kripto.AnlikFiyat * kripto.ToplamKripto;
                kripto.Fark = yeniToplamTRY - kripto.ToplamTRY;
            }

        }


    }
}
