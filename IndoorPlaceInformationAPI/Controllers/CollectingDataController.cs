using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndoorPlaceInformationAPI.Controllers
{
    public class CollectingDataController : Controller
    {
        // GET: CollectingData
        public ActionResult Index()
        {
            return View();
        }

        // GET: CollectingData/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CollectingData/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CollectingData/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CollectingData/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CollectingData/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CollectingData/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CollectingData/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
