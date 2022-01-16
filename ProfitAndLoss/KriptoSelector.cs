using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProfitAndLoss
{
    public class KriptoSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            item = ((ListBoxItem)item).Content as Kripto;
            if (item != null && item is Kripto)
            {
                var taskitem = (Kripto)item;
                var window = Application.Current.MainWindow;
                if(taskitem.Durum=="Kar")
                    return window.FindResource("bilgi_kripto_kar") as DataTemplate;
                else 
                    return window.FindResource("bilgi_kripto_zarar") as DataTemplate;
            }
            return null;
        }
    }
}
