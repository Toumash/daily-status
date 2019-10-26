using DailyStatus.Common.Configuration;
using DailyStatus.UI.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.UI
{
    public class SettingsManager
    {
        public static DailyStatusConfiguration LoadSettings()
        {
            var stg = Settings.Default;
            var cfg = new DailyStatusConfiguration();
            Enum.TryParse(stg.display_mainkpi_type, out DisplayType displayType);
            cfg.DisplayType = displayType;
            cfg.HourRate = stg.hour_rate;
            cfg.WorkDayStartHour = stg.start_hour;
            cfg.HoursADay = stg.hours_a_day;
            cfg.WorkspaceId = stg.api_workspaceid;
            cfg.Holidays = stg.holidays.Split(',').Select(str => DateTime.Parse(str, CultureInfo.InvariantCulture)).ToList();
            cfg.SumSince = stg.sum_since;
            return cfg;
        }

        public static void SaveSettings(DailyStatusConfiguration cfg)
        {
            var stg = Settings.Default;
            stg.display_mainkpi_type = cfg.DisplayType.ToString();
            stg.api_workspaceid = cfg.WorkspaceId;
            stg.hour_rate = cfg.HourRate;
            stg.start_hour = cfg.WorkDayStartHour;
            stg.hours_a_day = cfg.HoursADay;
            stg.holidays = string.Join(",", cfg.Holidays.Select(d => d.ToString(CultureInfo.InvariantCulture)));
            stg.sum_since = cfg.SumSince;
            stg.Save();
        }
    }
}
