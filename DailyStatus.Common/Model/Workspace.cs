using System.Diagnostics;

namespace DailyStatus.Common.Model
{
    [DebuggerDisplay("Workspace #{Id} {Name}")]  
    public class Workspace
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
