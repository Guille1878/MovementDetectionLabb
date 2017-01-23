using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System.Linq;
using HC_SR04_DistanceSensor;
using System.Threading.Tasks;

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

        DispatcherTimer timerMesuring;
        DispatcherTimer timerReading;

        private const int PIN_TRIG_SENSOR_1 = 22;
        private const int PIN_ECHO_SENSOR_1 = 27;

        private const int PIN_TRIG_SENSOR_2 = 24;
        private const int PIN_ECHO_SENSOR_2 = 23;

        private const int PIN_TRIG_SENSOR_3 = 6;
        private const int PIN_ECHO_SENSOR_3 = 13;

        private const int PIN_TRIG_SENSOR_4 = 12;
        private const int PIN_ECHO_SENSOR_4 = 16;

        HCSR04[] sensors = new HCSR04[]
        {
            new HCSR04("Sensor 1"),
            new HCSR04("Sensor 2"),
            new HCSR04("Sensor 3"),
            new HCSR04("Sensor 4")
        };

        private short countintDistanceSensorNumber = 0;
        private IHCSR04 countintDistanceSensor;
        public DateTime? StardedSettingStandardDistances { get; private set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            

        }

        private void TimerReading_Tick(object sender, object e)
        {
            try
            {
                    TextBlockDistanceSensro1Value.Text = sensors[0].MeasuredDistance.ToString() + " cm";
                    TextBlockDistanceSensro2Value.Text = sensors[1].MeasuredDistance.ToString() + " cm";
                    //TextBlockDistanceSensro3Value.Text = sensors[2].MeasuredDistance.ToString() + " cm";
                    //TextBlockDistanceSensro4Value.Text = sensors[3].MeasuredDistance.ToString() + " cm";
             
            }
            catch (Exception ex)
            {
                TextBlockMessage.Text = ex.ToString();
            }

        }

        Stopwatch timing;

        private void TimerMesuring_Tick(object sender, object e)
        {
            try
            {

                TextBlockTimeValue.Text = timing.Elapsed.ToString();

                if (StardedSettingStandardDistances != null)
                {
                    var textBlockStandardDistanceSensro = (TextBlock)FindName($"textBlockStandardDistanceSensro{countintDistanceSensorNumber}Value");
                    if ((countingStandardDistance != countintDistanceSensor.StandardDistance) || (DateTime.Now - StardedSettingStandardDistances.Value).TotalSeconds > 90)
                    {
                        countingStandardDistance = countintDistanceSensor.StandardDistance;

                        textBlockStandardDistanceSensro.Text = Math.Round(countingStandardDistance, 1).ToString() + " cm";

                        switch (countintDistanceSensorNumber)
                        {
                            case 1:
                                countintDistanceSensorNumber = 2;
                                countintDistanceSensor = sensors[1];
                                countintDistanceSensor.StartSearchingStandardDistance();
                                countingStandardDistance = standardDistance2;
                                break;
                            case 2:
                                countintDistanceSensorNumber = 3;
                                countintDistanceSensor = sensors[2];
                                countintDistanceSensor.StartSearchingStandardDistance();
                                countingStandardDistance = standardDistance3;
                                break;
                            case 3:
                                countintDistanceSensorNumber = 4;
                                countintDistanceSensor = sensors[3];
                                countintDistanceSensor.StartSearchingStandardDistance();
                                countingStandardDistance = standardDistance4;
                                break;
                            case 4:
                                StardedSettingStandardDistances = null;
                                return;
                        }
                        StardedSettingStandardDistances = DateTime.Now;

                    }
                    else
                    {
                        if (points == ".")
                            points = "..";
                        else if (points == "..")
                            points = "...";
                        else
                            points = ".";

                        textBlockStandardDistanceSensro.Text = points;
                    }
                }


                //Parallel.ForEach(sensors.Where(s => s.ArePinsInitialized), sensor =>
                //{
                //    sensor.AddMesure();
                //});
                foreach (var sensor in sensors.Where(s => s.ArePinsInitialized))
                {
                    sensor.AddMesure();
                }

            }
            catch (Exception ex)
            {
                TextBlockMessage.Text = ex.ToString();
            }
        }

        string points = ".";

        private void ButtonGetStandardDistances_Click(object sender, RoutedEventArgs e)
        {
            countintDistanceSensorNumber = 1;
            countintDistanceSensor = sensors[0];
            countintDistanceSensor.StartSearchingStandardDistance();
            countingStandardDistance = standardDistance1;
            StardedSettingStandardDistances = DateTime.Now;

        }

        private double
            countingStandardDistance = -1,
            standardDistance1 = -1,
            standardDistance2 = -1,
            standardDistance3 = -1,
            standardDistance4 = -1;

        private void ButtonStartCounting_Copy_Click(object sender, RoutedEventArgs e)
        {
            timerMesuring = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timerMesuring.Tick += TimerMesuring_Tick;

            timerReading = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            timerReading.Tick += TimerReading_Tick;

            try
            {

                sensors[0].InitializePins(PIN_TRIG_SENSOR_1, PIN_ECHO_SENSOR_1);
                sensors[1].InitializePins(PIN_TRIG_SENSOR_2, PIN_ECHO_SENSOR_2);
                //sensors[2].InitializePins(PIN_TRIG_SENSOR_3, PIN_ECHO_SENSOR_3);
                //sensors[3].InitializePins(PIN_TRIG_SENSOR_4, PIN_ECHO_SENSOR_4);

                Task.Delay(1000);
                //if (!(sensors.Any(s => !s.ArePinsInitialized)))
                //{
                timing = Stopwatch.StartNew();

                //foreach (var sensor in sensors.Where(s => s.ArePinsInitialized))
                //{
                //    sensor.StartMeasuring();
                //}

                timerMesuring.Start();
                timerReading.Start();
                //}

            }
            catch (Exception ex)
            {
                TextBlockMessage.Text = ex.Message;
            }
        }

        private void ButtonStartCounting_Click(object sender, RoutedEventArgs e)
        {
            new TwoSensorsDetectingMovementDirection(sensors[0], sensors[1], ref TextBlockInValue, ref TextBlockOutValue,ref TextBlockTotalInsideValue, ref TextBlockMessage);
            //new TwoSensorsDetectingMovementDirection(sensors[0], sensors[1], ref TextBlockInValue, ref TextBlockOutValue, ref TextBlockTotalInsideValue);
        }
    }
}


