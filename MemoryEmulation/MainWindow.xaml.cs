using MemoryEmulation.ViewModels;

namespace MemoryEmulation
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(Dispatcher);
        }
    }
}
