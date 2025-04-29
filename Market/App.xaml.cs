using Market;
using System.Windows;
using System;
using System.Linq;
using static Market.AuthWindow;

namespace Market
{
    public partial class App : Application
    {

        public static User CurrentUser { get; set; }

      

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LocalizationManager.SetLanguage("ru-RU");

            //var authWindow = new AuthWindow();
            //if (authWindow.ShowDialog() == true)
            //{
            //    new MainWindow().Show();
            //}
            

        }

        public static void NavigateToProductDetail(string productId, MainWindowViewModel viewModel)
        {
            var product = viewModel.AutoParts.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var detailWindow = new ProductDetailWindow(
                    product,
                    CurrentUser?.Role == "Admin" ? p => viewModel.EditProduct(p) : (Action<AutoPart>)null,
                    CurrentUser?.Role == "Admin" ? id => viewModel.DeleteProduct(id) : (Action<string>)null);
                detailWindow.Show();
            }
        }
    }
}