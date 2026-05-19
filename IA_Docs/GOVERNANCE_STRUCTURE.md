# Governance Structure — Nexus-ERP `.claude/` Directory

**Última actualización:** 2026-05-15  
**Objetivo:** Trazabilidad técnica, continuidad entre sesiones, visibilidad de estado real

---

## 🏗️ Estructura de Directorios

```
.claude/
│
├── plans/                          # Planes arquitectónicos importantes
│   ├── active/                     # En ejecución
│   │   └── 2026-05-10_catalogo-roadmap-sprints2-5.md
│   ├── completed/                  # Finalizados
│   │   └── 2026-05-10_catalogo-base-sprint1-complete.md ✅
│   ├── paused/                     # Detenidos temporalmente
│   └── archived/                   # Obsoletos/reemplazados
│
├── execution-status/               # Snapshots de estado actual (rápida lectura)
│   └── catalogo-base-status.md     # Status actual del proyecto
│
├── pending/                        # Backlog técnico, decisiones pendientes
│   └── 2026-05-15_technical-backlog.md
│
└── IA_Docs/                        # Documentación permanente (guías, patrones)
    ├── GOVERNANCE_STRUCTURE.md     # Este archivo
    ├── VALIDATOR_SERVICE_PATTERN.md  # (existente)
    ├── COMMON_ISSUES_AND_FIXES.md  # (existente)
    └── PROJECT_STATUS.md           # (existente)
```

---

## 📋 Cada Tipo de Documento

### 1. `plans/` — Planes Arquitectónicos

#### `plans/active/` — Planes en Ejecución
**Qué va aquí:**
- Planes de sprints o fases en progreso
- Roadmaps multi-sprint
- Decisiones arquitectónicas aprobadas

**Ejemplos actuales:**
- `2026-05-10_catalogo-roadmap-sprints2-5.md` — Roadmap Sprints 2-5 (activo)

**Estructura requerida:**
```markdown
# [Plan Name]
**Estado:** ACTIVO
**Objetivo:** resumen de 1 línea
## Objetivo General
## Alcance
## Fases
## Dependencias
## Decisiones Arquitectónicas
## Riesgos
## Progreso
## Próximos Pasos
```

#### `plans/completed/` — Planes Finalizados
**Qué va aquí:**
- Planes completados al 100%
- Histórico de sprints finalizados
- Referencia para patrones validados

**Ejemplos actuales:**
- `2026-05-10_catalogo-base-sprint1-complete.md` — Sprint 1 COMPLETADO ✅

**Transición:** Cuando plan alcanza 100%, actualizar Status → COMPLETADO, mover archivo aquí.

---

### 2. `execution-status/` — Snapshots Estado Actual

**Qué va aquí:**
- Estado ACTUAL del proyecto (vivo, se actualiza)
- Quick-read de progreso real
- Problemas detectados
- Riesgos técnicos activos

**Ejemplos actuales:**
- `catalogo-base-status.md` — Estado Catálogos Base (actualizar cada sesión)

**Estructura requerida:**
```markdown
# Execution Status: [Project Name]
**Fecha:** YYYY-MM-DD
**Rama actual:** 
**Compilación:** status

## 🎯 Progreso General (visual bars)
## ✅ Módulos Completados
## ⏳ Módulos en Progreso
## 📋 Módulos Pendientes
## 🐛 Problemas Detectados
## 🚨 Riesgos Técnicos Activos
## 📊 Métricas
## 🎯 Próximos Pasos
```

**Actualización:** Cada sesión. Reemplaza completamente al anterior (es snapshot, no histórico).

---

### 3. `pending/` — Backlog Técnico & Decisiones Pendientes

**Qué va aquí:**
- Ideas no aprobadas
- Riesgos futuros
- Mejoras posibles
- Deuda técnica identificada
- Decisiones sin resolver
- Bloqueantes a mitigar

**Ejemplos actuales:**
- `2026-05-15_technical-backlog.md` — PD-01 a PD-12 (riesgos + ideas)

**Estructura por item:**
```markdown
### PD-XX: [Título]
**Prioridad:** 🔴 CRÍTICO | 🟡 ALTO | 🟢 MEDIO | 🔵 BAJO
**Contexto:** qué es
**Problema:** qué falta
**Impacto:** consecuencias
**Recomendación:** qué hacer
**Próximos pasos:** checklist
**Responsable:** quién decide/implementa
```

**Actualización:** Agregar nuevos, marcar resueltos, reordenar por prioridad.

---

## 🔄 Ciclo de Vida Completo

### Estado 1: ACTIVO (en ejecución)
```
Ubicación: plans/active/YYYY-MM-DD_descripcion.md
Status: ACTIVO
Acción: Actualizar progreso % cada sesión
```

### Estado 2: COMPLETADO (100%)
```
Ubicación: plans/completed/YYYY-MM-DD_descripcion.md
Status: COMPLETADO
Acción: Mover archivo + cerrar en execution-status
```

### Estado 3: PAUSED (detenido temporalmente)
```
Ubicación: plans/paused/YYYY-MM-DD_descripcion.md
Status: PAUSED
Razón: documentar por qué
Acción: Comunicar a Miguel + registrar en pending
```

### Estado 4: ARCHIVED (obsoleto)
```
Ubicación: plans/archived/YYYY-MM-DD_descripcion.md
Status: ARCHIVED
Razón: reemplazado por otra idea / ya no relevante
```

---

## 📝 Convenciones de Nombres

### Plans
```
plans/active/YYYY-MM-DD_descripcion-kebab-case.md
plans/completed/YYYY-MM-DD_descripcion-kebab-case.md
plans/paused/YYYY-MM-DD_descripcion-kebab-case.md
plans/archived/YYYY-MM-DD_descripcion-kebab-case.md

Ejemplos:
  2026-05-10_catalogo-base-sprint1-complete.md
  2026-05-10_catalogo-roadmap-sprints2-5.md
  2026-05-11_catalogo-sprint2-organizacion.md
```

### Execution Status
```
execution-status/{nombre-proyecto}-status.md

Ejemplo:
  catalogo-base-status.md
```

### Pending
```
pending/YYYY-MM-DD_{tema-principal}.md

Ejemplo:
  2026-05-15_technical-backlog.md
```

---

## 🤖 Responsabilidades Automáticas de Claude Code

**Después de CADA sesión con cambios:**
1. Actualizar `execution-status/{proyecto}.md` con estado real
2. Si plan completado: mover a `plans/completed/` + actualizar Status
3. Si nuevos riesgos: agregar a `pending/`
4. Si decisión resuelta: marcar en `pending/`

**Al INICIAR nueva sesión:**
1. Leer `plans/active/` → contexto
2. Leer `execution-status/` → estado actual
3. Leer `pending/` → bloqueantes
4. Resumir para usuario antes de continuar

**Durante ejecución:**
1. Si estado cambia → actualizar execution-status
2. Si nuevo riesgo → agregar a pending
3. Si plan termina → preparar cierre y move

**Antes de autorizar nueva fase:**
1. Validar planes previos en `completed/`
2. Confirmar execution-status actualizado
3. Listar bloqueantes críticos

---

## ✅ Checklist: Crear Nuevo Plan

- [ ] Ubicación: `plans/active/`
- [ ] Nombre: `YYYY-MM-DD_descripcion.md`
- [ ] Estado inicial: `ACTIVO`
- [ ] Secciones: Objetivo, Alcance, Fases, Dependencias, Riesgos, Progreso, Próximos pasos
- [ ] Propósito claro (no vago)
- [ ] Responsables definidos
- [ ] Fechas estimadas
- [ ] **PRESENTAR A MIGUEL PARA VALIDACIÓN antes de iniciar trabajo**

---

## ✅ Checklist: Completar Plan

- [ ] Actualizar progreso: 100%
- [ ] Status: COMPLETADO
- [ ] Mover: `plans/active/` → `plans/completed/`
- [ ] Actualizar execution-status
- [ ] Crear siguiente plan si hay (sprint siguiente)
- [ ] Marcar dependencias resueltas en pending

---

## ✅ Checklist: Cada Sesión

**INICIO:**
- [ ] Leer `plans/active/`
- [ ] Leer `execution-status/`
- [ ] Leer `pending/` (bloqueantes)

**FIN:**
- [ ] Actualizar `execution-status/{proyecto}.md`
- [ ] Mover planes completados a `completed/`
- [ ] Agregar nuevos bloqueantes a `pending/`
- [ ] Verificar próximos pasos claros

---

## 🚨 Red Flags

| Situación | Acción |
|-----------|--------|
| Plan sin actualización hace 3+ sesiones | Revisar si aún activo, mover a paused |
| Execution-status desactualizado | Actualizar ANTES de continuar |
| Riesgo crítico sin plan de mitigación | Crear plan de resolución |
| Plan sin progreso pero sin bloqueantes | Investigar atasco |
| >20 items en pending | Consolidar, priorizar, archivar si necesario |

---

## 🔗 Integración con IA_Docs

```
IA_Docs (principios/patrones permanentes)
    ↓
plans/ (aplicar principios a plan concreto)
    ↓
execution-status/ (registrar progreso real)
    ↓
pending/ (problemas encontrados)
    ↓
IA_Docs (documentar aprendizajes nuevos)
```

**IA_Docs:** Permanente, reutilizable  
**plans/:** Ejecución actual  
**execution-status/:** Vivo (snapshot actual)  
**pending/:** Backlog dinámico

---

## 📋 Estado Actual (2026-05-15)

```
PLANS:
  ✅ completed/2026-05-10_catalogo-base-sprint1-complete.md
  🔄 active/2026-05-10_catalogo-roadmap-sprints2-5.md

EXECUTION-STATUS:
  📊 catalogo-base-status.md (actualizado 2026-05-15)

PENDING:
  📝 2026-05-15_technical-backlog.md (12 items: 2 críticos, 3 altos)
```

---

**Última actualización:** 2026-05-15  
**Responsable:** Sistema de gobernanza de .claude/

