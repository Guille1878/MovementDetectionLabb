using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using IndoorPlaceInformationAPI.Models;

namespace IndoorPlaceInformationAPI.Controllers
{
    /// <summary>
    /// API for collecting data from IOT devices
    /// </summary>
    public class CollentingDataController : ApiController
    {
        private SwedaviaLabEntities db = new SwedaviaLabEntities();

        /// <summary>
        /// This methods is to be call from a device with sensor telling if a person pass in or put.
        /// </summary>
        /// <param name="sensorId">The iditification for the sensor (Guid format)</param>
        /// <returns>1 or 0 (1 when it works or 0 when it did not work)</returns>
        [SwaggerOperation("CountingSensor")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        public int CountingSensor(string sensorId)
        {
            try
            {
                var sensorIdGuid = Guid.Parse(sensorId);
                var sensor = db.Sensor.FirstOrDefault(s => s.SensorId == sensorIdGuid);
                if (sensor == null)
                    return 0;
                else
                    db.SensorBoard.FirstOrDefault(sb => sensor.SensorId == (sensor.InsideOne ? sb.InSensorId : sb.OutSensorId)).Entrance.Place.PeoplePass(sensor);

            }
            catch 
            {
                return 0;
            }
            
            
            return 1;
        }

        /// <summary>
        /// ResetCounter put in zero the total of people inside in the place.
        /// </summary>
        /// <param name="placeId">The Id for the place you want to reset the counter</param>
        /// <returns>1 or 0 (1 when it works or 0 when it did not work)</returns>
        [SwaggerOperation("ResetCounter")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        public int ResetCounter(string placeId)
        {
            try
            {
                var placeIdGuid = Guid.Parse(placeId);
                var place = db.Place.FirstOrDefault(s => s.PlaceId == placeIdGuid);
                if (place == null)
                    return 0;
                else
                {
                    place.ResetCounter();
                }
            }
            catch
            {
                return 0;
            }


            return 1;
        }
    }
}
