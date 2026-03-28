# 🧾 Gestión Comercial

Sistema de gestión comercial en desarrollo, enfocado en ventas, compras, inventario y reportes.

Este proyecto está diseñado siguiendo principios de **Clean Architecture** y **CQRS (Command Query Responsibility Segregation)**, con el objetivo de ser escalable, mantenible y preparado para crecimiento futuro.

---

## 🚀 Tecnologías utilizadas

* .NET 10
* Entity Framework Core
* SQL Server (Docker)
* AutoMapper
* MediatR (CQRS)
* FluentValidation
* Arquitectura en capas (Clean Architecture)

---

## 🧱 Arquitectura del proyecto

El sistema está organizado en múltiples capas separadas por responsabilidad:

GestionComercial/
├── API.GestionComercial/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Extensions/ 
│   ├── DependencyInjection.cs
│   └── Program.cs
│
├── Application/
│   ├── Behaviours/
│   ├── Common/
│   │   └── Models/ (ApiResponse) 
│   ├── Dtos/
│   │   └── Producto/
│   ├── Exceptions/
│   ├── Features/
│   │   └── Productos/
│   │       ├── Crear/
│   │       ├── Actualizar/
│   │       ├── ActualizarEstado/
│   │       └── Eliminar/
│   ├── Interfaces/
│   └── Mappings/
│       └── Productos/
│
├── Domain/
│   └── Catalogo/
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── AppDbContext.cs
│   └── Repository/
│
└── Database/
├── 00_Inicializacion/
├── 01_Schemas/
├── 02_Tablas/
├── 03_Seeds/
└── 04_Procedures/

---

## 🧠 Descripción de capas

### 🔹 API

Contiene los controladores REST.
Responsable de exponer los endpoints y delegar la lógica mediante MediatR.

Incluye:

* Middleware global para manejo de excepciones
* Extensions para respuestas estandarizadas
* Versionado básico de endpoints 
* Configuración de dependencias

---

### 🔹 Application

Contiene la lógica de aplicación y reglas del sistema:

* DTOs
* Commands (CQRS)
* Handlers (MediatR)
* Validators (FluentValidation)
* Interfaces
* Mapeos (AutoMapper)
* Excepciones personalizadas
* Response Wrapper (`ApiResponse`)

---

### 🔹 Domain

Contiene las entidades del negocio.
No depende de ninguna otra capa.

---

### 🔹 Infrastructure

Implementaciones técnicas:

* Entity Framework Core
* Repositorios
* Configuración de base de datos (Fluent API)

---

### 🔹 Database

Scripts SQL versionados:

* Creación de esquemas
* Tablas
* Seeds
* Procedimientos almacenados

---

## ⚙️ Enfoque arquitectónico

### 🔸 CQRS (implementación pragmática)

* **Commands (MediatR)** → operaciones que modifican el estado (Create, Update, Delete)
* **Queries (Services)** → operaciones de solo lectura

Esto permite mantener simplicidad sin caer en sobreingeniería.

---

### 🔸 Controllers delgados

Los controladores:

* No contienen lógica de negocio
* No acceden directamente a la base de datos
* Solo envían comandos o consumen servicios

---

### 🔸 Manejo global de errores

Se implementa un **middleware personalizado** para:

* Centralizar errores
* Retornar respuestas HTTP consistentes
* Incluir `traceId` para debugging 
* Evitar try/catch repetitivos

---

### 🔸 Validaciones desacopladas

Uso de **FluentValidation** junto con pipeline de MediatR:

* Validaciones automáticas antes de ejecutar handlers
* Código limpio y reutilizable

---

### 🔸 Response Wrapper (v2.1.0)

Se implementa un wrapper estándar para todas las respuestas de la API:

```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": {},
  "traceId": "..."
}
```

Beneficios:

* Respuestas consistentes en toda la API
* Mejor integración con frontend (Angular-ready)
* Manejo uniforme de errores
* Mayor claridad en contratos de API

---

### 🔸 Versionado de API (básico) 

Se implementa un versionado simple a nivel de rutas para preparar la evolución futura del sistema:

Ejemplo:

```text
/api/v1/productos
```

Beneficios:

* Permite evolucionar la API sin romper clientes existentes
* Base para versionado más robusto en el futuro
* Mejora el control de cambios en endpoints

---

## 🧪 Estado actual

✔ CRUD de Productos (completo)
✔ Implementación de CQRS con MediatR
✔ Validaciones con FluentValidation
✔ Middleware global de excepciones
✔ Logging en Handlers
✔ Separación por Features
✔ AutoMapper configurado por módulos
✔ Scripts de base de datos versionados
✔ Response Wrapper implementado 
✔ TraceId para debugging distribuido 
✔ Versionado básico de API 

---

## 🔄 Próximas mejoras

* Integración con Angular (frontend) 
* Versionado de API avanzado
* Health Checks
* CQRS completo (queries con MediatR opcional)
* Módulos de ventas, compras e inventario

---

## ⚙️ Cómo ejecutar el proyecto

1. Clonar el repositorio
2. Levantar SQL Server (Docker recomendado)
3. Ejecutar scripts de la carpeta `Database`
4. Configurar cadena de conexión en `appsettings.json`
5. Ejecutar la API

---

## 🏷️ Versionado

* `v1.0.0`: CRUD básico con arquitectura base
* `v2.0.0`: CQRS con MediatR + validaciones + middleware global
* `v2.1.0`: Response Wrapper + respuestas estandarizadas + traceId 

---

## 📌 Autor

Desarrollado por **Miguel Gonzalez (MGCodeLab)** 🚀
Full Stack Developer | .NET | Arquitectura de Software
