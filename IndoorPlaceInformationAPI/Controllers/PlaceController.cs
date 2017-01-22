using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using IndoorPlaceInformationAPI.Models;
using System.Threading.Tasks;

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
        public IEnumerable<Place> Get()
        {

            try
            {
                return new Place[1]
                {
                    new Place("test", new Entrance[0])
                };
            }
            catch
            {
                return new Place[0];
            }
        }
           

        // GET api/places/5
        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public Place Get(int id)
        {
            try
            {
                return new Place("Test", new Entrance[0]);
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
        /// <param name="placeId"></param>
        /// <returns></returns>
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
        /// <param name="placeId"></param>
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
