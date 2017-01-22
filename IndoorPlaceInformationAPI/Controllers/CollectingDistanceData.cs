using IndoorPlaceInformationAPI.Models;
using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace IndoorPlaceInformationAPI.Controllers
{
    internal static class CollectingDistanceData
    {

        private static QueueClient client;
        private static ServiceClient serviceClient;
        private static readonly string deviceId = @"swedaviaiotlabbId";
            //"WillesRaspbarryDevId";
        private static readonly string deviceConnectionString = @"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
        //@"HostName=WillesIotHub1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=4YffuSTgyX5vHqeYaS+sy6uacKDxtuEktHh+rGZeEbM=";

        private static readonly string connectionString = @"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
        private static readonly string queueName = "DistanceMeasures";

        internal static void Initialize()
        {
            serviceClient = ServiceClient.CreateFromConnectionString(deviceConnectionString);
            client = QueueClient.CreateFromConnectionString(connectionString, queueName);
        }
        private static Sensor temporarySensor;

        private static Dictionary<Sensor, List<DistanceMeasure>> SensorDistanceMeasures = new Dictionary<Sensor, List<DistanceMeasure>>();
        internal static async Task StartDetectingPlaceAsync(Guid placeId)
        {
            try
            {
                
                var place = new Place(placeId);

                foreach (var entrance in place.Entrance)
                {
                    foreach (var board in entrance.SensorBoards)
                    {
                        temporarySensor = board.InSensor;
                        SensorDistanceMeasures.Add(board.InSensor, new List<DistanceMeasure>());
                        SensorDistanceMeasures.Add(board.OutSensor, new List<DistanceMeasure>());
                    }
                }
                await SendCloudToDeviceMessageAsync("#START");

                await Task.Delay(1000);

                //"sb://iothub-ns-swedaviala-105751-d1c5f83d17.servicebus.windows.net/";
                //@"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";


                AddEventOnMessage();

            }
            catch (Exception ex)
            {
                var vad = ex.Message;
                throw;
            }
        }

        internal static void AddEventOnMessage()
        {
            try
            {


                client.OnMessage(message =>
                {
                    Stream stream = message.GetBody<Stream>();
                    StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                    string messageFormated = reader.ReadToEnd();
                    foreach (var deviceSensor in messageFormated.Split('#'))
                    {
                        var deviceSensorInfo = deviceSensor.Split('|');
                        if (deviceSensorInfo.Count() == 2)
                        {
                            var sensorId = deviceSensorInfo[0];
                            var distanceValue = double.Parse(deviceSensorInfo[1]);
                            SensorDistanceMeasures[temporarySensor].Add(new DistanceMeasure(distanceValue, message.EnqueuedTimeUtc));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                var vad = ex.Message;
                throw;
            }
        }

        internal static async void StopDetectingPlaceAsync(Guid placeId)
        {
            var place = new Place(placeId);

            foreach (var entrance in place.Entrance)
            {
                foreach (var board in entrance.SensorBoards)
                {
                    SensorDistanceMeasures.Add(board.InSensor, new List<DistanceMeasure>());
                    SensorDistanceMeasures.Add(board.OutSensor, new List<DistanceMeasure>());
                }
            }
            await SendCloudToDeviceMessageAsync("#STOP");
                        
        }

        private static async Task SendCloudToDeviceMessageAsync(string message)
        {
            try
            {


                var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
                commandMessage.Properties.Add("Direction", "To Raspberry with Distance Sensors");
                await serviceClient.SendAsync(deviceId, commandMessage);
            }
            catch (Exception ex)
            {
                var vad = ex.Message;
                throw;
            }
        }
    }
       

    internal class DistanceMeasure
    {
        internal DistanceMeasure(double distance, DateTime timeStamp)
        {
            Distance = distance;
            Created = timeStamp;
        }
        internal double Distance { get; }
        internal DateTime Created { get; }
    }
}