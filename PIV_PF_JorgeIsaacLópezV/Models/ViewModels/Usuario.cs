using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;

namespace PIV_PF_JorgeIsaacLópezV.Models.ViewModels
{
    public class Usuario
    {
        public int Id_Usuario { get; set; }

        [Required(ErrorMessage = "La identificacion es Obligatoria")]
        [Display (Name = "Identificacion")]
        public int Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display (Name = "Nombre")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no debe contener números")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [Display(Name = "Apellidos")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Los apellidos no deben contener números")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es obligatorio")]
        [Display(Name = "Tipo de Usuario")]
        public int Id_TipoUsuario { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public int Id_Estado { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto 
        {
            get { return $"{Nombre} {Apellidos}"; }
        }

        public string TipoUsuarioDescripcion { get; set; }
        public string EstadoDescripcion { get; set; }


        public SelectList TiposUsuario { get; set; }
        public SelectList Estados { get; set; }


    }

    public class ListaUsuarios 
    {
        public List<Usuario> Usuarios { get; set; }
        public string Mensaje { get; set; }
        public string TipoMensaje { get; set; }
    }
}