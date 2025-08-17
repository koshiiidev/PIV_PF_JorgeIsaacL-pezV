using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIV_PF_JorgeIsaacLópezV.Models.ViewModels
{
    public class Login
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string CorreoUsuario { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [Display(Name = "Identificación")]
        public int IdentificacionUsuario { get; set; }
    }
}