using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WordleHelper;

public sealed partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainWindowViewModel();
    }
}
