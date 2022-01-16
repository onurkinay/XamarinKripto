using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Xamarin.Forms;
using System.Data;

namespace Mobile
{
    public class JsonOps
    {
        public List<AlimSatimBilgi> liste = new List<AlimSatimBilgi>();
        public JsonOps(string jsonText)
        { 

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(StringReplace(jsonText));

            DataTable dataTable = dataSet.Tables[0];
             
            foreach(DataRow item in dataTable.Rows)
            {
                AlimSatimBilgi bilgi = new AlimSatimBilgi();

                bilgi.Tarih = item[0].ToString();
                bilgi.Cift = item[1].ToString();
                bilgi.Taraf = item[2].ToString();
                bilgi.Fiyat = item[3].ToString();
                bilgi.Gerceklesti = item[4].ToString();
                bilgi.IslemUcreti = item[5].ToString();
                bilgi.Toplam = item[6].ToString();

                liste.Add(bilgi);
            }
          

        }

        public string StringReplace(string text)
        {
            text = text.Replace("İ", "I");
            text = text.Replace("ı", "i");
            text = text.Replace("Ğ", "G");
            text = text.Replace("ğ", "g");
            text = text.Replace("Ö", "O");
            text = text.Replace("ö", "o");
            text = text.Replace("Ü", "U");
            text = text.Replace("ü", "u");
            text = text.Replace("Ş", "S");
            text = text.Replace("ş", "s");
            text = text.Replace("Ç", "C");
            text = text.Replace("ç", "c"); 
            return text;
        }

    }

    public class AlimSatimBilgi
    {
        public string Tarih { get; set; }
        public string Cift { get; set; }
        public string Taraf { get; set; }
        public string Fiyat { get; set; }
        public string Gerceklesti { get; set; }
        public string IslemUcreti { get; set; }
        public string Toplam { get; set; }
    }
}
