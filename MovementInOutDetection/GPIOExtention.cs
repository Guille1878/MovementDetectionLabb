using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace MovementInOutDetection
{
    public static class GPIOExtention
    {
        private const int MAXIMUN_TIME_TO_WAIT_IN_MILLISECONDS = 10;

        private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        public static void MicrosecondsDelay(int delayMicroseconds)
        {
            manualResetEvent.WaitOne(
                TimeSpan.FromMilliseconds((double)delayMicroseconds / 1000d));
        }

        public static void Delay(int delayMilliseconds)
        {
            manualResetEvent.WaitOne(delayMilliseconds);
        }

        private static Stopwatch stopWatch = new Stopwatch();
        public static double PulseIn(GpioPin echoPin)
        {
            var t = Task.Run(() =>
            {
                stopWatch.Reset();

                while (echoPin.Read() == GpioPinValue.Low) { };

                stopWatch.Start();

                while (echoPin.Read() == GpioPinValue.High) { };

                stopWatch.Stop();

                return stopWatch.Elapsed.TotalSeconds;
            });

            bool isCompleted = t.Wait(TimeSpan.FromMilliseconds(MAXIMUN_TIME_TO_WAIT_IN_MILLISECONDS));

            if (isCompleted)
            {
                return t.Result;
            }
            else
            {
                return -1d;
            }
        }

    }

}
