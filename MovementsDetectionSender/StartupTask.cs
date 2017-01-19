using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Microsoft.Azure.Devices.Client;
using System.Diagnostics;
using HC_SR04_DistanceSensor;
using System.Threading.Tasks;
//using SendingDataToAzure;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace MovementsDetectionSender
{
    public sealed class StartupTask : IBackgroundTask
    {

        BackgroundTaskDeferral _deferral;

        internal static readonly string deviceId = "swedaviaiotlabbId";
        internal static readonly string deviceConnectionString = @"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
                                                               //@"HostName=swedavialabbHub.azure-devices.net;DeviceId=swedaviaiotlabbId;SharedAccessKey=nmBFrPYhac13PuyCQyarRcFlVOpYjNEevY/8ytl5PaM=";
                                                               //@"HostName=WillesIotHub1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=4YffuSTgyX5vHqeYaS+sy6uacKDxtuEktHh+rGZeEbM=";


        //private SendingData sendingToAzure;
        private DeviceClient deviceIot;

        private const int PIN_TRIG_SENSOR_1 = 22;
        private const int PIN_ECHO_SENSOR_1 = 27;

        private const int PIN_TRIG_SENSOR_2 = 24;
        private const int PIN_ECHO_SENSOR_2 = 23;

        private const int PIN_TRIG_SENSOR_3 = 19;
        private const int PIN_ECHO_SENSOR_3 = 26;

        private const int PIN_TRIG_SENSOR_4 = 16;
        private const int PIN_ECHO_SENSOR_4 = 20;

        HCSR04[] sensors = new HCSR04[]
        {
            new HCSR04("Sensor 1"),
            new HCSR04("Sensor 2"),
            new HCSR04("Sensor 3"),
            new HCSR04("Sensor 4")
        };

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            //sendingToAzure = new SendingData(deviceId, deviceConnectionString);
            deviceIot = DeviceClient.CreateFromConnectionString(deviceConnectionString, deviceId, TransportType.Http1);

            ReceiveCommandsAsync();


        }

        public async void ReceiveCommandsAsync()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            while (true)
            {
                if (stopWatch.ElapsedMilliseconds > 2000)
                {
                    stopWatch.Stop();
                    Message receivedMessage = await deviceIot.ReceiveAsync();

                    if (receivedMessage == null)
                    {
                        stopWatch.Restart();
                        continue;
                    }

                    if (receivedMessage.Properties.ContainsKey("Direction"))
                        if (receivedMessage.Properties["Direction"].Equals("To Raspberry with Distance Sensors"))
                        {
                            if ((DateTime.Now - receivedMessage.EnqueuedTimeUtc).TotalMinutes > 1)
                                continue;

                            string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                            if (messageData.Contains("#START"))
                            {
                                
                                sensors[0].InitializePins(PIN_TRIG_SENSOR_1, PIN_ECHO_SENSOR_1);
                                sensors[1].InitializePins(PIN_TRIG_SENSOR_2, PIN_ECHO_SENSOR_2);
                                sensors[2].InitializePins(PIN_TRIG_SENSOR_3, PIN_ECHO_SENSOR_3);
                                sensors[3].InitializePins(PIN_TRIG_SENSOR_4, PIN_ECHO_SENSOR_4);

                                isSendingData = true;
                                SendMeasuresAsync();
                            }
                            else if (messageData.Contains("#STOP"))
                            {
                                isSendingData = false;
                            }
                            await deviceIot.CompleteAsync(receivedMessage);
                        }
                    stopWatch.Restart();
                }
            }
        }

        private bool isSendingData = false;

        public async void SendMeasuresAsync()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            while (isSendingData)
            {
                if (stopWatch.ElapsedMilliseconds > 50)
                {
                    stopWatch.Stop();

                    Dictionary<Guid, double> sensorsDistance = new Dictionary<Guid, double>();
                    Parallel.ForEach(sensors, sensor => {
                        sensorsDistance.Add(sensor.Id, sensor.GetDistance());
                    });

                    string message = String.Join("#", sensorsDistance.Select(sd => sd.Key.ToString() + "|" + sd.Value.ToString()).ToArray());
                    var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
                    commandMessage.Properties.Add("Direction", "From Raspberry with Distance Sensors To Swagger API");
                    await deviceIot.SendEventAsync(commandMessage);
                    stopWatch.Restart();
                }
            }
        }
    }
}
