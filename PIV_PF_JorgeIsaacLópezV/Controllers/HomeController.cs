using PIV_PF_JorgeIsaacLópezV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIV_PF_JorgeIsaacLópezV.Models.ViewModels;

namespace PIV_PF_JorgeIsaacLópezV.Controllers
{
    public class HomeController : Controller
    {
        private LaFarmaciaEntities db = new LaFarmaciaEntities();
        public ActionResult Index()
        {
            try
            {
                // Obtener estadísticas del sistema
                var totalUsuarios = db.Usuarios.Count();
                var totalClientes = db.Usuarios.Where(u => u.Id_TipoUsuario == 4).Count(); // ID 4 = Cliente
                var totalAdmins = db.Usuarios.Where(u => u.Id_TipoUsuario != 4).Count(); // Diferente de cliente
                var usuariosActivos = db.Usuarios.Where(u => u.Id_Estado == 1).Count(); // ID 1 = Activo

                // Pasar datos a la vista
                ViewBag.TotalUsuarios = totalUsuarios;
                ViewBag.TotalClientes = totalClientes;
                ViewBag.TotalAdmins = totalAdmins;
                ViewBag.UsuariosActivos = usuariosActivos;

                // Información del usuario logueado (si existe en sesión)
                ViewBag.UsuarioLogueado = Session["NombreCompleto"];
                ViewBag.RolUsuario = Session["ROL"];

                return View();
            }
            catch (Exception ex)
            {
                // En caso de error, mostrar valores por defecto
                ViewBag.TotalUsuarios = 0;
                ViewBag.TotalClientes = 0;
                ViewBag.TotalAdmins = 0;
                ViewBag.UsuariosActivos = 0;
                ViewBag.Error = "Error al cargar las estadísticas: " + ex.Message;

                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Sistema de Gestión Farmacéutica";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Información de contacto";
            return View();
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