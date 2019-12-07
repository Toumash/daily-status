using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.Common.Model
{
    public class TogglMeDto
    {
        public TogglMeDataDto Data { get; set; }
    }
    public class TogglMeDataDto
    {
        /// <summary>
        /// Toggl User Id
        /// </summary>
        public long Id { get; set; }
    }
}
