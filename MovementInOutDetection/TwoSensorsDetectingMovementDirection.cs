using HC_SR04_DistanceSensor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MovementInOutDetection
{
    public class TwoSensorsDetectingMovementDirection
    {

        public TwoSensorsDetectingMovementDirection() { }

        public TwoSensorsDetectingMovementDirection(HCSR04 sensor1, HCSR04 sensor2)
        {
            sensors = new HCSR04[2]
            {
                sensor1, sensor2
            };

            for (short sensorIndex = 0; sensorIndex < 2; sensorIndex++)
            {
                if (sensors[sensorIndex].StandardDistance == -1)
                    this.sensors[sensorIndex].StartSearchingStandardDistance();
            }

            timer_calculating = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timer_calculating.Tick += Timer_calculating_Tick;
            timer_calculating.Start();
        }

              
        public TwoSensorsDetectingMovementDirection(HCSR04 sensor1, HCSR04 sensor2, ref TextBlock textBlockIn, ref TextBlock textBlockOut, ref TextBlock textBlockTotal)
        {

            this.textBlockIn = textBlockIn;
            this.textBlockOut = textBlockOut;
            this.textBlockTotal = textBlockTotal;

            sensors = new HCSR04[2]
            {
                sensor1, sensor2
            };

            for (short sensorIndex = 0; sensorIndex < 2; sensorIndex++)
            {
                if (sensors[sensorIndex].StandardDistance == -1)
                    this.sensors[sensorIndex].StartSearchingStandardDistance();
            }

            timer_calculating = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timer_calculating.Tick += Timer_calculating_Tick;
            timer_calculating.Start();

        }

        private HCSR04[] sensors;

        private DispatcherTimer timer_calculating;
        public double DistanceMarginal { get; set; } = 20;

        TextBlock textBlockIn, textBlockOut, textBlockTotal;
        public double TotalPassIn = 0, TotalPassOut = 0, TotalInside = 0;

        private void Timer_calculating_Tick(object sender, object e)
        {
            if (!sensors.Any(s => s.MeasuredDistance == -1))
            {
                Parallel.ForEach(new int[2] { 1, 2 }, sensorIndex =>
                {
                    if (sensors[sensorIndex].MeasuredDistance < sensors[sensorIndex].StandardDistance - DistanceMarginal)
                    {
                        if (!MeasuringsElapsed.Any(l => l.IsStillOpen && l.SensorId.Equals(sensors[sensorIndex].Id)))
                            MeasuringsElapsed.Add(new Lapse(sensors[sensorIndex].Id));
                    }
                    else
                    {
                        if (MeasuringsElapsed.Any(l => l.IsStillOpen && l.SensorId.Equals(sensors[sensorIndex].Id)))
                        {
                            MeasuringsElapsed.First(l => l.IsStillOpen && l.SensorId.Equals(sensors[sensorIndex].Id)).CloseLapse();
                            CalculateMeasuringsElapsed();
                        }
                    }
                });
            }
        }
                
        List<Lapse> MeasuringsElapsed = new List<Lapse>();
                
        private void CalculateMeasuringsElapsed()
        {
            var MeasuringsElapsedWorkingCopy = MeasuringsElapsed;
            if (MeasuringsElapsedWorkingCopy.Count(m => m.IsStillOpen) == 2)
            {
                if (MeasuringsElapsedWorkingCopy.Select(m => m.SensorId).Distinct().Count() == 2)
                {
                    // Processing is here
                    var comingFrom = MeasuringsElapsedWorkingCopy.First(m => m.Started == MeasuringsElapsedWorkingCopy.Min(mm => mm.Started)).SensorId;
                    var passDuration = MeasuringsElapsedWorkingCopy.Min(m => m.Closed) - MeasuringsElapsedWorkingCopy.Min(m => m.Started);

                    if (comingFrom == sensors[0].Id)
                    {
                        TotalPassIn++;
                        TotalInside++;
                    }
                    else
                    {
                        TotalPassOut++;
                        TotalInside--;
                    }

                    lock (MeasuringsElapsed)
                        MeasuringsElapsedWorkingCopy.ForEach(mwc => MeasuringsElapsed.RemoveAll(m => m.Id.Equals(mwc.Id)));

                    // ----------------

                    //Special only for this pilot app
                    textBlockIn.Text = TotalPassIn.ToString();
                    textBlockOut.Text = TotalPassOut.ToString();
                    textBlockTotal.Text = TotalInside.ToString();

                    // ----------------

                }
                else
                {
                    lock (MeasuringsElapsed)
                        MeasuringsElapsed.RemoveAll(m => m.Closed != MeasuringsElapsedWorkingCopy.Max(mwc => mwc.Closed));
                }
            }
        }
    }

    public class Lapse
    {
        public Lapse(Guid sensorId)
        {
            this.SensorId = sensorId;
        }

        public Guid Id = Guid.NewGuid();
        public Guid SensorId;
        public readonly DateTime Started = DateTime.Now;
        public DateTime? Closed { get; private set; }
        public TimeSpan? Duration { get; private set; }

        public void CloseLapse()
        {
            Closed = DateTime.Now;
            Duration = Closed - Started;
            IsStillOpen = false;
        }

        public bool IsStillOpen = true;
    }
}
