# PENDIENTE: Hallazgos del code review de PR#15 (no corregidos en el PR)

**Fecha:** 2026-10-05
**Origen:** /code-review high sobre PR#15. Los hallazgos graves (Actualizar de CondicionPago, ListaPrecio y Proveedor pisaban PublicId/FechaRegistro/Activo) ya se corrigieron en el PR.

## Pendientes
1. `UpdateAuditableEntity` recibe `CancellationToken` y no lo usa (quitar parametro o `Func<CancellationToken, Task>`).
2. Los 17 handlers `ActualizarEstado` pasan `null` como `IMapper` a la base (crear base sin mapper o sobrecarga).
3. ~~Soft-delete de `Eliminar` (CategoriaProducto, MarcaProducto) sin `FechaActualizacion`~~ — **DECISION DE MIGUEL (2026-10-06): no se toca.** `DELETE` esta pensado como borrado real; cada modulo definira sus validaciones de eliminacion (dependencias) en su momento. Hoy 17 modulos hacen hard delete y Categoria/Marca hacen soft-delete temporal (equivale a Inactivar). Al definir las validaciones de cada modulo, alinear Categoria y Marca al borrado real.
4. **Decision de Miguel:** interceptor de `SaveChanges` sobre `AuditableEntity` para garantizar `FechaActualizacion` sin depender de handlers (los services de Empresa/Sucursal/Almacen ya no la asignan).
5. Excepciones inconsistentes: `Actualizar`/`Eliminar` de Almacen, Empresa, Sucursal, CategoriaProducto y MarcaProducto aun lanzan `KeyNotFoundException`/`InvalidOperationException` (500) mientras `ActualizarEstado` lanza `NotFoundException` (404).
6. `ListaPrecioService.ActualizarEstado` (y su interfaz) quedo sin uso por handlers; CondicionPago/Proveedor siguen con patron B.
7. Logs con `{@request}` serializan comandos completos; preferir solo el Id.
8. `ReverseMap()` agregados en Profiles sin uso (ParametroSistema, TipoDocumento, UnidadMedida).
9. `ActualizarCondicionPago/ListaPrecio` y `ObtenerPorId` de esos services no tienen parametro `tracking` (ver pendiente de unificacion de firmas).

## Segunda ronda de review (2026-10-06) — hallazgos nuevos
10. **[PRIORIDAD ALTA — integridad de datos, preexistente desde Sprint 4, commit d4840be]** `ActualizarCategoriaProductoHandler:37` llama `EsDescendienteDeAsync(nuevoPadre, command.Id)` con los argumentos invertidos (correcto: `(command.Id, nuevoPadre)`). Efecto: rechaza movimientos validos (bajo el abuelo), **acepta ciclos reales** y `PadreId == Id`; con un ciclo en BD el `while` del validador no termina (tambien lo usa `Crear`). Ademas `Actualizar` no valida el maximo de 3 niveles que si exige `Crear`.
11. `CrearListaPrecioHandler`: baja la default anterior en un `SaveChanges` aparte y sin `FechaActualizacion` (mismo problema que se corrigio en `Actualizar`); si falla el insert queda sin default.
12. `ListaPrecioService.Eliminar` y `ActualizarEstado` aun lanzan `InvalidOperationException` (500 con id inexistente).
13. Services de Categoria, Marca, Almacen, Empresa, Sucursal y Proveedor siguen con `Update()` sobre entidades ya trackeadas (reescribe todas las columnas); en ListaPrecio ya se corrigio.
14. Errores de reglas de negocio (ciclo, profundidad) lanzan `InvalidOperationException` (500); deberian ser 400.
15. `MonedasController.Crear` usa `HttpContext.RequestAborted` en vez del token recibido.
