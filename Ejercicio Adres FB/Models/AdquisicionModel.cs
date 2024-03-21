using System.ComponentModel.DataAnnotations;

namespace Ejercicio_Adres_FB.Models
{
    public class AdquisicionModel
    { // Identificador único para cada adquisición.
        [Key]
        public Guid Id { get; set; }

        // Presupuesto asignado para la adquisición. Debe ser un número positivo.
        [Required(ErrorMessage = "El presupuesto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El presupuesto debe ser un número positivo.")]
        public double Presupuesto { get; set; }

        // Unidad administrativa responsable de la adquisición.
        [Required(ErrorMessage = "La unidad administrativa es obligatoria.")]
        public string UnidadAdministrativa { get; set; }

        // Descripción del tipo de bien o servicio adquirido.
        [Required(ErrorMessage = "El tipo de bien o servicio es obligatorio.")]
        public string TipoBienServicio { get; set; }

        // Cantidad de unidades adquiridas.
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        // Costo por unidad del bien o servicio.
        [Required(ErrorMessage = "El valor unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor unitario debe ser mayor que 0.")]
        public double ValorUnitario { get; set; }

        // Costo total de la adquisición.
        public double ValorTotal => Cantidad * ValorUnitario;

        // Fecha en que se realizó la adquisición.
        [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
        public DateTime FechaAdquisicion { get; set; }

        // Entidad proveedora del bien o servicio.
        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        public string Proveedor { get; set; }

        // Detalles de la documentación asociada, como órdenes de compra, facturas, etc.
        public string Documentacion { get; set; }

        public bool Eliminado { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
    }
}
