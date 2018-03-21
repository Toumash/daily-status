using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.UI.ViewModel
{
    class StatusViewModel
    {
        public SeriesCollection SeriesCollection { get; set; }
            = new SeriesCollection {
                    new LineSeries { Values = new ChartValues <double> {3,5,7,4}, StrokeDashArray = new System.Windows.Media.DoubleCollection {2} },
                    new ColumnSeries { Values = new ChartValues <decimal> {5,6,2,7} }
                  };
    }
}
