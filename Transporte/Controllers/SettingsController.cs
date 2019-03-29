using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Transporte.Helpers;
using Transporte.Models;
using Transporte.Tags;
using Transporte.ViewModel;

namespace Transporte.Controllers
{
    [AutenticadoAttribute]
    public class SettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Configuración";
        public string WindowDescription = "Sistema";

        // GET: Settings/Edit
        public ActionResult Edit()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            int intDiasAntesDeVencer = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;

            SettingsViewModel settingsViewModel = new SettingsViewModel
            {
                DiasAntesDeVencer = intDiasAntesDeVencer
            };

            return View(settingsViewModel);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DiasAntesDeVencer")] SettingsViewModel setting)
        {
            if (ModelState.IsValid)
            {

                Setting settingDiasAntesDeVencer = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").FirstOrDefault();

                if (settingDiasAntesDeVencer != null)
                {
                    settingDiasAntesDeVencer.Numero1 = setting.DiasAntesDeVencer;
                    db.Entry(settingDiasAntesDeVencer).State = EntityState.Modified;
                    db.SaveChanges();
                }

           


                return RedirectToAction("Edit");
            }
            return View(setting);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
