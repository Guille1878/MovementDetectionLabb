using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using IndoorPlaceInformationAPI;
using Swashbuckle.Swagger.Annotations;

namespace IndoorPlaceInformationAPI.Controllers
{
    public class PlacesController : ApiController
    {
        private SwedaviaLabEntities db = new SwedaviaLabEntities();

        // GET: api/Places
        [SwaggerOperation("GetAllPlaces")]
        public IQueryable<Place> GetPlace()
        {
            var vad =  db.Place.ToList();
            return db.Place;
        }

        // GET: api/Places/5
        [SwaggerOperation("GetPlace")]
        [ResponseType(typeof(Place))]
        public IHttpActionResult GetPlace(Guid id)
        {
            Place place = db.Place.Find(id);
            if (place == null)
            {
                return NotFound();
            }

            return Ok(place);
        }

        /// <summary>
        /// Get how many people is inside a place
        /// </summary>
        /// <param name="id">Identity for a Place</param>
        /// <returns>Returns only the number of people is inside the place</returns>
        [SwaggerOperation("GetTotalPeopleInsideByPlaceId")]
        [ResponseType(typeof(long))]
        public IHttpActionResult GetTotalPeopleInsideByPlaceId(Guid id)
        {
            Place place = db.Place.Find(id);
            if (place == null)
            {
                return NotFound();
            }
            return Ok(place.TotalPeopleInside);
        }

        // PUT: api/Places/5
        [SwaggerOperation("PutPlace")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPlace(Guid id, Place place)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != place.PlaceId)
            {
                return BadRequest();
            }

            db.Entry(place).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Places
        [SwaggerOperation("PostPlace")]
        [ResponseType(typeof(Place))]
        public IHttpActionResult PostPlace(Place place)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Place.Add(place);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PlaceExists(place.PlaceId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = place.PlaceId }, place);
        }

        // DELETE: api/Places/5
        [SwaggerOperation("DeletePlace")]
        [ResponseType(typeof(Place))]
        public IHttpActionResult DeletePlace(Guid id)
        {
            Place place = db.Place.Find(id);
            if (place == null)
            {
                return NotFound();
            }

            db.Place.Remove(place);
            db.SaveChanges();

            return Ok(place);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlaceExists(Guid id)
        {
            return db.Place.Count(e => e.PlaceId == id) > 0;
        }
    }
}