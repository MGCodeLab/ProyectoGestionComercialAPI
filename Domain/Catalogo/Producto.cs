using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Catalogo
{
    public class Producto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
    }
}
