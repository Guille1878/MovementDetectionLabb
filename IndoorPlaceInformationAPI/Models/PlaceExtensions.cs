using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IndoorPlaceInformationAPI.Models
{
    public static class PlaceExtensions
    {
        public static SwedaviaLabEntities dbFunc = new SwedaviaLabEntities();
        internal static void PeoplePass(this Place place, Sensor sensor)
        {
            if (sensor.InsideOne)
                place.TotalPeopleInside++;
            else
            {
                if (--place.TotalPeopleInside < 0)
                    place.TotalPeopleInside = 0;
            }
            Task.Run(() => dbFunc.sp_SensorTransaction_Add(sensor.SensorId));
        }

        internal static void ResetCounter(this Place place)
        {
            place.TotalPeopleInside = 0;
            Task.Run(() => dbFunc.sp_Place_ResetCounter(place.PlaceId));

        }
    }   
}