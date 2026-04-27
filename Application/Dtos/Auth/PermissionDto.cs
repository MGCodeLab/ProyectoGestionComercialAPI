namespace Application.Dtos.Auth
{
    /// <summary>
    /// DTO que representa un permiso específico.
    /// Vincula un recurso del sistema con una acción permitida.
    /// </summary>
    public class PermissionDto
    {
        /// <summary>Nombre del recurso (ej: "productos", "clientes", "usuarios", "ventas").</summary>
        public string Recurso { get; set; } = string.Empty;

        /// <summary>Acción permitida en el recurso (ej: "create", "edit", "delete", "view", "export").</summary>
        public string Accion { get; set; } = string.Empty;
    }
}
