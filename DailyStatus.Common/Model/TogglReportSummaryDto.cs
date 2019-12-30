using System.Collections.Generic;

namespace DailyStatus.Common.Model
{
    public class TogglSummaryReportDto
    {
        public List<Datum> Data { get; set; }
    }

    public class Datum
    {
        public long Id { get; set; }
        public long Time { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public long Time { get; set; }
        public double Sum { get; set; }
        public double Rate { get; set; }
    }
}
