using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MovementInOutDetection
{
    public class TwoSensorsDetectingMovementDirection
    {
        private HC_SR04_DistanceSensor[] sensors;

        //private double[] standardDistanceSensors = new double[2] { -1, -1 };

        private DispatcherTimer timer_calculating;
        public double DistanceMarginal { get; set; } = 20;
        public TwoSensorsDetectingMovementDirection() { }

        public TwoSensorsDetectingMovementDirection(HC_SR04_DistanceSensor sensor1, HC_SR04_DistanceSensor sensor2)
        {
            sensors[0] = sensor1;
            sensors[1] = sensor2;

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


        Stopwatch[] SensorDetectingWatch = null;

        private void Timer_calculating_Tick(object sender, object e)
        {
            if (!sensors.Any(s => s.MeasuredDistance == -1))
            {
                Parallel.ForEach(new int[2] { 1, 2 }, sensorIndex =>
                {
                    if (sensors[sensorIndex].MeasuredDistance < sensors[sensorIndex].StandardDistance - DistanceMarginal)
                        if (!SensorDetectingWatch[sensorIndex].IsRunning)
                            SensorDetectingWatch[sensorIndex].Start();
                        else
                        if (SensorDetectingWatch[sensorIndex].IsRunning)
                        {
                            //TODO: nonon


                    khj
                        }
                });
            }
        }
    }
}
