using System.Windows;

namespace SalesAnalysis
{
    public partial class SalesAnalyzerWindow : Window
    {
        private static SalesAnalyzerWindow _window;
        public CarSalesViewModel ViewModel { get; set; }
        public SalesAnalyzerWindow()
        {
            InitializeComponent();
            _window = this;
            ViewModel = new CarSalesViewModel();
            _window.DataContext = new CarSalesViewModel();

        }
        public static SalesAnalyzerWindow Instance { get { return _window; } }     
    }
}
