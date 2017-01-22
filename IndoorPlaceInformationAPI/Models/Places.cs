using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndoorPlaceInformationAPI.Models
{
    /// <summary>
    /// Places Model
    /// </summary>
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
