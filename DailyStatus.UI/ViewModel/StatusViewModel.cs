﻿using DailyStatus.UI.WpfExtensions;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DailyStatus.UI.ViewModel
{
    class StatusViewModel
    {
        public double TimeDiff { get; set; } = 1.5d;

        public ICommand CloseButtonCommand { get; } = new RelayCommand((s) => Environment.Exit(0));

        public string TbTimeDiff { get; set; } = "2h";
    }
}
