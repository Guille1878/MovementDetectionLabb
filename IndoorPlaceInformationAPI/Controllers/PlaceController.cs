using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using IndoorPlaceInformationAPI.Models;

namespace IndoorPlaceInformationAPI.Controllers
{
    public class PlacesController : ApiController
    {
        // GET api/places
        [SwaggerOperation("GetAll")]
        public IEnumerable<Place> Get()
        {
            
            try
            {
                return new Place[1]
                {
                    new Place("Test", new Entrance[0])
                };
            }
            catch 
            {
                return new Place[0];
            }
        }

        // GET api/places?StartCounting
        [SwaggerOperation("StartCounting")]
        public string StartCounting()
        {
            try
            {
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        // GET api/places?StopCounting
        [SwaggerOperation("StopCounting")]
        public string StopCounting()
        {
            try
            {
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // GET api/places/5
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

        // POST api/places
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/places/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/places/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
