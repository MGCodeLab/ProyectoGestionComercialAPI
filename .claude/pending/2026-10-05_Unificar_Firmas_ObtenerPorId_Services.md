# PENDIENTE: Unificar firmas de ObtenerPorId en Services (ex Refinement 2 de PR#15)

**Fecha:** 2026-10-05
**Prioridad:** Media
**Decisión requerida de:** Miguel (arquitectura)

## Contexto
Los services no comparten firma: `ObtenerPorId` vs `ObtenerPorIdAsync`, con o sin parámetro `tracking`
(ej. CondicionPago y Proveedor/ListaPrecio sin `tracking`). CategoriaProducto y MarcaProducto ya se renombraron a `ObtenerPorId`.

## Por qué los extension methods no se hicieron
No hay una firma común sobre la cual definirlos. Un overload por servicio sería boilerplate y oculta el parámetro `tracking`.

## Propuesta
Unificar primero las interfaces a `ObtenerPorId(int id, bool tracking, CancellationToken ct)` en un PR aparte;
reevaluar después si hace falta un default de `tracking`.

## Relacionado
- Pendiente de la auditoría de homogeneidad: Profiles de AutoMapper, `AsNoTracking`, `ParametroSistemaService` minificado.
