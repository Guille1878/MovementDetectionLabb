using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using static MovementInOutDetection.MainPage.GPIOExtention;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MovementInOutDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        DispatcherTimer timer;

        private const int PIN_TRIG_SENSOR_1 = 22;
        GpioPin pin_trig_sensor_1;
        private const int PIN_ECHO_SENSOR_1 = 27;
        GpioPin pin_echo_sensor_1;

        private const int PIN_TRIG_SENSOR_2 = 2;
        GpioPin pin_trig_sensor_2;
        private const int PIN_ECHO_SENSOR_2 = 3;
        GpioPin pin_echo_sensor_2;

        private const int MAXIMUN_TIME_TO_WAIT_IN_MILLISECONDS = 10;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            InitGPIO();
            if (pin_trig_sensor_1 != null &&
                pin_echo_sensor_1 != null &&
                pin_trig_sensor_2 != null &&
                pin_echo_sensor_2 != null)
            {
                timing = Stopwatch.StartNew();
                timer.Start();
            }
        }

        double standardDistanceSensro1 = -1, standardDistanceSensro2 = -1;
        Sensor sensor1 = new Sensor();
        Sensor sensor2 = new Sensor();

        Stopwatch timing;

        private void Timer_Tick(object sender, object e)
        {
            try
            {

                textBlockTimeValue.Text = timing.Elapsed.ToString();

                Parallel.Invoke(
                    () => sensor1.AddMesure(GetDistanceFromSensor(pin_trig_sensor_1, pin_echo_sensor_1)),
                    () => sensor2.AddMesure(GetDistanceFromSensor(pin_trig_sensor_2, pin_echo_sensor_2))
                );

                textBlockDistanceSensro1Value.Text = Math.Round(sensor1.Distance, 1).ToString() + " cm";
                textBlockDistanceSensro2Value.Text = Math.Round(sensor2.Distance, 1).ToString() + " cm";

            }
            catch (Exception ex)
            {
                textBlockMessage.Text = ex.ToString();
            }
        }

        double GetDistanceFromSensor(GpioPin trigPin, GpioPin echoPin)
        {

            double reslut = -1;

            try
            {

                Stopwatch pulseLength = new Stopwatch();

                trigPin.Write(GpioPinValue.Low);
                MicrosecondsDelay(5);

                trigPin.Write(GpioPinValue.High);
                MicrosecondsDelay(20);

                trigPin.Write(GpioPinValue.Low);

                double duration = PulseIn(echoPin);

                if (duration > -1) 
                    reslut = duration * 17000;

            }
            catch (Exception ex)
            {
                textBlockMessage.Text = ex.ToString();
            }

            return reslut;
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin_trig_sensor_1 = pin_echo_sensor_1 = pin_trig_sensor_2 = pin_echo_sensor_2 = null;
                textBlockMessage.Text = "There is no GPIO controller on this device.";
                return;
            }


            pin_trig_sensor_1 = gpio.OpenPin(PIN_TRIG_SENSOR_1);
            pin_trig_sensor_1.SetDriveMode(GpioPinDriveMode.Output);

            pin_echo_sensor_1 = gpio.OpenPin(PIN_ECHO_SENSOR_1);
            pin_echo_sensor_1.SetDriveMode(GpioPinDriveMode.Input);

            pin_trig_sensor_2 = gpio.OpenPin(PIN_TRIG_SENSOR_2);
            pin_trig_sensor_2.SetDriveMode(GpioPinDriveMode.Output);

            pin_echo_sensor_2 = gpio.OpenPin(PIN_ECHO_SENSOR_2);
            pin_echo_sensor_2.SetDriveMode(GpioPinDriveMode.Input);


            textBlockMessage.Text = "GPIO pins initialized correctly.";

            GetDistanceFromSensor(pin_trig_sensor_1, pin_echo_sensor_1);
            GetDistanceFromSensor(pin_trig_sensor_2, pin_echo_sensor_2);

            Task.Delay(50);

            Parallel.Invoke(
                () => standardDistanceSensro1 = GetDistanceFromSensor(pin_trig_sensor_1, pin_echo_sensor_1),
                () => standardDistanceSensro2 = GetDistanceFromSensor(pin_trig_sensor_2, pin_echo_sensor_2)
            );

            Task.Delay(50);

        }

        public static class GPIOExtention
        {

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

    public class Sensor
    {
        List<double> mesuringList = new List<double>();

        private const int MAX_VALUE = 300;
        private const int MAX_QUEUE = 20;

        public void AddMesure(double mesureValue)
        {
            if (mesureValue < 0 || mesureValue > MAX_VALUE)
                return;

            mesuringList.Add(mesureValue);

            try
            {
                if (mesuringList.Count > MAX_QUEUE)
                    mesuringList.RemoveAt(0);
            }
            catch { }

        }

        public double Distance
        {
            get
            {
                if (!mesuringList.Any())
                    return 0;

                if (mesuringList.Count < MAX_QUEUE / 2)
                {
                    return mesuringList.Average();
                }
                else
                {
                    var listWithoutExtremeValues = mesuringList;
                    var maxMesure = listWithoutExtremeValues.Max();
                    var minMesure = listWithoutExtremeValues.Min();

                    /*
                    do
                    {
                        listWithoutExtremeValues.Remove(listWithoutExtremeValues.Max());
                        listWithoutExtremeValues.Remove(listWithoutExtremeValues.Min());
                    } while (counter++ < (int)(MAX_QUEUE * 0.2));
                    */

                    if (listWithoutExtremeValues.Count(m => m == maxMesure) == 1)
                    {
                        listWithoutExtremeValues.Remove(maxMesure);
                        maxMesure = mesuringList.Max();
                        if (listWithoutExtremeValues.Count(m => m == maxMesure) == 1)
                            listWithoutExtremeValues.Remove(maxMesure);
                    }

                    if (listWithoutExtremeValues.Count(m => m == minMesure) == 1)
                    {
                        listWithoutExtremeValues.Remove(minMesure);
                        minMesure = mesuringList.Min();
                        if (listWithoutExtremeValues.Count(m => m == minMesure) == 1)
                            listWithoutExtremeValues.Remove(minMesure);
                    }
                    
                    return listWithoutExtremeValues.Any() ?
                        listWithoutExtremeValues.Average() :
                        mesuringList.Average();
                }
            }
        }
    }
}


