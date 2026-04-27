# Tareas Pendientes — Mejoras Futuras

**Última actualización:** 2026-04-26  
**Estado:** En seguimiento para futuras iteraciones

---

## 1. Configuración Global de JSON Serialization (PRIORIDAD: MEDIA)

### Estado Actual
- `ApiResponse<T>` usa propiedades PascalCase (Success, Message, Data, Errors, TraceId)
- DTOs también usan PascalCase (ProductoDto, LoginResponseDto, etc.)
- El frontend actualmente funciona con esta configuración
- No hay una configuración global de `JsonSerializerOptions`

### Problema
- La serialización JSON es inconsistente con estándares REST (que típicamente usan camelCase)
- Si en el futuro el frontend cambia a esperar camelCase, habría que actualizar TODO

### Solución Propuesta
1. Agregar configuración global en `Program.cs`:
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

2. Opción alternativa: Usar `[JsonPropertyName]` en todos los DTOs (ya parcialmente hecho en Auth)

### Impacto
- **Riesgo:** El frontend podría dejar de funcionar si no está configurado para camelCase
- **Beneficio:** API más profesional, consistente con convenciones REST

### Decisión
Pendiente de validación con el equipo de frontend. Si deciden cambiar a camelCase en el futuro, este será el punto de inicio.

---

## 2. Validación en Clientes HTTP (PRIORIDAD: BAJA)

### Estado Actual
- Se agregaron atributos `[Required]`, `[StringLength]`, `[EmailAddress]`, etc. en DTOs
- Estos atributos solo aplican cuando ASP.NET hace Model Binding en Server
- El frontend no recibe información de validación automáticamente

### Oportunidad
- Implementar [OpenAPI/Swagger](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger) para documentar validaciones
- Usar `Swashbuckle` para generar especificación OpenAPI desde los atributos
- El frontend podría leer el esquema OpenAPI y validar antes de enviar

### Impacto
- Mejora DX (Developer Experience) del frontend
- Reduce requests innecesarios al servidor

### Nota
`AddOpenApi()` ya está en `Program.cs`, pero requiere configuración adicional.

---

## 3. Auditoría de Cambios en Entidades (PRIORIDAD: BAJA)

### Nota Técnica
El campo `FechaActualizacion` existe en todas las entidades `AuditableEntity`, pero actualmente se actualiza manualmente en cada Handler.

**Oportunidad futura:** Implementar interceptor de EF Core para automatizar esto:

```csharp
public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<AuditableEntity>();
    foreach (var entry in entries.Where(e => e.State == EntityState.Modified))
    {
        entry.Entity.FechaActualizacion = DateTime.UtcNow;
    }
    return base.SaveChangesAsync(cancellationToken);
}
```

---

## 4. Refresh Tokens (PRIORIDAD: BAJA)

### Nota
Actualmente los JWT tokens expiran en 60 minutos y el usuario debe re-loguear.

**Oportunidad futura:** Implementar refresh token pattern:
- Token JWT de acceso: 15 minutos (corta vida)
- Refresh token: 7 días (se almacena en BD)
- Endpoint POST /auth/refresh que acepta refresh token y devuelve nuevo access token

---

## 5. Role-Based Access Control ([Authorize]) (PRIORIDAD: MEDIA)

### Estado Actual
- `AuthController` tiene `[AllowAnonymous]`
- `ProductosController` y `ClientesController` están abiertos (sin `[Authorize]`)
- `AuthController.Me()` usa `[Authorize]`

### Próximo Paso
Una vez que el frontend envíe Bearer tokens en TODOS los requests:
1. Agregar `[Authorize]` en ProductosController
2. Agregar `[Authorize]` en ClientesController
3. Opcionalmente: Agregar `[Authorize(Roles = "ADMIN")]` para endpoints sensibles

---

## 6. Integración de Testing (PRIORIDAD: BAJA)

### Oportunidad
- No hay tests unitarios ni tests de integración
- Futura iteración podría incluir:
  - xUnit para tests unitarios
  - Tests de Handlers (LoginHandler, CrearClienteHandler, etc.)
  - Tests de integración con TestServer

---

## Checklist para Futuras Sesiones

- [ ] Validar con frontend que JSON serialization es correcta (camelCase vs PascalCase)
- [ ] Si hay cambios, revisar punto 1 (Configuración Global JSON)
- [ ] Evaluar necesidad de OpenAPI/Swagger (punto 2)
- [ ] Implementar interceptor para FechaActualizacion automática (punto 3)
- [ ] Implementar refresh token cuando escalemos autenticación (punto 4)
- [ ] Habilitar [Authorize] cuando frontend esté listo (punto 5)
- [ ] Planificar suite de tests (punto 6)

---

**Nota:** Este documento es un registro de decisiones que se pueden revisar y ajustar según las necesidades del proyecto evolucionen.
