using DailyStatus.Common.Configuration;
using DailyStatus.UI.Properties;
using System;
using System.Collections.Generic;
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
            cfg.NumberOfWorkingHoursPerDay = stg.hours_a_day;
            cfg.WorkspaceId = stg.api_workspaceid;
            return cfg;
        }

        public static void SaveSettings(DailyStatusConfiguration cfg)
        {
            var stg = Settings.Default;
            stg.display_mainkpi_type = cfg.DisplayType.ToString();
            stg.api_workspaceid = cfg.WorkspaceId;
            stg.hour_rate = cfg.HourRate;
            stg.start_hour = cfg.WorkDayStartHour;
            stg.hours_a_day = cfg.NumberOfWorkingHoursPerDay;
            stg.Save();
        }
    }
}
