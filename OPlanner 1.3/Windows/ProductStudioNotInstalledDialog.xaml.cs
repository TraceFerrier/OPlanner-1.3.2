using System.Diagnostics;
using System.Windows;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for ProductStudioNotInstalledDialog.xaml
    /// </summary>
    public partial class ProductStudioNotInstalledDialog : Window
    {
        const string noVersionMessage = "OPlanner Requires Product Studio version 2.2 or later to be installed on your machine, but no installed version was found.  You can click the button below to open the Product Studio Installation page - once you've installed Product Studio successfully, start up OPlanner again.";
        const string olderVersionMessage = "OPlanner Requires Product Studio version 2.2 or later to be installed on your machine, but an older version was found.  You can click the button below to open the Product Studio Installation page.  Note: after starting the install, if the Installer indicates that Product Studio is already installed, select the option to reinstall the product. ";
        public ProductStudioNotInstalledDialog(StoreErrors error)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            ErrorMessageBox.Text = error == StoreErrors.ProductStudioNotInstalled ? noVersionMessage : olderVersionMessage;
            LaunchInstallButton.Click += LaunchInstallButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void LaunchInstallButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Process.Start("http://productsweb/product.aspx?productnameid=1251&status=3&platformid=-999&category=1");
        }
    }
}
