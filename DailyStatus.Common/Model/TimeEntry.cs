using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace DailyStatus.Common.Model
{
    public class TimeEntry : ITimeEntry
    {
        public long WorkspaceId { get; set; }

        public long? ProjectId { get; set; }
        public long? TaskId { get; set; }

        public bool Billable { get; set; }

        public DateTimeOffset Start { get; set; }

        public long? Duration { get; set; }

        public string Description { get; set; }

        public IEnumerable<long> TagIds { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public long UserId { get; set; }

        public long Id { get; set; }
    }
}
