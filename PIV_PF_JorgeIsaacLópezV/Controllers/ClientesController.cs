using PIV_PF_JorgeIsaacLópezV.Models;
using PIV_PF_JorgeIsaacLópezV.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIV_PF_JorgeIsaacLópezV.Filters;

namespace PIV_PF_JorgeIsaacLópezV.Controllers
{
    [VerificarAdmin]
    public class ClientesController : Controller
    {
        private LaFarmaciaEntities db = new LaFarmaciaEntities();
        private const int ID_TIPO_CLIENTE = 4;
        private const int ID_ESTADO_ACTIVO = 1;

        // GET: Clientes
        public ActionResult Index()
        {
            var clientes = ObtenerListaClientes();
            var modelo = new ListaClientes
            {
                Clientes = clientes
            };

            if (TempData["Mensaje"] != null)
            {
                modelo.Mensaje = TempData["Mensaje"].ToString();
                modelo.TipoMensaje = TempData["TipoMensaje"]?.ToString() ?? "info";
            }

            
            ViewBag.UsuarioLogueado = Session["NombreCompleto"];
            ViewBag.RolUsuario = Session["ROL"];

            return View(modelo);
        }

        public ActionResult CrearCliente()
        {
            var modelo = new Cliente();
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCliente(Cliente modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Usuarios.Any(u => u.Identificacion == modelo.Identificacion))
                    {
                        ModelState.AddModelError("Identificacion", "Ya existe un cliente con esta identificación");
                        return View(modelo);
                    }

                    var partesNombre = SepararNombreCompleto(modelo.NombreCompleto);

                    var cliente = new Usuarios
                    {
                        Identificacion = modelo.Identificacion,
                        Nombre = partesNombre.Nombre,
                        Apellidos = partesNombre.Apellidos,
                        Correo = modelo.Correo,
                        Id_TipoUsuario = ID_TIPO_CLIENTE,
                        Id_Estado = ID_ESTADO_ACTIVO
                    };

                    db.Usuarios.Add(cliente);
                    db.SaveChanges();

                    TempData["Mensaje"] = "Cliente registrado exitosamente";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar el cliente: " + ex.Message);
                }
            }

            return View(modelo);
        }

        public ActionResult EditarCliente(int id)
        {
            try
            {
                var cliente = db.Usuarios
                    .Where(u => u.Id_Usuario == id && u.Id_TipoUsuario == ID_TIPO_CLIENTE)
                    .FirstOrDefault();

                if (cliente == null)
                {
                    TempData["Mensaje"] = "Cliente no encontrado";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                var modelo = new Cliente
                {
                    Id_Usuario = cliente.Id_Usuario,
                    Identificacion = cliente.Identificacion,
                    NombreCompleto = $"{cliente.Nombre} {cliente.Apellidos}".Trim(),
                    Correo = cliente.Correo,
                    Id_TipoUsuario = cliente.Id_TipoUsuario,
                    Id_Estado = cliente.Id_Estado
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar el cliente: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCliente(Cliente modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cliente = db.Usuarios.Find(modelo.Id_Usuario);
                    if (cliente != null && cliente.Id_TipoUsuario == ID_TIPO_CLIENTE)
                    {
                        var partesNombre = SepararNombreCompleto(modelo.NombreCompleto);

                        cliente.Nombre = partesNombre.Nombre;
                        cliente.Apellidos = partesNombre.Apellidos;
                        cliente.Correo = modelo.Correo;

                        db.Entry(cliente).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Mensaje"] = "Cliente actualizado exitosamente";
                        TempData["TipoMensaje"] = "success";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Mensaje"] = "Cliente no encontrado";
                        TempData["TipoMensaje"] = "error";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el cliente: " + ex.Message);
                }
            }

            return View(modelo);
        }

        public ActionResult DetallesCliente(int id)
        {
            try
            {
                var cliente = db.Usuarios
                    .Include(u => u.Estados)
                    .Where(u => u.Id_Usuario == id && u.Id_TipoUsuario == ID_TIPO_CLIENTE)
                    .FirstOrDefault();

                if (cliente == null)
                {
                    TempData["Mensaje"] = "Cliente no encontrado";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                var modelo = new Cliente
                {
                    Id_Usuario = cliente.Id_Usuario,
                    Identificacion = cliente.Identificacion,
                    NombreCompleto = $"{cliente.Nombre} {cliente.Apellidos}".Trim(),
                    Correo = cliente.Correo,
                    EstadoDescripcion = cliente.Estados?.Descripcion
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar los detalles del cliente: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        #region Métodos Privados

        private List<Cliente> ObtenerListaClientes()
        {
            return db.Usuarios
                .Include(u => u.Estados)
                .Where(u => u.Id_TipoUsuario == ID_TIPO_CLIENTE)
                .Select(u => new Cliente
                {
                    Id_Usuario = u.Id_Usuario,
                    Identificacion = u.Identificacion,
                    NombreCompleto = (u.Nombre + " " + u.Apellidos).Trim(),
                    Correo = u.Correo,
                    EstadoDescripcion = u.Estados.Descripcion,
                    Id_Estado = u.Id_Estado
                })
                .OrderBy(c => c.NombreCompleto)
                .ToList();
        }

        private (string Nombre, string Apellidos) SepararNombreCompleto(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return ("", "");

            var partes = nombreCompleto.Trim().Split(' ');

            if (partes.Length == 1)
            {
                return (partes[0], "");
            }
            else if (partes.Length == 2)
            {
                return (partes[0], partes[1]);
            }
            else
            {
                var nombre = partes[0];
                var apellidos = string.Join(" ", partes.Skip(1));
                return (nombre, apellidos);
            }
        }

        #endregion

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