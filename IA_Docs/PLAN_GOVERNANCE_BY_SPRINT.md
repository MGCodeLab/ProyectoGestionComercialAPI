# Plan Governance by Sprint — Convención de Gobernanza

**Creado:** 2026-05-16  
**Propósito:** Establecer estructura clara de planes por sprint  
**Aplica a:** Sprints 2-5 en adelante

---

## 📋 Visión General

### Problema Identificado

Antes: Un archivo `catalogo-roadmap-sprints2-5.md` contenía todos los sprints (1-5) en un único documento. Esto causaba:
- Dificultad para mover sprints completados individualmente
- Mezcla de visión macro con detalles ejecutables
- No permitía segregar por tarjeta/sprint específico

### Solución Implementada

**Estructura segregada por sprint:**
- **1 archivo = 1 sprint** en `plans/active/`
- **Formato:** `YYYY-MM-DD_catalogo-sprintN-{nombre}.md`
- **Cada archivo:** Detallado pero específico (como tarjeta Jira/Azure DevOps)
- **Al completar:** Mover a `plans/completed/` con status actualizado
- **Visión macro:** Centralizada en `.claude/PROYECTO_VISION_COMPLETA.md`

---

## 📁 Estructura Nueva

### Active Plans (En ejecución)

```
.claude/plans/active/
├── 2026-05-16_catalogo-sprint2-organizacion.md       ✅ COMPLETADO
├── 2026-05-16_catalogo-sprint3-fiscal.md             ⏳ PENDIENTE
├── 2026-05-16_catalogo-sprint4-producto.md           ⏳ PENDIENTE
└── 2026-05-16_catalogo-sprint5-comercial.md          ⏳ PENDIENTE
```

### Completed Plans (Histórico)

```
.claude/plans/completed/
├── 2026-05-10_catalogo-sprint1-catálogos-base.md     ✅ COMPLETADO
└── 2026-05-16_catalogo-sprint2-organizacion.md       ✅ COMPLETADO (tras testing)
```

### Vision Macro

```
.claude/
├── PROYECTO_VISION_COMPLETA.md                       📊 Visión general
│   - Contexto del proyecto
│   - Decisiones arquitectónicas aprobadas
│   - Mapa de dependencias (alto nivel)
│   - Timeline estimado
│   - Riesgos globales
│   - Decisiones pendientes
```

---

## 🎯 Contenido de Cada Sprint Plan

Cada archivo de sprint debe incluir:

### 1. Encabezado (Identificación)
```markdown
# Sprint N: {Contexto} ({Entidades})

**Estado:** ✅ COMPLETADO / ⏳ PENDIENTE / 🔴 BLOQUEADO
**Fecha Inicio:** YYYY-MM-DD HH:MM
**Duración Estimada:** X horas
**Rama:** catalogo-base/sprint_N
**Complejidad:** 🟢 BAJA / 🟡 MEDIA / 🔴 ALTA
```

### 2. Objetivo
```markdown
## 📋 Objetivo

Párrafo claro de qué se logra con este sprint.
Dependencias previas.
Qué desbloquea este sprint.
```

### 3. Entidades (1-4 máximo)
```markdown
## 🎯 Entidades a Crear (N)

Para cada entidad:
- Nombre → `schema.TablaPlural`
- Estructura SQL completa
- Características específicas
- Restricciones de negocio
```

### 4. Archivos a Crear
```markdown
## 📁 Archivos a Crear: ~X nuevos

Por categoría:
- Commands (N)
- Handlers (N)
- Validators (N)
- DTOs (N)
- ... resto de layers
```

### 5. Riesgos Técnicos
```markdown
## ⚠️ Riesgos / Decisiones Especiales

Si hay problemas conocidos:
- Race conditions
- ALTER TABLE
- Self-references
- Etc.

Incluir mitigación específica.
```

### 6. Implementación Details
```markdown
## 🔧 Decisiones de Implementación

Patterns específicos si aplican:
- Transacciones (isolation level)
- Concurrencia
- Validaciones complejas
- Endpoints especiales
```

### 7. Checklist
```markdown
## ✅ Checklist Pre-Implementación

- [ ] Item 1
- [ ] Item 2
- [ ] Compilación: 0 errores
```

### 8. Métricas
```markdown
## 📊 Métricas Esperadas

| Item | Planeado |
|------|----------|
| Entidades | N |
| Commands | N |
| ... | |
| Tiempo | X-Y horas |
```

### 9. Referencias
```markdown
## 🔗 Referencias

- Dependencias: SprintN, SprintN+1
- Bloquea: SprintN+1, Módulo Ventas
- Patrón referencia: {archivo en codebase}
```

---

## 🔄 Flujo de Movimiento de Sprints

### Estado: PENDIENTE → COMPLETADO

```
1. Sprint está en: .claude/plans/active/2026-05-16_catalogo-sprintN-{nombre}.md
   Status: ⏳ PENDIENTE

2. Developer implementa en rama: catalogo-base/sprint_N

3. Testing completa: ✅ VERIFICADO

4. Commit realizado: {commit-hash}
   Actualizar archivo sprint:
   - Status → ✅ COMPLETADO
   - Fecha Finalización → YYYY-MM-DD HH:MM
   - Commit → {hash}

5. Mover archivo a completed:
   mv .claude/plans/active/2026-05-16_catalogo-sprintN-{nombre}.md \
      .claude/plans/completed/2026-05-16_catalogo-sprintN-{nombre}.md

6. Documentar en History Changed/
   └── 20260516_T1430_feat_SprintN{Contexto}/
       └── SUMMARY.md (con detalles de lo realizado)
```

---

## 📝 Documentación Obligatoria por Sprint

Cuando se completa un sprint:

### 1. Actualizar Plan File
```markdown
# En: .claude/plans/active/2026-05-16_catalogo-sprintN-{nombre}.md

**Estado:** ✅ COMPLETADO
**Fecha Finalización:** 2026-05-16 14:30
**Commit:** {hash}
```

### 2. Crear History Changed Entry
```
History Changed/20260516_T1430_feat_SprintN{Contexto}/
├── SUMMARY.md          (Resumen ejecutivo)
└── CHANGES.log         (Detalle de cambios por archivo)
```

### 3. Crear USUARIO_DOCS Entry
```
USUARIO_DOCS/
└── avance_XX_2026-05-16_SprintN{Contexto}.md
    (Resumen funcional para continuidad de sesión)
```

### 4. Actualizar Execution Status
```
.claude/execution-status/
└── catalogo-base-status.md
    (Actualizar % completado, errores, próximos pasos)
```

---

## 🎯 Convención de Nombres

### Plan Files
```
.claude/plans/active/YYYY-MM-DD_catalogo-sprintN-{nombre}.md
.claude/plans/completed/YYYY-MM-DD_catalogo-sprintN-{nombre}.md

Ejemplo:
2026-05-16_catalogo-sprint2-organizacion.md
2026-05-16_catalogo-sprint3-fiscal.md
```

### Git Branches
```
catalogo-base/sprint_N

Ejemplo:
catalogo-base/sprint_1
catalogo-base/sprint_2
catalogo-base/sprint_3
```

### Commits
```
feat(catalogo): Sprint N — {Contexto} [✅ COMPLETADO | ⏳ WIP]

Ejemplo:
feat(catalogo): Sprint 2 — Organización ✅ COMPLETADO
feat(catalogo): Sprint 3 — Fiscal [WIP: 70%]
```

---

## 🔄 Flujo de Actualización Diaria

Durante desarrollo de un sprint:

1. **Morning:** Actualizar status en `catalogo-base-status.md`
   ```
   Sprint 2: 70% → Empresa creada, Sucursal en progreso
   ```

2. **During work:** Documentar hallazgos en `IA_Docs/COMMON_ISSUES_AND_FIXES.md`
   ```
   Sección 8: Nueva lección aprendida
   ```

3. **End of session:** Actualizar sprint plan file
   ```
   Status: ⏳ En progreso (70%)
   Próximo: Completar Almacén + testing
   ```

4. **Completion:** Mover a `completed/`, actualizar todo

---

## 🚨 Regla Crítica

> **Cada sprint debe ser tratado como una tarjeta independiente:**
> - Archivo separado = facilidad para rastrearlo
> - Movimiento de active → completed = visibilidad clara
> - Una entidad de versión = un punto de referencia histórico

---

## 📊 Comparativa: Antes vs Después

### ❌ ANTES (Problema)
```
.claude/plans/active/
└── 2026-05-10_catalogo-roadmap-sprints2-5.md
    (450+ líneas, contiene Sprints 1-5, imposible mover individual)
```

### ✅ DESPUÉS (Solución)
```
.claude/plans/active/
├── 2026-05-16_catalogo-sprint2-organizacion.md    (250 líneas)
├── 2026-05-16_catalogo-sprint3-fiscal.md          (280 líneas)
├── 2026-05-16_catalogo-sprint4-producto.md        (230 líneas)
└── 2026-05-16_catalogo-sprint5-comercial.md       (220 líneas)

.claude/plans/completed/
├── 2026-05-10_catalogo-sprint1-catálogos-base.md
└── 2026-05-16_catalogo-sprint2-organizacion.md    (tras completar)
```

---

## 🔗 Referencias

- **Para crear nuevo plan:** Usar template de este documento
- **Cuando terminar sprint:** Mover a `completed/` + documentar en History Changed
- **Visión macro:** `.claude/PROYECTO_VISION_COMPLETA.md`
- **Ejecución actual:** `.claude/execution-status/catalogo-base-status.md`

---

**Convención vigente desde:** 2026-05-16  
**Aplicable a:** Todos los sprints posteriores a Sprint 2  
**Propósito:** Mejora de gobernanza y trazabilidad  
