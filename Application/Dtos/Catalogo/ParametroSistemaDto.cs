namespace Application.Dtos.Catalogo;

public class ParametroSistemaDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string TipoDato { get; set; } = "STRING";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
