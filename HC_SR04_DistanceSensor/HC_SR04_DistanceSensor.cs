using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.System.Threading;
//using Windows.UI.Xaml;
using static GPIOExtention.GPIO;

namespace HC_SR04_DistanceSensor
{
    public sealed class HCSR04 : IHCSR04
    {
        public HCSR04()
        {
            this.Name = "";
            Id = Guid.NewGuid();
        }
        public HCSR04(string name)
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

        public Guid Id { get; private set; }

        //DispatcherTimer timer_measuring;
        ThreadPoolTimer timer_measuring;
        bool IsSettingStandard = false;
        int StandardSetupingTicksCounter = 0;

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
            Pin_trig.DebounceTimeout = TimeSpan.FromMilliseconds(SensorMesuringSetting.DebounceTimeout);

            Pin_echo = gpio.OpenPin(pin_echo);
            Pin_echo.SetDriveMode(GpioPinDriveMode.Input);
            Pin_echo.DebounceTimeout = TimeSpan.FromMilliseconds(SensorMesuringSetting.DebounceTimeout);

            Delay(50);
        }

        public void InitializePins(Guid id, string name, int pin_trig, int pin_echo)
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Pin_trig = Pin_echo = null;
                throw new Exception("There is no GPIO controller on this device.");
            }

            this.Id = id;
            this.Name = name;

            Pin_trig = gpio.OpenPin(pin_trig);
            Pin_trig.SetDriveMode(GpioPinDriveMode.Output);
            Pin_trig.DebounceTimeout = TimeSpan.FromMilliseconds(SensorMesuringSetting.DebounceTimeout);

            Pin_echo = gpio.OpenPin(pin_echo);
            Pin_echo.SetDriveMode(GpioPinDriveMode.Input);
            Pin_echo.DebounceTimeout = TimeSpan.FromMilliseconds(SensorMesuringSetting.DebounceTimeout);

            Delay(50);
        }

        public void StartMeasuring()
        {
            if (ArePinsInitialized)
            {
                timer_measuring = ThreadPoolTimer.CreatePeriodicTimer(Timer_measuringTimerElapsedHandler, TimeSpan.FromMilliseconds(SensorMesuringSetting.MeasuringTimerInterval));

                /*
                timer_measuring = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(SensorMesuringSetting.MeasuringTimerInterval),
                };

                timer_measuring.Tick += Timer_measuring_Tick;
                timer_measuring.Start();
                */
            }
            else
                throw new Exception("The pins are not initialized yet.");
        }

        private void Timer_measuringTimerElapsedHandler(ThreadPoolTimer timer)
        {
            try
            {
                if (IsSettingStandard)
                {
                    StandardSetupingTicksCounter++;
                    if (StandardSetupingTicksCounter > SensorMesuringSetting.SetupingTicksCounterMaxItmes)
                    {
                        StandardSetupingTicksCounter = 0;
                        if (mesuringStandardList.Count == SensorMesuringSetting.SetupingTicksCounterMaxItmes)
                        {
                            if (mesuringStandardList.Count(m => m == mesuringStandardList.Max()) == 1)
                            {
                                mesuringStandardList.Remove(mesuringStandardList.Max());
                            }

                            if (mesuringStandardList.Count(m => m == mesuringStandardList.Min()) == 1)
                            {
                                mesuringStandardList.Remove(mesuringStandardList.Min());
                            }

                            var arroundList = mesuringStandardList
                                .Select(m => Math.Round(m, 0))
                                .GroupBy(m => m).Select(g => new
                                {
                                    Mesure = g.Key,
                                    Count = g.Count()
                                }).ToList();
                            var maxCountValues = arroundList.First(m => m.Count == arroundList.Max(am => am.Count)).Mesure;
                            StandardDistance = Math.Round(arroundList
                                .Where(m => m.Mesure > maxCountValues - SensorMesuringSetting.AverageCalculatingMarginal
                                            && m.Mesure < maxCountValues + SensorMesuringSetting.AverageCalculatingMarginal)
                                .Average(m => m.Mesure), 0);

                            mesuringStandardList.Clear();
                            IsSettingStandard = false;
                        }
                        else
                            mesuringStandardList.Add(GetDistance());
                    }
                }

                Task.Run(() => AddMesure());
            }
            catch { }
        }

        private void Timer_measuring_Tick(object sender, object e)
        {
            try
            {
                if (IsSettingStandard)
                {
                    StandardSetupingTicksCounter++;
                    if (StandardSetupingTicksCounter > SensorMesuringSetting.SetupingTicksCounterMaxItmes)
                    {
                        StandardSetupingTicksCounter = 0;
                        if (mesuringStandardList.Count == SensorMesuringSetting.SetupingTicksCounterMaxItmes)
                        {
                            if (mesuringStandardList.Count(m => m == mesuringStandardList.Max()) == 1)
                            {
                                mesuringStandardList.Remove(mesuringStandardList.Max());
                            }

                            if (mesuringStandardList.Count(m => m == mesuringStandardList.Min()) == 1)
                            {
                                mesuringStandardList.Remove(mesuringStandardList.Min());
                            }

                            var arroundList = mesuringStandardList
                                .Select(m => Math.Round(m, 0))
                                .GroupBy(m => m).Select(g => new
                                {
                                    Mesure = g.Key,
                                    Count = g.Count()
                                }).ToList();
                            var maxCountValues = arroundList.First(m => m.Count == arroundList.Max(am => am.Count)).Mesure;
                            StandardDistance = Math.Round(arroundList
                                .Where(m => m.Mesure > maxCountValues - SensorMesuringSetting.AverageCalculatingMarginal 
                                            && m.Mesure < maxCountValues + SensorMesuringSetting.AverageCalculatingMarginal)
                                .Average(m => m.Mesure), 0);

                            mesuringStandardList.Clear();
                            IsSettingStandard = false;
                        }
                        else
                            mesuringStandardList.Add(GetDistance());
                    }
                }

                Task.Run(() => AddMesure());
            }
            catch { }
        }      

        List<double> mesuringStandardList = new List<double>();
        List<double> mesuringList = new List<double>();

        public double StandardDistance { get; set; } = -1;

        public GpioPin Pin_trig { get; private set; }
        public GpioPin Pin_echo { get; private set; }

        public string Name { get; private set; }

        public void AddMesure()
        {
            AddMesure(GetDistance());
        }

        public void AddMesure(double mesureValue)
        {

            try
            {
                if (mesureValue < 0 || mesureValue > SensorMesuringSetting.MaxValueForDistance)
                    return;

                mesuringList.Add(mesureValue);

                lock (mesuringList)
                {
                    if (mesuringList.Count > SensorMesuringSetting.MaxItemInCalculatingQueue)
                        mesuringList.RemoveAt(0);
                }
            }
            catch
            {
            }

        }

        public void StartSearchingStandardDistance()
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
                    reslut = duration * SensorMesuringSetting.DurationFactorForDistanceClaculation;

            }
            catch {}

            return reslut;
        }

       

        public double MeasuredDistance
        {
            get
            {
                if (!mesuringList.Any())
                    return 0;

                if (mesuringList.Count < SensorMesuringSetting.MaxItemInCalculatingQueue)
                {
                    return Math.Round(mesuringList.Average(),1);
                }
                else
                {
                    List<double> listWithoutExtremeValues;
                    lock (mesuringList)
                    {
                        listWithoutExtremeValues = mesuringList.ToList();
                    }
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

                    if (listWithoutExtremeValues.Any())
                    {
                        //return Math.Round(listWithoutExtremeValues.Average(),1);

                        var arroundList = listWithoutExtremeValues
                                    .Select(m => Math.Round(m, 0))
                                    .GroupBy(m => m).Select(g => new
                                    {
                                        Mesure = g.Key,
                                        Count = g.Count()
                                    })
                                    .ToList();

                        var maxCountValues = arroundList
                            .First(m => m.Count == arroundList.Max(am => am.Count))
                            .Mesure;

                        return Math.Round(
                            arroundList
                            .Where(m => m.Mesure > maxCountValues - SensorMesuringSetting.AverageCalculatingMarginal 
                                        && m.Mesure < maxCountValues + SensorMesuringSetting.AverageCalculatingMarginal)
                            .Average(m => m.Mesure)
                            , 1);
                    }

                    return Math.Round(mesuringList.Average(),1);
                }
            }
        }

        private SensorMesuringSetting sensorMesuringSetting = new SensorMesuringSetting();
        public SensorMesuringSetting SensorMesuringSetting
        {
            get
            {
                return sensorMesuringSetting;
            }
        }        
    }

    public sealed class SensorMesuringSetting
    {
        public int DebounceTimeout { get; set; } = 100;
        public int SetupingTicksCounterMaxItmes { get; set; } = 20;
        public double MeasuringTimerInterval { get; set; } = 50;
        public double AverageCalculatingMarginal { get; set; } = 10;
        public int MaxValueForDistance { get; set; }= 300;
        public int MaxItemInCalculatingQueue { get; set; } = 30;
        public double DurationFactorForDistanceClaculation { get; set; } = 17900;//19000; // 18500;//17150; //17900;

    }
}
