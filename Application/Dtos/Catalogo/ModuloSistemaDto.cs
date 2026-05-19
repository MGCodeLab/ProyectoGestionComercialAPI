namespace Application.Dtos.Catalogo;

public class ModuloSistemaDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
