using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PIV_PF_JorgeIsaacLópezV.Filters
{
    public class VerificarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            if (filterContext.HttpContext.Session["Id_Usuario"] == null)
            {
                
                filterContext.Result = new RedirectResult("~/Login/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class VerificarAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            if (filterContext.HttpContext.Session["Id_Usuario"] == null)
            {
                filterContext.Result = new RedirectResult("~/Login/Login");
                return;
            }

            
            var rol = filterContext.HttpContext.Session["ROL"]?.ToString();
            if (rol != "Administrador")
            {
                
                filterContext.Result = new ContentResult()
                {
                    Content = "<html><body style='font-family:Arial;text-align:center;padding:50px;'>" +
                             "<h2>Acceso Denegado</h2>" +
                             "<p>No tiene permisos para acceder a esta sección.</p>" +
                             "<a href='javascript:history.back()'>Regresar</a>" +
                             "</body></html>",
                    ContentType = "text/html"
                };
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}