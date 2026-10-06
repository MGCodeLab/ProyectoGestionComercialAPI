# PENDIENTE: Hallazgos del code review de PR#15 (no corregidos en el PR)

**Fecha:** 2026-10-05
**Origen:** /code-review high sobre PR#15. Los hallazgos graves (Actualizar de CondicionPago, ListaPrecio y Proveedor pisaban PublicId/FechaRegistro/Activo) ya se corrigieron en el PR.

## Pendientes
1. `UpdateAuditableEntity` recibe `CancellationToken` y no lo usa (quitar parametro o `Func<CancellationToken, Task>`).
2. Los 17 handlers `ActualizarEstado` pasan `null` como `IMapper` a la base (crear base sin mapper o sobrecarga).
3. Soft-delete (`Eliminar` de CategoriaProducto y MarcaProducto) no asigna `FechaActualizacion`.
4. **Decision de Miguel:** interceptor de `SaveChanges` sobre `AuditableEntity` para garantizar `FechaActualizacion` sin depender de handlers (los services de Empresa/Sucursal/Almacen ya no la asignan).
5. Excepciones inconsistentes: `Actualizar`/`Eliminar` de Almacen, Empresa, Sucursal, CategoriaProducto y MarcaProducto aun lanzan `KeyNotFoundException`/`InvalidOperationException` (500) mientras `ActualizarEstado` lanza `NotFoundException` (404).
6. `ListaPrecioService.ActualizarEstado` (y su interfaz) quedo sin uso por handlers; CondicionPago/Proveedor siguen con patron B.
7. Logs con `{@request}` serializan comandos completos; preferir solo el Id.
8. `ReverseMap()` agregados en Profiles sin uso (ParametroSistema, TipoDocumento, UnidadMedida).
9. `ActualizarCondicionPago/ListaPrecio` y `ObtenerPorId` de esos services no tienen parametro `tracking` (ver pendiente de unificacion de firmas).
