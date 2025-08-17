using PIV_PF_JorgeIsaacLópezV.Models;
using PIV_PF_JorgeIsaacLópezV.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PIV_PF_JorgeIsaacLópezV.Filters;

namespace PIV_PF_JorgeIsaacLópezV.Controllers
{
    [VerificarAdmin]
    public class UsuariosController : Controller
    {

        private LaFarmaciaEntities db = new LaFarmaciaEntities();

        // GET: Usuarios
        public ActionResult Index(int? filtroRol = null)
        {
            var usuarios = ObtenerListaUsuariosAdministrativos(filtroRol);
            var modelo = new ListaUsuarios
            {
                Usuarios = usuarios,
                FiltroRolSeleccionado = filtroRol
            };

            if (TempData["Mensaje"] != null)
            {
                modelo.Mensaje = TempData["Mensaje"].ToString();
                modelo.TipoMensaje = TempData["TipoMensaje"]?.ToString() ?? "info";
            }

            
            CargarRolesAdministrativos(modelo);

            ViewBag.UsuarioLogueado = Session["NombreCompleto"];
            ViewBag.RolUsuario = Session["ROL"];

            return View(modelo);
        }

        public ActionResult CrearUsuario()
        {
            var modelo = new Usuario();
            CargarListasDesplegables(modelo);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario(Usuario modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Usuarios.Any(u => u.Identificacion == modelo.Identificacion))
                    {
                        ModelState.AddModelError("Identificacion", "Ya existe un usuario con esta identificacion");
                        CargarListasDesplegables(modelo);
                        return View(modelo);
                    }

                    var usuario = new Usuarios
                    {
                        Identificacion = modelo.Identificacion,
                        Nombre = modelo.Nombre,
                        Apellidos = modelo.Apellidos,
                        Correo = modelo.Correo,
                        Id_TipoUsuario = modelo.Id_TipoUsuario,
                        Id_Estado = modelo.Id_Estado
                    };

                    db.Usuarios.Add(usuario);
                    db.SaveChanges();

                    TempData["Mensaje"] = "Usuario creado exitosamente";
                    TempData["TipoMensaje"] = "Success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el usuario" + ex.Message);
                }
            }

            CargarListasDesplegables(modelo);
            return View(modelo);
        }

        public ActionResult EditarUsuario(int id)
        {
            try
            {
                var usuario = db.Usuarios.Find(id);
                if (usuario == null)
                {
                    TempData["Mensaje"] = "Usuario no encontrado";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                var modelo = new Usuario
                {
                    Id_Usuario = usuario.Id_Usuario,
                    Identificacion = usuario.Identificacion,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Correo = usuario.Correo,
                    Id_TipoUsuario = usuario.Id_TipoUsuario,
                    Id_Estado = usuario.Id_Estado
                };

                CargarListasDesplegables(modelo);
                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar el usuario: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario(Usuario modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Usuarios.Any(u => u.Identificacion == modelo.Identificacion && u.Id_Usuario != modelo.Id_Usuario))
                    {
                        ModelState.AddModelError("Identificacion", "Ya existe otro usuario con esta identificiacion");
                        CargarListasDesplegables(modelo);
                        return View(modelo);
                    }

                    var usuario = db.Usuarios.Find(modelo.Id_Usuario);
                    if (usuario != null)
                    {
                        usuario.Nombre = modelo.Nombre;
                        usuario.Apellidos = modelo.Apellidos;
                        usuario.Correo = modelo.Correo;
                        usuario.Id_TipoUsuario = modelo.Id_TipoUsuario;
                        usuario.Id_Estado = modelo.Id_Estado;

                        db.Entry(usuario).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Mensaje"] = "Usuario actualizado exitosamente";
                        TempData["TipoMensaje"] = "success";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Mensaje"] = "Usuario no encontrado";
                        TempData["TipoMensaje"] = "error";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el usuario: " + ex.Message);
                }
            }
            CargarListasDesplegables(modelo);
            return View(modelo);
        }

        public ActionResult DetallesUsuario(int id)
        {
            try
            {
                var usuario = db.Usuarios
                    .Include(u => u.TiposUsuario)
                    .Include(u => u.Estados)
                    .FirstOrDefault(u => u.Id_Usuario == id);

                if (usuario == null)
                {
                    TempData["Mensaje"] = "Usuario no encontrado";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                var modelo = new Usuario
                {
                    Id_Usuario = usuario.Id_Usuario,
                    Identificacion = usuario.Identificacion,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Correo = usuario.Correo,
                    Id_TipoUsuario = usuario.Id_TipoUsuario,
                    Id_Estado = usuario.Id_Estado,
                    TipoUsuarioDescripcion = usuario.TiposUsuario?.Descripcion,
                    EstadoDescripcion = usuario.Estados?.Descripcion
                };

                return View(modelo);

            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar los detalles del usuario: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        #region Métodos Privados

        
        private List<Usuario> ObtenerListaUsuariosAdministrativos(int? filtroRol = null)
        {
            var query = db.Usuarios
                .Include(u => u.TiposUsuario)
                .Include(u => u.Estados)
                .Where(u => u.Id_TipoUsuario >= 1 && u.Id_TipoUsuario <= 3); 

            
            if (filtroRol.HasValue && filtroRol.Value > 0)
            {
                query = query.Where(u => u.Id_TipoUsuario == filtroRol.Value);
            }

            return query.Select(u => new Usuario
            {
                Id_Usuario = u.Id_Usuario,
                Identificacion = u.Identificacion,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Correo = u.Correo,
                Id_TipoUsuario = u.Id_TipoUsuario,
                Id_Estado = u.Id_Estado,
                TipoUsuarioDescripcion = u.TiposUsuario.Descripcion,
                EstadoDescripcion = u.Estados.Descripcion
            })
                .OrderBy(u => u.Nombre)
                .ToList();
        }

        
        private void CargarRolesAdministrativos(ListaUsuarios modelo)
        {
            var rolesAdministrativos = db.TiposUsuario
                .Where(t => t.Id_TipoUsuario >= 1 && t.Id_TipoUsuario <= 3) // Solo roles administrativos
                .OrderBy(t => t.Descripcion)
                .Select(t => new SelectListItem
                {
                    Value = t.Id_TipoUsuario.ToString(),
                    Text = t.Descripcion,
                    Selected = modelo.FiltroRolSeleccionado == t.Id_TipoUsuario
                })
                .ToList();

            
            rolesAdministrativos.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "Todos los roles",
                Selected = !modelo.FiltroRolSeleccionado.HasValue || modelo.FiltroRolSeleccionado.Value == 0
            });

            modelo.RolesAdministrativos = rolesAdministrativos;
        }

        private void CargarListasDesplegables(Usuario modelo)
        {
            modelo.TiposUsuario = new SelectList(
                db.TiposUsuario.OrderBy(t => t.Descripcion),
                "Id_TipoUsuario",
                "Descripcion",
                modelo.Id_TipoUsuario
                );

            modelo.Estados = new SelectList(
                db.Estados.OrderBy(e => e.Descripcion),
                "ID_Estado",
                "Descripcion",
                modelo.Id_Estado
                );
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