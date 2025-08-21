using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WordleHelper.ViewModel;

namespace WordleHelper;

public partial class Main : Window
{
    private readonly MainViewModel mainViewModel = new();

    public Main()
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}
