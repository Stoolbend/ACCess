using ACCess.ViewModel;
using System.Reflection;
using System.Windows;

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
    }
}
