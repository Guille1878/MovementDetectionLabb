using HC_SR04_DistanceSensor;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MovementsDetectionSender
{
    public sealed class Measuring
    {

        private const int PIN_TRIG_SENSOR_1 = 22;
        private const int PIN_ECHO_SENSOR_1 = 27;

        private const int PIN_TRIG_SENSOR_2 = 24;
        private const int PIN_ECHO_SENSOR_2 = 23;

        private const double DistanceMarginal = 20;

        internal static readonly string deviceId = "swedaviaiotlabbId";
        internal static readonly string deviceConnectionString = @"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";

        private static DeviceClient deviceIot;
        private static HCSR04[] Sensors { get; set; }
        private static bool isSearchingStandardDistance;
        private static Dictionary<Guid, List<double>> SensorMeasuresForStandarSearching = new Dictionary<Guid, List<double>>();

        private static List<Lapse> MeasuringsElapsed = new List<Lapse>();
        
        private static int countingTiks = 0;
        private static int SleepingInterval = 100;

        public static void StartSendingMeasuresAsync()
        {

            try
            {

                var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "CF2D4313-33DE-489D-9721-6AFF69841DEA", out bool createdNew);
                var signaled = false;

                if (!createdNew)
                {
                    waitHandle.Set();
                    return;
                }

                Sensors = new HCSR04[]
                {
                    new HCSR04("Sensor Inside"),
                    new HCSR04("Sensor Outside")
                };

                deviceIot = DeviceClient.CreateFromConnectionString(deviceConnectionString, deviceId, TransportType.Http1);

                isSearchingStandardDistance = true;

                Sensors[0].InitializePins(PIN_TRIG_SENSOR_1, PIN_ECHO_SENSOR_1);
                Sensors[1].InitializePins(PIN_TRIG_SENSOR_2, PIN_ECHO_SENSOR_2);

                InfoMessageAsync($"Sensors are initialized");

                signaled = waitHandle.WaitOne(TimeSpan.FromSeconds(2));

                SensorMeasuresForStandarSearching.Add(Sensors[0].Id, new List<double>());
                SensorMeasuresForStandarSearching.Add(Sensors[1].Id, new List<double>());

                do
                {
                    
                    try
                    {
                        signaled = waitHandle.WaitOne(SleepingInterval);

                        foreach (var sensor in Sensors)
                            sensor.AddMesure();

                        if (isSearchingStandardDistance)
                        {
                            //await Task.Delay(SleepingInterval);
                            if (++countingTiks > 50)
                            {
                                countingTiks = 0;
                                foreach (var sensor in Sensors)
                                {
                                    if (!sensor.ArePinsInitialized)
                                        break;

                                    SensorMeasuresForStandarSearching[sensor.Id].Add(sensor.MeasuredDistance);
                                }

                                if (SensorMeasuresForStandarSearching.First().Value.Count > 4)
                                {
                                    foreach (var sensor in Sensors)
                                        sensor.StandardDistance = SensorMeasuresForStandarSearching[sensor.Id].Average();

                                    SensorMeasuresForStandarSearching = null;

                                    SleepingInterval = 20;
                                    isSearchingStandardDistance = false;

                                    InfoMessageAsync($"DONE! Standard values taken. ex: { Sensors.First().StandardDistance.ToString()}, {Sensors.Last().StandardDistance.ToString()}");
                                }
                                else
                                {
                                    InfoMessageAsync($"One standard value catched. {SensorMeasuresForStandarSearching.First().Value.Last().ToString()}, {SensorMeasuresForStandarSearching.Last().Value.Last().ToString()}");
                                }
                            }
                        }
                        else
                        {

                            foreach (var sensor in Sensors)
                            {
                                if (sensor.MeasuredDistance < sensor.StandardDistance - DistanceMarginal)
                                {
                                    if (!MeasuringsElapsed.Any(l => l.IsStillOpen && l.SensorId.Equals(sensor.Id)))
                                    {
                                        MeasuringsElapsed.Add(new Lapse(sensor.Id));
                                    }

                                }
                                else
                                {
                                    if (MeasuringsElapsed.Any(l => l.IsStillOpen && l.SensorId.Equals(sensor.Id)))
                                    {
                                        MeasuringsElapsed.First(l => l.IsStillOpen && l.SensorId.Equals(sensor.Id)).CloseLapse();
                                        CalculateMeasuringsElapsed();
                                    }
                                }
                            }


                            /*
                            Parallel.ForEach(new int[2] { 0, 1 }, sensorIndex =>
                            {
                                if (Sensors[sensorIndex].MeasuredDistance < Sensors[sensorIndex].StandardDistance - DistanceMarginal)
                                {
                                    if (!MeasuringsElapsed.Any(l => l.IsStillOpen && l.SensorId.Equals(Sensors[sensorIndex].Id)))
                                        MeasuringsElapsed.Add(new Lapse(Sensors[sensorIndex].Id));
                                }
                                else
                                {
                                    if (MeasuringsElapsed.Any(l => l.IsStillOpen && l.SensorId.Equals(Sensors[sensorIndex].Id)))
                                    {
                                        MeasuringsElapsed.First(l => l.IsStillOpen && l.SensorId.Equals(Sensors[sensorIndex].Id)).CloseLapse();
                                        CalculateMeasuringsElapsed();
                                    }
                                }
                            });
                            */

                        }
                    }
                    catch (Exception ex)
                    {

                        ErrorMessageAsync(ex.ToString());

                    }
                } while (true); //while (!signaled);

            }
            catch (Exception ex)
            {
                ErrorMessageAsync(ex.ToString());

            }
        }            

        private static void CalculateMeasuringsElapsed()
        {
            try
            {


                var MeasuringsElapsedWorkingCopy = MeasuringsElapsed;
                if (MeasuringsElapsedWorkingCopy.Count(m => !m.IsStillOpen) == 2)
                {
                    if (MeasuringsElapsedWorkingCopy.Select(m => m.SensorId).Distinct().Count() == 2)
                    {
                        // Processing is here
                        var comingFrom = MeasuringsElapsedWorkingCopy.First(m => m.Started == MeasuringsElapsedWorkingCopy.Min(mm => mm.Started)).SensorId;
                        var passDuration = MeasuringsElapsedWorkingCopy.Min(m => m.Closed) - MeasuringsElapsedWorkingCopy.Min(m => m.Started);

                        SomebodyPassAsync(comingFrom == Sensors[0].Id ? PassDirection.In : PassDirection.Out);

                        MeasuringsElapsed.RemoveAll(m => MeasuringsElapsedWorkingCopy.Any(mwc => m.Id.Equals(mwc.Id)));
                                                                       
                    }
                    else
                    {
                        MeasuringsElapsed.RemoveAll(m => m.Closed != MeasuringsElapsedWorkingCopy.Max(mwc => mwc.Closed));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessageAsync(ex.ToString());
            }
        }

        private static async void SomebodyPassAsync(PassDirection direciton)
        {
            string message = $"Pass##{(int)direciton}";
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Properties.Add("Direction", "From Raspberry with Distance Sensors To Swagger API");
            await deviceIot.SendEventAsync(commandMessage);
        }

        private static async void ErrorMessageAsync(string exception)
        {
            string message = $"Error##{exception}";
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Properties.Add("Direction", "From Raspberry with Distance Sensors To Swagger API");
            await deviceIot.SendEventAsync(commandMessage);
        }

        private static async void InfoMessageAsync(string info)
        {
            string message = $"Info##{info}";
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Properties.Add("Direction", "From Raspberry with Distance Sensors To Swagger API");
            await deviceIot.SendEventAsync(commandMessage);
        }
    }
    enum PassDirection
    {
        In = 1,
        Out = -1
    }
}
