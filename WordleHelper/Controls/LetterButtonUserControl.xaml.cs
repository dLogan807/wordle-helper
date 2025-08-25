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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WordleHelper.Models;

namespace WordleHelper.Controls;

public partial class LetterButtonUserControl : UserControl
{
    public Letter Letter
    {
        get { return (Letter)GetValue(LetterProperty); }
        set { SetValue(LetterProperty, value); }
    }

    private readonly Storyboard _showLetterStoryboard;

    public static readonly DependencyProperty LetterProperty = DependencyProperty.Register(
        "Letter",
        typeof(Letter),
        typeof(LetterButtonUserControl),
        new PropertyMetadata(null)
    );

    public LetterButtonUserControl()
    {
        InitializeComponent();
        _showLetterStoryboard = (Storyboard)FindResource("showLetterButtonStoryboard");
    }

    //Switch letter colour when hidden (halfway through animation)
    private void LetterHideAnim_Completed(object sender, EventArgs e)
    {
        Letter.CycleLetterCorrectness();
        _showLetterStoryboard.Begin();
    }
}
