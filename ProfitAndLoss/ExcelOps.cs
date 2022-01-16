using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ProfitAndLoss
{
    internal class ExcelOps
    {
        public List<AlimSatimBilgi> liste = new List<AlimSatimBilgi>();
        public ExcelOps(string fileName)
        {
            
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(fileName);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            for (int i = 2; i <= rowCount; i++)
            {

                //write the value to the console
                if (xlRange.Cells[i, 1] != null && xlRange.Cells[i, 1].Value2 != null)
                {
                    AlimSatimBilgi bilgi = new AlimSatimBilgi();
                  

                    bilgi.Tarih = xlRange.Cells[i, 1].Value2.ToString();
                    bilgi.Cift = xlRange.Cells[i, 2].Value2.ToString();
                    bilgi.Taraf = xlRange.Cells[i, 3].Value2.ToString();
                    bilgi.Fiyat = xlRange.Cells[i, 4].Value2.ToString();
                    bilgi.Gerceklesti = xlRange.Cells[i, 5].Value2.ToString();
                    bilgi.IslemUcreti = xlRange.Cells[i, 6].Value2.ToString();
                    bilgi.Toplam = xlRange.Cells[i, 7].Value2.ToString();

                    liste.Add(bilgi);
                    //add useful things here!   
                }
            }
            xlWorkbook.Close(0);
            xlApp.Quit();
            xlApp = null;
            GC.Collect();

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
