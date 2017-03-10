using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovementsDetectionSender
{
    public sealed class Lapse
    {
        public Lapse(Guid sensorId)
        {
            Id = Guid.NewGuid();
            SensorId = sensorId;
            IsStillOpen = true;
            Started = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public Guid SensorId { get; private set; }
        public DateTimeOffset Started { get; private set; }
        public DateTimeOffset? Closed { get; private set; }
        public TimeSpan? Duration { get; private set; }

        public void CloseLapse()
        {
            Closed = DateTime.Now;
            Duration = Closed - Started;
            IsStillOpen = false;
        }

        public bool IsStillOpen { get; private set; }
    }
}
