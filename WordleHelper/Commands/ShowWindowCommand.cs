using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleHelper.Models;
using WordleHelper.ViewModels;
using WordleHelper.Views;

namespace WordleHelper.Commands;

class ShowWindowCommand : CommandBase
{
    public override void Execute(object? parameter)
    {
        PreviousDays previousDays = new();
        previousDays.Show();
    }
}
