# Task Management

Sistema de administración de tareas desarrollado como solución técnica en .NET.

La aplicación permite crear, consultar, actualizar y eliminar tareas, además de aplicar filtros, paginación y consultar un reporte de tareas pendientes y vencidas por usuario.

## Tecnologías utilizadas

### Backend

- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQL Server
- Repository Pattern
- Unit of Work
- Inyección de dependencias
- Stored Procedures
- Triggers
- Auditoría de cambios

### Frontend

- ASP.NET Core MVC
- Razor Views
- JavaScript
- Fetch API
- Bootstrap
- Font Awesome
- CSS personalizado

## Funcionalidades

- Inicio de sesión.
- CRUD de tareas.
- Consulta de tareas por identificador.
- Filtros por:
  - Prioridad.
  - Estatus.
  - Usuario responsable.
  - Fecha inicial.
  - Fecha final.
- Paginación de resultados.
- Catálogos de prioridades, estatus y usuarios.
- Reporte de tareas pendientes y vencidas por usuario.
- Auditoría automática de cambios de estatus mediante trigger.
- Cliente JavaScript reutilizable para consumir la API.
- Manejo centralizado de respuestas y errores.

## Credenciales de prueba:
Usuario: maria.lopez@empresa.com
Password: 123456

## Estructura de la solución
```text
TaskManagement
├── database
│   └── TaskManagementDb_Instalacion.sql
├── TaskManagement.API
├── TaskManagement.UI
├── TaskManagement.sln
└── README.md
