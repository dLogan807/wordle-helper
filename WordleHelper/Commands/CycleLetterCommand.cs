using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleHelper.Models;
using WordleHelper.ViewModels;

namespace WordleHelper.Commands;

class CycleLetterCommand : CommandBase
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        Letter letter = (Letter)parameter;
        letter.CycleLetterCorrectness();
    }
}
