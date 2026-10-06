# Avance 09: Homogeneidad de Handlers de Actualización (AuditableUpdateHandlerBase) — ✅ COMPLETADO

**Fecha:** 2026-10-05
**Rama:** `catalogo-base/validators`
**Status:** ✅ Implementado, compilado (0 errores) y probado manualmente por Miguel

---

## Resumen

Todos los handlers `Actualizar` y `ActualizarEstado` heredan de `AuditableUpdateHandlerBase`, que asigna `FechaActualizacion = DateTime.UtcNow` de forma centralizada. Ya no es posible olvidar la auditoría en un update.

## Qué cambió
- 29 handlers `Actualizar` migrados a la clase base.
- 7 handlers `ActualizarEstado` (patrón A) migrados. Los 2 de patrón B (Proveedor, CondicionPago) conservan la lógica en el service por diseño.
- `FechaActualizacion` redundante eliminada de AlmacenService, EmpresaService y SucursalService.
- Commands de ModuloSistema y ParametroSistema ahora son `IRequest<Unit>`.
- `ObtenerPorIdAsync` → `ObtenerPorId` en CategoriaProducto y MarcaProducto (interfaces, services y controllers).
- Handlers lanzan `NotFoundException` (antes `InvalidOperationException`/`KeyNotFoundException`), por lo que un id inexistente responde 404.

## Riesgos y notas
- Los handlers `ActualizarEstado` pasan `null` como `IMapper` a la base. Si alguno llega a usar `Mapper.Map()` fallará en runtime.
- `ListaPrecioService.ActualizarEstado` quedó sin uso por handlers y aún asigna `FechaActualizacion` (candidato a limpieza).
- Las firmas de services siguen inconsistentes (`ObtenerPorId` vs `ObtenerPorIdAsync`, con/sin `tracking`).

## Próximos pasos
1. Abrir PR `catalogo-base/validators` → `main` (merge commit).
2. Decidir el Refinement 2 (`ServiceQueryExtensions`): unificar firmas de services o descartarlo. El archivo existe sin commit y sin uso.
3. Pendientes restantes de la auditoría de homogeneidad: Profiles de AutoMapper, `AsNoTracking`, `ParametroSistemaService` minificado.

## Documentación relacionada
- `IA_Docs/AUDITABLE_ENTITY_UPDATE_HANDLERS_PATTERN.md`
- `History Changed/20260628_T1100_refactor_ActualizarEstadoHandlersAndServiceCleanup/SUMMARY.md`
- `.claude/execution-status/2026-06-28_PR15_Refinements_Execution.md`
