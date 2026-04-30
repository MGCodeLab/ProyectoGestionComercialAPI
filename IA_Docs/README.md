# IA_DOCS — Technical Knowledge Base

**Propósito:** Documentación técnica para futuras sesiones de IA, desarrolladores senior, y auditoría.  
**Audiencia:** LLMs, IAs, arquitectos, developers enterprise.  
**Organización:** Modular, específica por tema, no genérica.

---

## 📚 Documentación Disponible

### 🎯 Punto de Partida (Comienza aquí)

| Documento | Descripción | Cuándo usarlo |
|-----------|-------------|---------------|
| **PROJECT_STATUS.md** | Estado actual completo | Primera sesión nueva, necesitas saber dónde estamos |
| **IMPLEMENTATION_PATTERNS.md** | Estándar exacto para módulos | Vas a crear un nuevo módulo |
| **ARCHITECTURE_DECISIONS.md** | Decisiones registradas | Entender el por qué de las decisiones |

---

### 🔧 Técnico Detallado

| Documento | Descripción | Cuándo usarlo |
|-----------|-------------|---------------|
| **DATABASE_ARCHITECTURE.md** | Schema SQL, constraints, patrones | Modificas base de datos |
| **COMMON_ISSUES_AND_FIXES.md** | Problemas resueltos, troubleshooting | Encuentra un error, aquí está la solución |
| **PROJECT_KNOWLEDGE_BASE.md** | Context para IAs, estructura técnica | Necesitas entender cómo funciona todo |

---

### 📋 Referencia

| Documento | Descripción | Cuándo usarlo |
|-----------|-------------|---------------|
| **DATABASE_SETUP_INSTRUCTIONS.md** | Setup inicial de BD | Primera vez configurando BD |
| **PENDING_IMPROVEMENTS.md** | Mejoras futuras documentadas | Planificación de siguiente versión |
| **ITERATION_3_PLAN.md** | Histórico de plans | Referencia histórica |

---

## 🔄 Workflow Típico (Para IAs)

### Sesión Nueva sobre el Proyecto
1. Lee **PROJECT_STATUS.md** → ¿Dónde estamos ahora?
2. Lee **ARCHITECTURE_DECISIONS.md** → ¿Por qué está así?
3. Consulta documento específico si necesitas profundidad

### Vas a Crear un Nuevo Módulo
1. Lee **IMPLEMENTATION_PATTERNS.md** → Estándar exacto
2. Copia estructura de Productos o Clientes exactamente
3. Usa **DATABASE_ARCHITECTURE.md** para patrones SQL

### Encontraste un Error
1. Busca en **COMMON_ISSUES_AND_FIXES.md** → Puede estar resuelto
2. Si no está, diagnostica y luego documenta tu solución aquí

### Modificas Base de Datos
1. Lee **DATABASE_ARCHITECTURE.md** sección relevante
2. Entiende constraints, índices, decisiones existentes
3. Documenta tu cambio (razón + pattern usado)

---

## 📊 Estructura de Documentación

```
IA_Docs/
├── README.md (este archivo)
│
├── 🎯 PUNTO DE PARTIDA
│   ├── PROJECT_STATUS.md           (¡EMPIEZA AQUÍ!)
│   ├── IMPLEMENTATION_PATTERNS.md   (Estándar obligatorio)
│   └── ARCHITECTURE_DECISIONS.md    (Por qué)
│
├── 🔧 TÉCNICO
│   ├── DATABASE_ARCHITECTURE.md     (Schema + patrones SQL)
│   ├── COMMON_ISSUES_AND_FIXES.md   (Troubleshooting)
│   └── PROJECT_KNOWLEDGE_BASE.md    (Context general)
│
└── 📋 REFERENCIA
    ├── DATABASE_SETUP_INSTRUCTIONS.md
    ├── PENDING_IMPROVEMENTS.md
    └── ITERATION_*.md (histórico)
```

---

## 🎯 Por Tarea

### "Necesito crear un nuevo módulo (Ventas, Compras, etc.)"
**Lectura obligatoria:**
1. `PROJECT_STATUS.md` — Estado actual
2. `IMPLEMENTATION_PATTERNS.md` — Estándar exacto
3. `DATABASE_ARCHITECTURE.md` — Patrones SQL

**Ejecuta:**
- Copia estructura Productos/Clientes exactamente
- Sigue patrones en IMPLEMENTATION_PATTERNS.md línea por línea
- No inventes nuevos patrones

---

### "Encontré un error o problema raro"
**Lectura recomendada:**
1. `COMMON_ISSUES_AND_FIXES.md` — Puede estar resuelto
2. `PROJECT_KNOWLEDGE_BASE.md` — Entender contexto
3. Busca en commits recientes: `git log --grep="fix" --oneline`

---

### "Necesito entender una decisión arquitectónica"
**Lectura:**
1. `ARCHITECTURE_DECISIONS.md` — Decisiones registradas (ADR format)
2. `DATABASE_ARCHITECTURE.md` — Decisiones SQL específicas
3. `PROJECT_STATUS.md` — Contexto de decisiones recientes

---

### "Tengo que modificar la base de datos"
**Lectura obligatoria:**
1. `DATABASE_ARCHITECTURE.md` — Patrones, constraints, índices
2. Sección "Schema Versioning Strategy" — Cómo versionar cambios
3. `COMMON_ISSUES_AND_FIXES.md` — Problemas evitables

**Checklist antes de hacer cambios:**
- [ ] ¿Sigo el patrón AuditableEntity?
- [ ] ¿Es PERSISTED la computed column?
- [ ] ¿Uso filtered index para NULL + unique?
- [ ] ¿Está documentado el por qué en comentarios SQL?
- [ ] ¿Creé migration script para bases existentes?

---

### "Necesito entender cómo funciona un módulo específico"
**Módulos disponibles:**
- **Productos** → Patrón base (referencia)
- **Clientes** → Patrón base + features avanzadas (computed column, soft delete)
- **Auth** → Autenticación (JWT, BCrypt, roles)

**Lectura:**
1. `PROJECT_STATUS.md` sección "Módulos Completados"
2. Lee el código:
   - `Domain/Comercial/{Modulo}.cs` (Entity)
   - `Application/Features/{Modulo}/` (CQRS)
   - `Application/Dtos/{Modulo}/` (Validación)
   - `Database/02_Tablas/{numero}_{Modulo}.sql` (DDL)

---

## ⚡ Quick Reference

### Patrones Clave (Memorizalos)
- **Entity:** Hereda `AuditableEntity`
- **Configuration:** Hereda `AuditableEntityConfiguration<T>`
- **DTOs:** Llevan atributos de validación + `required` keyword
- **Commands:** `CrearXxxCommand`, `ActualizarXxxCommand`, `EliminarXxxCommand`
- **Handlers:** Usan `_mapper.Map()`, nunca asignación manual
- **Services:** Métodos `ObtenerTodos()`, `ObtenerPorId()`, `Crear()`, `Actualizar()`, `Eliminar()`
- **Controllers:** 7 endpoints estándar (GET, GET/{id}, POST, PUT, PATCH inactivar/activar, DELETE)

### Decisiones Clave (No cambies)
- ✅ Soft delete = auditoría, no ocultación (GET retorna todos)
- ✅ SQL Server filtered indexes para NULL + unique
- ✅ BCrypt HS256, cost factor 11-12
- ✅ AutoMapper bidireccional siempre
- ✅ FluentValidation + atributos en DTOs
- ✅ MediatR para Commands, Services para Queries

---

## 🔒 Reglas Inviolables

1. **No cambies patrones** sin consultar primero
2. **Nuevo módulo = copia Producto/Cliente exactamente**
3. **DTOs con validación SIEMPRE**
4. **Computed columns en Configuration.cs AND SQL DDL**
5. **Soft delete es auditoría, no ocultación**
6. **No uses HasQueryFilter global**
7. **Scripts SQL versionados, no EF Migrations**
8. **Índices filtered para NULLs + unique**

---

## 📞 Contacto & Contexto

**Dueño:** Miguel González Cuevas (MGCodeLab)  
**Email:** gonzalezcuevasmiguelignacio@gmail.com  
**Proyecto:** Nexus-ERP v3.0.0 (Gestión Comercial)  
**Stack:** .NET 8 + SQL Server + Angular  
**Tipo:** Producto real, destino producción

---

## 📈 Versionado

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 3.0.0 | 2026-04-30 | Base lista: Productos, Clientes, Auth. SQL constraint fixed. Documentación completa. |
| 3.1.0 | TBD | Módulo Ventas |
| 3.2.0 | TBD | Módulo Compras |
| 3.3.0 | TBD | Módulo Inventario |

---

**Última actualización:** 2026-04-30  
**Estado:** ✅ Documentación completa y actual  
**Próxima revisión:** Después de implementar Ventas
