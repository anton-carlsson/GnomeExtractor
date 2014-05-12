using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GnomeExtractor
{
    public partial class About : Window
    {
        public About()
        {
            Globals.Logger.Debug("Creating About.xaml window...");
            InitializeComponent();
        }

        private void projectLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private void discussionLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(projectLinkTextBlock.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Globals.Logger.Debug("About.xaml window is closed");
        }
    }
}
