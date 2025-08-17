using PIV_PF_JorgeIsaacLópezV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PIV_PF_JorgeIsaacLópezV.Models.ViewModels;

namespace PIV_PF_JorgeIsaacLópezV.Controllers
{
    public class LoginController : Controller
    {
        private LaFarmaciaEntities db = new LaFarmaciaEntities();

        // GET: Login
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpGet]
        public ActionResult Ingresar()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Ingresar(Models.ViewModels.Login login)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }

            var existenciaUsuario = db.Usuarios
                .Where(p => p.Correo == login.CorreoUsuario && p.Identificacion == login.IdentificacionUsuario)
                .FirstOrDefault();

            if (existenciaUsuario != null)
            {
                
                if (existenciaUsuario.Id_Estado != 1) 
                {
                    ViewBag.ValorMensaje = 0;
                    ViewBag.MensajeProceso = "Usuario inactivo";
                    return View("Login");
                }

                
                var tipoUsuario = db.TiposUsuario.Find(existenciaUsuario.Id_TipoUsuario);
                string rolUsuario = tipoUsuario?.Descripcion?.ToUpper() ?? "";

                if (rolUsuario == "ADMINISTRADOR")
                {
                    Session["ROL"] = existenciaUsuario.TiposUsuario.Descripcion;
                    Session["Id_Usuario"] = existenciaUsuario.Id_Usuario;
                    Session["NombreCompleto"] = existenciaUsuario.Nombre + " " + existenciaUsuario.Apellidos;
                    Session.Timeout = 45;
                    return RedirectToAction("Index", "Home");
                }
                else if (rolUsuario == "VENDEDOR")
                {
                    Session["ROL"] = existenciaUsuario.TiposUsuario.Descripcion;
                    Session["Id_Usuario"] = existenciaUsuario.Id_Usuario;
                    Session["NombreCompleto"] = existenciaUsuario.Nombre + " " + existenciaUsuario.Apellidos;
                    Session.Timeout = 45;
                    return RedirectToAction("Index", "Home");
                }
                else if (rolUsuario == "CONTABILIDAD")
                {
                    Session["ROL"] = existenciaUsuario.TiposUsuario.Descripcion;
                    Session["Id_Usuario"] = existenciaUsuario.Id_Usuario;
                    Session["NombreCompleto"] = existenciaUsuario.Nombre + " " + existenciaUsuario.Apellidos;
                    Session.Timeout = 45;
                    return RedirectToAction("Index", "Home"); 
                }
                else if (rolUsuario == "CLIENTE")
                {
                    Session["ROL"] = existenciaUsuario.TiposUsuario.Descripcion;
                    Session["Id_Usuario"] = existenciaUsuario.Id_Usuario;
                    Session["NombreCompleto"] = existenciaUsuario.Nombre + " " + existenciaUsuario.Apellidos;
                    Session.Timeout = 45;
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    ViewBag.ValorMensaje = 0;
                    ViewBag.MensajeProceso = "Usuario no tiene permisos asignados";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Usuario no existe";
                return View("Login");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
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