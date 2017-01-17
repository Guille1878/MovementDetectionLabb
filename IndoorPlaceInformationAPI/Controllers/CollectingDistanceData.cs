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
    internal class CollectingDistanceData
    {
        private static readonly ServiceClient serviceClient;
        private static readonly string deviceId = "WillesRaspbarryDevId";
        private static readonly string deviceConnectionString = @"HostName=WillesIotHub1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=4YffuSTgyX5vHqeYaS+sy6uacKDxtuEktHh+rGZeEbM=";



        private static Dictionary<Sensor, List<DistanceMeasure>> SensorDistanceMeasures = new Dictionary<Sensor, List<DistanceMeasure>>();
        internal static async void StartDetectingPlaceAsync(Guid placeId)
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
            await SendCloudToDeviceMessageAsync("#START");

            await Task.Delay(1000);

            var connectionString = "sb://iothub-ns-willesioth-82285-53001c7cc2.servicebus.windows.net/";
            var queueName = "DistanceMesures";

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

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
                        SensorDistanceMeasures[place.Entrance.First().SensorBoards.FirstOrDefault().InSensor].Add(new DistanceMeasure(distanceValue, message.EnqueuedTimeUtc));
                    }                    
                }
            });

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
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Properties.Add("Direction", "To Raspberry with Distance Sensors");
            await serviceClient.SendAsync(deviceId, commandMessage);
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