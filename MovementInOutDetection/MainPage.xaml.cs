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

        private const int PIN_TRIG_SENSOR_2 = 2;
        private const int PIN_ECHO_SENSOR_2 = 3;


        HC_SR04_DistanceSensor sensor1 = new HC_SR04_DistanceSensor("Sensor 1");
        HC_SR04_DistanceSensor sensor2 = new HC_SR04_DistanceSensor("Sensor 2");

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += Timer_Tick;

            try
            {

                sensor1.InitializePins(PIN_TRIG_SENSOR_1, PIN_ECHO_SENSOR_1);
                sensor2.InitializePins(PIN_TRIG_SENSOR_2, PIN_ECHO_SENSOR_2);

                if (sensor1.ArePinsInitialized &&
                    sensor2.ArePinsInitialized)
                {
                    //timing = Stopwatch.StartNew();

                    sensor1.StartMesuring();
                    sensor2.StartMesuring();

                    timer.Start();
                }

            }
            catch (Exception ex)
            {
                textBlockMessage.Text = ex.Message;
            }

        }


        //Stopwatch timing;

        private void Timer_Tick(object sender, object e)
        {
            try
            {

                //textBlockTimeValue.Text = timing.Elapsed.ToString();

                textBlockDistanceSensro1Value.Text = Math.Round(sensor1.Distance, 1).ToString() + " cm";
                textBlockDistanceSensro2Value.Text = Math.Round(sensor2.Distance, 1).ToString() + " cm";

                if (StardedSettingStandardDistances != null)
                {
                    if ((standardDistance1 != sensor1.StandardDistance && standardDistance2 != sensor2.StandardDistance) || (DateTime.Now - StardedSettingStandardDistances.Value).TotalSeconds > 90)
                    {
                        standardDistance1 = sensor1.StandardDistance;
                        standardDistance2 = sensor2.StandardDistance;

                        textBlockStandardDistanceSensro1Value.Text = Math.Round(standardDistance1, 1).ToString() + " cm";
                        textBlockStandardDistanceSensro2Value.Text = Math.Round(standardDistance2, 1).ToString() + " cm";

                        StardedSettingStandardDistances = null;
                    }
                    else
                    {
                        if (points == ".")
                            points = "..";
                        else if (points == "..")
                            points = "...";
                        else
                            points = ".";

                        textBlockStandardDistanceSensro1Value.Text = textBlockStandardDistanceSensro2Value.Text = points;
                    }
                }

            }
            catch (Exception ex)
            {
                textBlockMessage.Text = ex.ToString();
            }
        }

        string points = ".";

        private void buttonGetStandardDistances_Click(object sender, RoutedEventArgs e)
        {
            sensor1.SetStandardDistance();
            sensor2.SetStandardDistance();

            StardedSettingStandardDistances = DateTime.Now;

        }

        private double standardDistance1 = -1;
        private double standardDistance2 = -1;

        public DateTime? StardedSettingStandardDistances { get; private set; }

    }
}


