using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace GPIOExtention
{
   
    public class GPIO
    {
        private const int MAXIMUN_TIME_TO_WAIT_IN_MILLISECONDS = 100;

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

            try
            {


                stopWatch.Reset();

                while (echoPin.Read() == GpioPinValue.Low)
                {
                    if (stopWatch.ElapsedMilliseconds > MAXIMUN_TIME_TO_WAIT_IN_MILLISECONDS)
                        return -1;
                };

                stopWatch.Start();

                while (echoPin.Read() == GpioPinValue.High)
                {
                    if (stopWatch.ElapsedMilliseconds > MAXIMUN_TIME_TO_WAIT_IN_MILLISECONDS)
                        return -1;
                };
                stopWatch.Stop();

                return stopWatch.Elapsed.TotalSeconds;

            }
            catch
            {
                return -1;
            }

        }

    }

}
