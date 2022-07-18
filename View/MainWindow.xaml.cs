using ACCess.ViewModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ACCess.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            lblVersion.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void txtAddress_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var parent = sender as TextBox;
            var vm = DataContext as MainViewModel;
            if (vm.SelectedSavedServer != null && !string.Equals(vm.SelectedSavedServer.Address, vm.SelectedServer))
            {
                lstFavourites.SelectedItem = null;
                vm.SelectedSavedServer = null;
            }

            if (!string.IsNullOrWhiteSpace(vm.ServerListAddress) && !string.Equals(vm.ServerListAddress, parent.Text))
                vm.UnsavedChanges = true;
            else if (!string.IsNullOrWhiteSpace(parent.Text))
                vm.UnsavedChanges = true;
            else
                vm.UnsavedChanges = false;
        }
    }
}