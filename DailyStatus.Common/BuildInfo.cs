using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.Common
{
    public static class DailyStatusBuildInfo
    {
        public static string ShortVersion => $"{ThisAssembly.Git.BaseTag}-{ThisAssembly.Git.Commit}";
        public static string LongVersion => $"{ThisAssembly.Git.BaseTag}-{ThisAssembly.Git.Branch}-{ThisAssembly.Git.Commit}-{ThisAssembly.Git.Commits + (ThisAssembly.Git.IsDirty ? "-dirty" : "")}";
    }
}
