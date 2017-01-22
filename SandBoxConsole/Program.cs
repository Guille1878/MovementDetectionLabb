using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using System.Threading;

namespace SandBoxConsole
{
    class Program
    {
        static void Main(string[] args)
        {
     
            try
            {
                Initialize();

                var place = new Place(Guid.Parse("e2a96171-df08-438f-99a8-5bd0950b6dac"));

                foreach (var entrance in place.Entrance)
                {
                    foreach (var board in entrance.SensorBoards)
                    {
                        temporarySensor = board.InSensor;
                        SensorDistanceMeasures.Add(board.InSensor, new List<DistanceMeasure>());
                        SensorDistanceMeasures.Add(board.OutSensor, new List<DistanceMeasure>());
                    }
                }

                Console.WriteLine("It started measuiring...");

                SendCloudToDeviceMessageAsync("#START").Wait();

                Task.Delay(1000);


                var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

                CancellationTokenSource cts = new CancellationTokenSource();

                System.Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                    Console.WriteLine("Exiting...");
                };

                var tasks = new List<Task>();
                foreach (string partition in d2cPartitions)
                {
                    tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
                }
                Task.WaitAll(tasks.ToArray());

                //"sb://iothub-ns-swedaviala-105751-d1c5f83d17.servicebus.windows.net/";
                //@"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";

                /*

                client.OnMessage(message =>
                {
                    Stream stream = message.GetBody<Stream>();
                    StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                    string messageFormated = reader.ReadToEnd();
                    Console.WriteLine(String.Format("Message body: {0}", messageFormated));
                    //foreach (var deviceSensor in messageFormated.Split('#'))
                    //{
                    //    var deviceSensorInfo = deviceSensor.Split('|');
                    //    if (deviceSensorInfo.Count() == 2)
                    //    {
                    //        var sensorId = deviceSensorInfo[0];
                    //        var distanceValue = double.Parse(deviceSensorInfo[1]);
                    //        SensorDistanceMeasures[temporarySensor].Add(new DistanceMeasure(distanceValue, message.EnqueuedTimeUtc));
                    //    }
                    //}
                });
                */


            }
            catch (Exception ex)
            {
                var vad = ex.Message;
                throw;
            }
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            }
        }

        private static QueueClient queueClient;
        private static ServiceClient serviceClient;
        private static readonly string deviceId = @"swedaviaiotlabbId";
        static EventHubClient eventHubClient;
        //"WillesRaspbarryDevId";

        private static readonly string deviceConnectionString = @"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
        //private static readonly string deviceConnectionString = @"Endpoint=iothub-ns-swedaviala-105751-d1c5f83d17.servicebus.windows.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
        //sb://iothub-ns-swedaviala-105751-d1c5f83d17.servicebus.windows.net/
        //@"HostName=WillesIotHub1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=4YffuSTgyX5vHqeYaS+sy6uacKDxtuEktHh+rGZeEbM=";

        private static readonly string queueConnectionString = @"Endpoint=sb://iothub-ns-swedaviala-105751-d1c5f83d17.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
        //@"HostName=swedavialabbHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JM15d+vfOGvgcckCJ71zbKAnIwCN76CeHanG27a8dQ=";
        private static readonly string queueName = "DistanceMeasures";
        private static readonly string eventQueue = "messages/events";

        internal static void Initialize()
        {
            serviceClient = ServiceClient.CreateFromConnectionString(deviceConnectionString);
            eventHubClient = EventHubClient.CreateFromConnectionString(deviceConnectionString, eventQueue);

             //  var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            queueClient = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);
        }
        private static Sensor temporarySensor;

        private static Dictionary<Sensor, List<DistanceMeasure>> SensorDistanceMeasures = new Dictionary<Sensor, List<DistanceMeasure>>();

        internal static void AddEventOnMessage()
        {
            try
            {


                queueClient.OnMessage(message =>
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
    public class Place
    {
        /// <summary>
        /// Id 
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Places Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entrance"></param>
        public Place(string name, Entrance[] entrance)
        {
            Id = Guid.NewGuid();
            Name = name;
            Entrance[] Entrance = entrance;
        }

        public Place(Guid id)
        {
            Id = Guid.NewGuid();
            Name = id.ToString();
            Entrance = new Entrance[1] {
                new Entrance("Main door", new SensorBoard[2]
                {
                    new SensorBoard(new Sensor(Guid.NewGuid(),"AIn",13,13), new Sensor(Guid.NewGuid(),"AOut",13,13)),
                    new SensorBoard(new Sensor(Guid.NewGuid(),"BIn",13,13), new Sensor(Guid.NewGuid(),"BOut",13,13))
                })
            };
        }

        /// <summary>
        /// Entrances
        /// </summary>
        public Entrance[] Entrance { get; set; }
        /// <summary>
        /// Counting Total People Inside
        /// </summary>
        public int TotalPeopleInside { get; private set; }
        /// <summary>
        /// Counting Total People Pass IN
        /// </summary>
        public int TotalPeoplePassIn { get; private set; }
        /// <summary>
        /// Counting Total People Pass OUT
        /// </summary>
        public int TotalPeoplePassOut { get; private set; }
    }

    /// <summary>
    /// Entrance Model
    /// </summary>
    public class Entrance
    {
        public Guid Id { get; }
        public string Name { get; }

        public SensorBoard[] SensorBoards;
        /// <summary>
        /// Entrance Constructor
        /// </summary>
        public Entrance()
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
        }
        /// <summary>
        /// Entrance Constructor with parameters and name
        /// </summary> 
        public Entrance(string name, SensorBoard[] sensorBoards)
        {
            Id = Guid.NewGuid();
            Name = name;
            SensorBoards = sensorBoards;
        }

        /// <summary>
        /// Entrance Constructor with parameters
        /// </summary> 
        public Entrance(SensorBoard[] sensorBoards)
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
            SensorBoards = sensorBoards;
        }
    }

    /// <summary>
    /// SensorBoard Model
    /// </summary>
    public class SensorBoard
    {
        /// <summary>
        /// In Sensor
        /// </summary>
        public Sensor InSensor { get; set; }
        /// <summary>
        /// Out Sensor
        /// </summary>
        public Sensor OutSensor { get; set; }
        /// <summary>
        /// Sensor Constructor
        /// </summary>
        public SensorBoard() { }
        /// <summary>
        /// Sensor Constructor with parameters
        /// </summary>
        public SensorBoard(Sensor inSensor, Sensor outSensor)
        {
            InSensor = inSensor;
            OutSensor = outSensor;
        }

    }

    /// <summary>
    /// Sensor Model
    /// </summary>
    public class Sensor
    {
        Guid Id { get; }
        string Name { get; }
        /// <summary>
        /// Pin number for Trig from Sensor to Raspberry
        /// </summary>
        public short PinTrig { get; }
        /// <summary>
        /// Pin number for Echo from Sensor to Raspberry
        /// </summary>
        public short PinEcho { get; }
        /// <summary>
        /// Sensor Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="pinTrig"></param>
        /// <param name="pinEcho"></param>
        public Sensor(Guid id, string name, short pinTrig, short pinEcho)
        {
            Id = id;
            Name = name;
            PinTrig = pinTrig;
            PinEcho = pinEcho;
        }
    }
}
