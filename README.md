# 🧾 Gestión Comercial

Sistema de gestión comercial en desarrollo, enfocado en ventas, compras, inventario y reportes.

Este proyecto está diseñado siguiendo principios de **Clean Architecture**, con el objetivo de ser escalable, mantenible y preparado para crecimiento futuro.

---

## 🚀 Tecnologías utilizadas

- .NET 10
- Entity Framework Core
- SQL Server (Docker)
- AutoMapper
- Arquitectura en capas (Clean Architecture)

---

## 🧱 Arquitectura del proyecto

El sistema está organizado en múltiples capas separadas por responsabilidad:

GestionComercial/
├── API.GestionComercial/
│ └── Controllers
│
├── Application/
│ ├── Dtos/
│ │ └── Producto/
│ ├── Interfaces/
│ └── Mappings/
│ └── Productos/
│
├── Domain/
│ └── Catalogo/
│
├── Infrastructure/
│ ├── Persistence/
│ │ ├── Configurations/
│ │ └── AppDbContext.cs
│ └── Repository/
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
Responsable de exponer los endpoints y manejar las solicitudes HTTP.

---

### 🔹 Application
Contiene la lógica de aplicación:
- DTOs
- Interfaces
- Mapeos (AutoMapper)

---

### 🔹 Domain
Contiene las entidades del negocio.  
No depende de ninguna otra capa.

---

### 🔹 Infrastructure
Implementaciones técnicas:
- Entity Framework Core
- Repositorios
- Configuración de base de datos

---

### 🔹 Database
Scripts SQL versionados:
- Creación de esquemas
- Tablas
- Seeds
- Procedimientos almacenados

---

## 🧪 Estado actual

✔ CRUD de Productos  
✔ Arquitectura base implementada  
✔ AutoMapper configurado  
✔ Separación por capas  
✔ Scripts de base de datos versionados  

---

## 🔄 Próximas mejoras

- Implementación de MediatR (CQRS)
- Validaciones con FluentValidation
- Integración con Angular (frontend)
- Módulos de ventas, compras e inventario

---

## ⚙️ Cómo ejecutar el proyecto

1. Clonar el repositorio
2. Levantar SQL Server (Docker recomendado)
3. Ejecutar scripts de la carpeta `Database`
4. Configurar cadena de conexión en `appsettings.json`
5. Ejecutar la API

---

## 🏷️ Versionado

- `v1`: Versión inicial sin MediatR, con arquitectura base y CRUD funcional

---

## 📌 Autor

Desarrollado por Miguel (MGCodeLab)