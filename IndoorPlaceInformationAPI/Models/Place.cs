using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndoorPlaceInformationAPI.Models
{
    public class Place
    {
        Guid Id { get; }
        string Name { get; }
        public Place(string name, Entrance[] entrance)
        {
            Id = Guid.NewGuid();
            Name = name;
            Entrance[] Entrance = entrance;
        }
        public Entrance[] Entrance { get; set; }
        public int TotalPeopleInside { get; private set; }
        public int TotalPeoplePassIn { get; private set; }
        public int TotalPeoplePassOut { get; private set; }
    }

    public class Entrance
    {
        Guid Id { get; }
        string Name { get; }
        SensorBoard[] SensorBoards;
        public Entrance()
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
        }
        public Entrance(string name, SensorBoard[] sensorBoards)
        {
            Id = Guid.NewGuid();
            Name = name;
            SensorBoards = sensorBoards;
        }
        public Entrance(SensorBoard[] sensorBoards)
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
            SensorBoards = sensorBoards;
        }  
    }

    public class SensorBoard
    {
        public Sensor InSensor { get; set; }
        public Sensor OutSensor { get; set; }
        public SensorBoard() { }
        public SensorBoard(Sensor inSensor, Sensor outSensor)
        {
            InSensor = inSensor;
            OutSensor = outSensor;
        }

    }
    public class Sensor
    {
        Guid Id { get; }
        string Name { get; }
        public short PinTrig { get; }
        public short PinEcho { get; }

        public Sensor(Guid id, string name, short pinTrig, short pinEcho)
        {
            Id = id;
            Name = name;
            PinTrig = pinTrig;
            PinEcho = pinEcho;
        }
    }
}
