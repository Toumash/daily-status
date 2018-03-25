using DailyStatus.Common.Model;
using DailyStatus.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace DailyStatus.Common.Configuration
{
    public class DailyStatusConfiguration
    {
        AppConfig _config = null;

        public AppConfig Config
        {
            get
            {
                if (_config == null)
                {
                    var serializer = new XmlSerializer(typeof(AppConfig));
                    using (var fileStream = new FileStream("config.xml", FileMode.Open))
                    {
                        _config = (AppConfig)serializer.Deserialize(fileStream);
                    }
                }
                return _config;
            }
        }

        public WorkDay GetWorkDayConfig()
        {
            return Config.WorkDayConfig;
        }
    }
}
