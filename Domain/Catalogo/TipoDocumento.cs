using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Catalogo
{
    public class TipoDocumento
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
