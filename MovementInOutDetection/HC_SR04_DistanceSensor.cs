using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using static MovementInOutDetection.GPIOExtention;

namespace MovementInOutDetection
{
    public class HC_SR04_DistanceSensor
    {

        Guid Id { get; }   
        public HC_SR04_DistanceSensor(string name = "")
        {
            this.Name = name;
            Id = Guid.NewGuid();
        }

        public bool ArePinsInitialized
        {
            get
            {
                return Pin_trig != null && Pin_echo != null;
            }
        }

        DispatcherTimer timer_mesuring;
        bool IsSettingStandard = false;
        int StandardSettingTiocksCounter = 0;

        public void InitializePins(int pin_trig, int pin_echo)
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Pin_trig = Pin_echo = null;
                throw new Exception("There is no GPIO controller on this device.");
            }

            Pin_trig = gpio.OpenPin(pin_trig);
            Pin_trig.SetDriveMode(GpioPinDriveMode.Output);

            Pin_echo = gpio.OpenPin(pin_echo);
            Pin_echo.SetDriveMode(GpioPinDriveMode.Input);

            Delay(50);
        }

        public void StartMesuring()
        {
            if (ArePinsInitialized)
            {
                timer_mesuring = new DispatcherTimer();
                timer_mesuring.Interval = TimeSpan.FromMilliseconds(5);
                timer_mesuring.Tick += Timer_mesuring_Tick;

                timer_mesuring.Start();
            }
            else
                throw new Exception("The pins are not initialized yet.");
        }

        private void Timer_mesuring_Tick(object sender, object e)
        {
            Task.Run(() => AddMesure());

            if (IsSettingStandard)
            {
                StandardSettingTiocksCounter++;
                if (StandardSettingTiocksCounter > 350)
                {
                    StandardSettingTiocksCounter = 0;
                    if (mesuringStandardList.Count == 5)
                    {
                        if (mesuringStandardList.Count(m => m == mesuringStandardList.Max()) == 1)
                        {
                            mesuringStandardList.Remove(mesuringStandardList.Max());
                        }

                        if (mesuringStandardList.Count(m => m == mesuringStandardList.Min()) == 1)
                        {
                            mesuringStandardList.Remove(mesuringStandardList.Min());
                        }
                        StandardDistance = mesuringStandardList.Average();
                        mesuringStandardList.Clear();
                        IsSettingStandard = false;
                    }
                    else
                        mesuringStandardList.Add(GetDistance());
                }
            }
        }        

        List<double> mesuringStandardList = new List<double>();

        public double StandardDistance { get; private set; } = -1;

        public GpioPin Pin_trig { get; set; }
        public GpioPin Pin_echo { get; set; }

        public string Name { get; private set; }

        List<double> mesuringList = new List<double>();

        private const int MAX_VALUE = 300;
        private const int MAX_QUEUE = 40;
        
        public void AddMesure()
        {
            var mesureValue = GetDistance();

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

        public void SetStandardDistance()
        {
            IsSettingStandard = true;
        }

        public double GetDistance()
        {

            double reslut = -1;

            try
            {

                Pin_trig.Write(GpioPinValue.Low);
                MicrosecondsDelay(2);

                Pin_trig.Write(GpioPinValue.High);
                MicrosecondsDelay(10);

                Pin_trig.Write(GpioPinValue.Low);

                double duration = PulseIn(Pin_echo);

                if (duration > -1)
                    reslut = duration * 17900;

            }
            catch {}

            return reslut;
        }

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

                if (mesuringList.Count < MAX_QUEUE)
                {
                    return mesuringList.Average();
                }
                else
                {
                    var listWithoutExtremeValues = mesuringList;
                    var maxMesure = listWithoutExtremeValues.Max();
                    var minMesure = listWithoutExtremeValues.Min();

                    if (listWithoutExtremeValues.Count(m => m == maxMesure) == 1)
                    {
                        listWithoutExtremeValues.Remove(maxMesure);
                        maxMesure = listWithoutExtremeValues.Max();
                        if (listWithoutExtremeValues.Count(m => m == maxMesure) == 1)
                            listWithoutExtremeValues.Remove(maxMesure);
                    }

                    if (listWithoutExtremeValues.Count(m => m == minMesure) == 1)
                    {
                        listWithoutExtremeValues.Remove(minMesure);
                        minMesure = listWithoutExtremeValues.Min();
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
