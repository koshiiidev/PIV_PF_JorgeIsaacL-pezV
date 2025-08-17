using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIV_PF_JorgeIsaacLópezV.Models.ViewModels
{
    public class Cliente
    {
        public int Id_Usuario { get; set; }
        
        [Required(ErrorMessage = "La identificación es obligatoria")]
        [Display(Name = "Identificación")]
        public int Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no debe contener números")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        public string Correo { get; set; }


        public string EstadoDescripcion { get; set; }


        public int Id_TipoUsuario { get; set; }
        public int Id_Estado { get; set; }
    }

    public class ListaClientes
    {
        public List<Cliente> Clientes { get; set; }
        public string Mensaje { get; set; }
        public string TipoMensaje { get; set; }
    }
}