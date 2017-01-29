using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace IndoorPlaceInformationAPI.Models
{
    /// <summary>
    /// Places Model
    /// </summary>
    [DataContract]
    public class Place
    {
        /// <summary>
        /// Id 
        /// </summary>
        [DataMember]

        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Places Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entrance"></param>
        public Place(string name, Entrance[] entrance)
        {
            Id = Guid.NewGuid();
            Name = name;
            Entrance = entrance;
        }

        /// <summary>
        /// Places Constructor with Id
        /// </summary>
        /// <param name="id">
        /// Id in Guid format identifying the place.
        /// </param>
        public Place(Guid id)
        {
            Id = id;
            Name = "Sky City Big WC";
            Entrance = new Entrance[1] {
                new Entrance("Main door", new SensorBoard[2]
                {
                    new SensorBoard(new Sensor(Guid.NewGuid(),"AIn",13,13), new Sensor(Guid.NewGuid(),"AOut",13,13)),
                    new SensorBoard(new Sensor(Guid.NewGuid(),"BIn",13,13), new Sensor(Guid.NewGuid(),"BOut",13,13))
                })
            };
            Capacity = 150;
            TotalPeopleInside = 132;
            TotalPeoplePassIn = 35321312;
            TotalPeoplePassOut = 35321180;
            LastService = DateTime.Now.AddMinutes(-14);
            Type = PlaceType.ToiletMix;

        }

        /// <summary>
        /// Entrances
        /// </summary>
        [DataMember]
        public Entrance[] Entrance { get; set; }
        /// <summary>
        /// Counting Total People Inside
        /// </summary>
        [DataMember]
        public int TotalPeopleInside { get; set; }
        /// <summary>
        /// Counting Total People Pass IN
        /// </summary>
        [DataMember]
        public int TotalPeoplePassIn { get; set; }
        /// <summary>
        /// Counting Total People Pass OUT
        /// </summary>
        [DataMember]
        public int TotalPeoplePassOut { get; set; }

        /// <summary>
        /// Estimated how many people get place in the place.
        /// </summary>
        [DataMember]
        public int Capacity { get; set; }

        /// <summary>
        /// Last time the place got a Service.
        /// </summary>
        [DataMember]
        public DateTime LastService { get; set; }

        /// <summary>
        /// Type of place
        /// </summary>
        [DataMember]
        public PlaceType Type { get; set; }

    }

    /// <summary>
    /// Entrance Model
    /// </summary>
    [DataContract]
    public class Entrance
    {
        /// <summary>
        /// Entrance Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// Entrance name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Bord with sensor and one raspberry
        /// </summary>
        [DataMember]
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
    [DataContract]
    public class SensorBoard
    {
        /// <summary>
        /// In Sensor
        /// </summary>
        [DataMember]
        public Sensor InSensor { get; set; }
        /// <summary>
        /// Out Sensor
        /// </summary>
        [DataMember]
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
    [DataContract]
    public class Sensor
    {
        /// <summary>
        /// Sensor Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// Sensor name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Pin number for Trig from Sensor to Raspberry
        /// </summary>
        [DataMember]
        public short PinTrig { get; set; }
        /// <summary>
        /// Pin number for Echo from Sensor to Raspberry
        /// </summary>
        [DataMember]
        public short PinEcho { get; set; }
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
