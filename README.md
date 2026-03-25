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
│   ├── DependencyInjection.cs
│   └── Program.cs
│
├── Application/
│   ├── Behaviours/
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
* Evitar try/catch repetitivos

---

### 🔸 Validaciones desacopladas

Uso de **FluentValidation** junto con pipeline de MediatR:

* Validaciones automáticas antes de ejecutar handlers
* Código limpio y reutilizable

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

---

## 🔄 Próximas mejoras

* Response Wrapper (estandarización de respuestas)
* Integración con Angular (frontend)
* CQRS completo (queries con MediatR opcional)
* Versionado de API
* Health Checks
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
* `v2.0.0`: Implementación de CQRS con MediatR, validaciones y middleware global

---

## 📌 Autor

Desarrollado por Miguel Gonzalez (MGCodeLab)