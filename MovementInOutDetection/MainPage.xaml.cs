using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

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
        private const int PIN_ECHO_SENSOR_1 = 27;

        private const int PIN_TRIG_SENSOR_2 = 24;
        private const int PIN_ECHO_SENSOR_2 = 23;

        private const int PIN_TRIG_SENSOR_3 = 19;
        private const int PIN_ECHO_SENSOR_3 = 26;

        private const int PIN_TRIG_SENSOR_4 = 16;
        private const int PIN_ECHO_SENSOR_4 = 20;


        HC_SR04_DistanceSensor sensor1 = new HC_SR04_DistanceSensor("Sensor 1");
        HC_SR04_DistanceSensor sensor2 = new HC_SR04_DistanceSensor("Sensor 2");
        HC_SR04_DistanceSensor sensor3 = new HC_SR04_DistanceSensor("Sensor 3");
        HC_SR04_DistanceSensor sensor4 = new HC_SR04_DistanceSensor("Sensor 4");

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;

            try
            {

                sensor1.InitializePins(PIN_TRIG_SENSOR_1, PIN_ECHO_SENSOR_1);
                sensor2.InitializePins(PIN_TRIG_SENSOR_2, PIN_ECHO_SENSOR_2);
                sensor3.InitializePins(PIN_TRIG_SENSOR_3, PIN_ECHO_SENSOR_3);
                sensor4.InitializePins(PIN_TRIG_SENSOR_4, PIN_ECHO_SENSOR_4);

                if (sensor1.ArePinsInitialized 
                    && sensor2.ArePinsInitialized
                    && sensor3.ArePinsInitialized 
                    && sensor4.ArePinsInitialized
                    )
                {
                    timing = Stopwatch.StartNew();

                    sensor1.StartMeasuring();
                    sensor2.StartMeasuring();
                    sensor3.StartMeasuring();
                    sensor4.StartMeasuring();

                    timer.Start();
                }

            }
            catch (Exception ex)
            {
                textBlockMessage.Text = ex.Message;
            }

        }


        Stopwatch timing;

        private void Timer_Tick(object sender, object e)
        {
            try
            {

                textBlockTimeValue.Text = timing.Elapsed.ToString();

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
                                countintDistanceSensor = sensor2;
                                countintDistanceSensor.StartSearchingStandardDistance();
                                countingStandardDistance = standardDistance2;
                                break;
                            case 2:
                                countintDistanceSensorNumber = 3;
                                countintDistanceSensor = sensor3;
                                countintDistanceSensor.StartSearchingStandardDistance();
                                countingStandardDistance = standardDistance3;
                                break;
                            case 3:
                                countintDistanceSensorNumber = 4;
                                countintDistanceSensor = sensor4;
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
                    textBlockDistanceSensro1Value.Text = Math.Round(sensor1.MeasuredDistance, 1).ToString() + " cm";
                    textBlockDistanceSensro2Value.Text = Math.Round(sensor2.MeasuredDistance, 1).ToString() + " cm";
                    textBlockDistanceSensro3Value.Text = Math.Round(sensor3.MeasuredDistance, 1).ToString() + " cm";
                    textBlockDistanceSensro4Value.Text = Math.Round(sensor4.MeasuredDistance, 1).ToString() + " cm";
                
            }
            catch (Exception ex)
            {
                textBlockMessage.Text = ex.ToString();
            }
        }

        string points = ".";

        private void buttonGetStandardDistances_Click(object sender, RoutedEventArgs e)
        {
            countintDistanceSensorNumber = 1;
            countintDistanceSensor = sensor1;
            countintDistanceSensor.StartSearchingStandardDistance();
            countingStandardDistance = standardDistance1;
            StardedSettingStandardDistances = DateTime.Now;

        }

        private short countintDistanceSensorNumber = 0;
        private HC_SR04_DistanceSensor countintDistanceSensor;

        private double
            countingStandardDistance = -1,
            standardDistance1 = -1,
            standardDistance2 = -1,
            standardDistance3 = -1,
            standardDistance4 = -1;

        public DateTime? StardedSettingStandardDistances { get; private set; }

    }
}


