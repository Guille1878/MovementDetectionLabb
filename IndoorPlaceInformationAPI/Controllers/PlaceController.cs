using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using IndoorPlaceInformationAPI.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IndoorPlaceInformationAPI.Controllers
{
    /// <summary>
    /// Places Controller
    /// </summary>
    public class PlacesController : ApiController
    {
        // get api/places
        /// <summary>
        /// Get All Places in the database
        /// </summary>
        /// <returns>A list of Places</returns>
        [SwaggerOperation("GetAll")]
        [EnumDataType(typeof(PlaceType))]
        public IEnumerable<Place> Get()
        {

            try
            {
                return new Place[6]
                {
                    new Place("Sky City Big WC Men", new Entrance[1]
                    {
                        new Entrance("Main Door", 
                              new SensorBoard[2]
                              {
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  },
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  }
                              }
                        )
                    })
                    {
                        Capacity = 60,
                        TotalPeopleInside = 34,
                        TotalPeoplePassIn = 5651,
                        TotalPeoplePassOut = 5617,
                        LastService = DateTime.Now.AddMinutes(-32),
                        Type = PlaceType.ToiletGentlemen
                    },
                    new Place("Sky City Big WC Women", new Entrance[1]
                    {
                        new Entrance("Main Door",
                              new SensorBoard[2]
                              {
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  },
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  }
                              }
                        )
                    })
                    {
                        Capacity = 60,
                        TotalPeopleInside = 8,
                        TotalPeoplePassIn = 81250,
                        TotalPeoplePassOut = 81242,
                        LastService = DateTime.Now.AddMinutes(-5),
                        Type = PlaceType.ToiletLadies
                    },
                    new Place("Terminal 2 WC", new Entrance[1]
                    {
                        new Entrance("Main Door",
                              new SensorBoard[1]
                              {
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  }
                              }
                        )
                    })
                    {
                        Capacity = 4,
                        TotalPeopleInside = 1,
                        TotalPeoplePassIn = 668,
                        TotalPeoplePassOut = 667,
                        LastService = DateTime.Now.AddMinutes(-197),
                        Type = PlaceType.ToiletMix
                    },
                    new Place("Terminal 3 WC", new Entrance[1]
                    {
                        new Entrance("Main Door",
                              new SensorBoard[1]
                              {
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  }
                              }
                        )
                    })
                    {
                        Capacity = 4,
                        TotalPeopleInside = 4,
                        TotalPeoplePassIn = 68,
                        TotalPeoplePassOut = 64,
                        LastService = DateTime.Now.AddMinutes(-289),
                        Type = PlaceType.ToiletMix
                    },
                    new Place("Terminal 4 WC", new Entrance[1]
                    {
                        new Entrance("Main Door",
                              new SensorBoard[1]
                              {
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  }
                              }
                        )
                    })
                    {
                        Capacity = 2,
                        TotalPeopleInside = 3,
                        TotalPeoplePassIn = 9668,
                        TotalPeoplePassOut = 9665,
                        LastService = DateTime.Now.AddMinutes(-19),
                        Type = PlaceType.ToiletMix
                    },
                    new Place("Terminal 4 WC Handicap", new Entrance[1]
                    {
                        new Entrance("Main Door",
                              new SensorBoard[1]
                              {
                                  new SensorBoard
                                  {
                                       InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                       OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                  }
                              }
                        )
                    })
                    {
                        Capacity = 1,
                        TotalPeopleInside = 1,
                        TotalPeoplePassIn = 35,
                        TotalPeoplePassOut = 34,
                        LastService = DateTime.Now.AddMinutes(-574),
                        Type = PlaceType.ToiletAccessible
                    }
                };
            }
            catch
            {
                return new Place[0];
            }
        }
        /// <summary>
        /// Get a Sensor
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation("GetAll")]
        public Sensor GetSensor()
        {
            return new Sensor(Guid.NewGuid(), "",12,32);
        }

        // GET api/places/5
        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id">
        /// Id in Guid format identifying the place.
        /// </param>
        /// <returns>
        /// One place from the Id you gave.
        /// </returns>
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [EnumDataType(typeof(PlaceType))]
        public Place Get(string id)
        {
            try
            {
                return new Place(id, new Entrance[1]
                {
                    new Entrance
                    {
                            SensorBoards = new SensorBoard[1]
                            {
                                new SensorBoard
                                {
                                    InSensor = new Sensor(Guid.NewGuid(),"Inside sensor", 12,23),
                                    OutSensor = new Sensor(Guid.NewGuid(),"Outside sensor", 27,26),
                                }
                            }
                    }
                })
                {
                    Capacity = 150,
                    TotalPeopleInside = 132,
                    TotalPeoplePassIn = 35321312,
                    TotalPeoplePassOut = 35321180,
                    LastService = DateTime.Now.AddMinutes(-14),
                    Type = PlaceType.ToiletMix
                };
            }
            catch
            {
                return new Place("", new Entrance[0]);
            }
        }

        // GET api/places/5
        /// <summary>
        /// StartDetecting
        /// </summary>
        /// <param name="placeId">
        /// PlaceId in Guid format identifying the place.
        /// </param>
        /// <returns>
        /// It activate the device/s of the place refering to the Id in the parameter. The device/s is/are going to return measures counting people coming in and out from the place.
        /// </returns>
        [SwaggerOperation("Get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        public async Task<string> StartDetecting(string placeId)
        {
            try
            {
                var placeIdGuid = Guid.Parse(placeId);
                await CollectingDistanceData.StartDetectingPlaceAsync(placeIdGuid);
                
                return "OK";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// StopDetecting
        /// </summary>
        /// <param name="placeId">
        /// PlaceId in Guid format identifying the place.
        /// </param>
        /// <returns></returns>
        [SwaggerOperation("Get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        public string StopDetecting(string placeId)
        {
            try
            {
                var placeIdGuid = Guid.Parse(placeId);
                CollectingDistanceData.StopDetectingPlaceAsync(placeIdGuid);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // POST api/places
        /// <summary>
        /// Post places
        /// </summary>
        /// <param name="value"></param>
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/places/5
        /// <summary>
        /// Update places
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/places/5
        /// <summary>
        /// DELETE places
        /// </summary>
        /// <param name="id"></param>
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
