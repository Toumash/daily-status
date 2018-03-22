using DailyStatus.Common.Model;
using DailyStatus.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace DailyStatus.Common.Configuration
{
    public class DailyStatusConfiguration
    {
        public WorkDay GetWorkDayConfig()
        {
            var serializer = new XmlSerializer(typeof(AppConfig));
            var fileStream = new FileStream("config.xml", FileMode.Open);
            var appConfig = (AppConfig)serializer.Deserialize(fileStream);
            return appConfig.WorkDayConfig;
        }
    }
}
