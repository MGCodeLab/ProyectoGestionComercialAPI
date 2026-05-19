namespace Application.Dtos.Catalogo;

public class CondicionPagoDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DiasCredito { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
