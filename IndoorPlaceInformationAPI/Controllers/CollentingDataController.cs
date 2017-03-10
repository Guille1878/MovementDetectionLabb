using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

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
        [SwaggerOperation("Get")]
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
                {
                    db.SensorBoard.FirstOrDefault(sb => sensor.SensorId == (sensor.InsideOne ? sb.InSensorId : sb.OutSensorId)).Entrance.Place.PeoplePass(sensor.InsideOne);
                    Task.Run(() => db.sp_SensorTransaction_Add(sensor.SensorId));
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
