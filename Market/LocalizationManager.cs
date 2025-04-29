using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Market
{
    public class LocalizationManager
    {
        public static void SetLanguage(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var dict = new ResourceDictionary();
            switch (languageCode)
            {
                case "en-US":
                    dict.Source = new Uri("Resources/Strings.en-US.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Resources/Strings.ru-RU.xaml", UriKind.Relative);
                    break;
            }

            var app = Application.Current;
            var oldDict = app.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("Resources/Strings."));
            if (oldDict != null)
            {
                app.Resources.MergedDictionaries.Remove(oldDict);
            }
            app.Resources.MergedDictionaries.Add(dict);
        }
    }
}
