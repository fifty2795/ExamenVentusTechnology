/*
===============================================================================
 PROYECTO      : Task Management
 BASE DE DATOS : TaskManagementDb
 DESCRIPCIÓN   : Script completo de instalación para el examen técnico.

 CONTENIDO
   1. Creación y validación de la base de datos
   2. Tablas y restricciones
   3. Datos iniciales
   4. Índices
   5. Stored procedure de reporte
   6. Trigger de auditoría
   7. Validaciones finales

 INSTRUCCIONES
   - Ejecutar el archivo completo en SQL Server Management Studio.
   - El script crea la base de datos si no existe.
   - Por seguridad, se detiene si detecta tablas principales ya existentes.
   - Las contraseñas incluidas son datos de prueba y están en texto plano.
===============================================================================
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/*=============================================================================
  1. CREACIÓN DE LA BASE DE DATOS
=============================================================================*/

IF DB_ID(N'TaskManagementDb') IS NULL
BEGIN
    PRINT N'Creando base de datos TaskManagementDb...';
    CREATE DATABASE [TaskManagementDb];
END
ELSE
BEGIN
    PRINT N'La base de datos TaskManagementDb ya existe.';
END;
GO

USE [TaskManagementDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Evita sobrescribir accidentalmente una instalación existente. */
IF OBJECT_ID(N'dbo.Tareas', N'U') IS NOT NULL
   OR OBJECT_ID(N'dbo.Usuarios', N'U') IS NOT NULL
   OR OBJECT_ID(N'dbo.CatEstatus', N'U') IS NOT NULL
   OR OBJECT_ID(N'dbo.CatPrioridades', N'U') IS NOT NULL
BEGIN
    THROW 50001,
          'La base de datos ya contiene objetos de TaskManagement. Use una base vacía o elimine la instalación anterior.',
          1;
END;
GO

/*=============================================================================
  2. TABLAS Y RESTRICCIONES
=============================================================================*/

/*-----------------------------------------------------------------------------
  Catálogo de estatus
-----------------------------------------------------------------------------*/
CREATE TABLE dbo.CatEstatus
(
    IdEstatus INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_CatEstatus
        PRIMARY KEY CLUSTERED (IdEstatus),

    CONSTRAINT UQ_CatEstatus_Nombre
        UNIQUE NONCLUSTERED (Nombre),

    CONSTRAINT CK_CatEstatus_Nombre
        CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

/*-----------------------------------------------------------------------------
  Catálogo de prioridades
-----------------------------------------------------------------------------*/
CREATE TABLE dbo.CatPrioridades
(
    IdPrioridad INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_CatPrioridades
        PRIMARY KEY CLUSTERED (IdPrioridad),

    CONSTRAINT UQ_CatPrioridades_Nombre
        UNIQUE NONCLUSTERED (Nombre),

    CONSTRAINT CK_CatPrioridades_Nombre
        CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

/*-----------------------------------------------------------------------------
  Usuarios
-----------------------------------------------------------------------------*/
CREATE TABLE dbo.Usuarios
(
    IdUsuario INT IDENTITY(1,1) NOT NULL,
    NombreCompleto NVARCHAR(150) NOT NULL,
    CorreoElectronico NVARCHAR(200) NOT NULL,
    Activo BIT NOT NULL
        CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    FechaCreacion DATETIME2(0) NOT NULL
        CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT (SYSUTCDATETIME()),
    [Password] NVARCHAR(255) NOT NULL,

    CONSTRAINT PK_Usuarios
        PRIMARY KEY CLUSTERED (IdUsuario),

    CONSTRAINT UQ_Usuarios_CorreoElectronico
        UNIQUE NONCLUSTERED (CorreoElectronico),

    CONSTRAINT CK_Usuarios_NombreCompleto
        CHECK (LEN(LTRIM(RTRIM(NombreCompleto))) > 0),

    CONSTRAINT CK_Usuarios_CorreoElectronico
        CHECK (LEN(LTRIM(RTRIM(CorreoElectronico))) > 0)
);
GO

/*-----------------------------------------------------------------------------
  Tareas
-----------------------------------------------------------------------------*/
CREATE TABLE dbo.Tareas
(
    IdTarea INT IDENTITY(1,1) NOT NULL,
    Titulo NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    IdPrioridad INT NOT NULL,
    FechaCreacion DATETIME2(7) NOT NULL
        CONSTRAINT DF_Tareas_FechaCreacion DEFAULT (SYSUTCDATETIME()),
    FechaInicio DATE NULL,
    FechaFinalizacion DATE NULL,
    FechaLimite DATE NOT NULL,
    IdEstatus INT NOT NULL,
    IdUsuarioResponsable INT NOT NULL,

    CONSTRAINT PK_Tareas
        PRIMARY KEY CLUSTERED (IdTarea),

    CONSTRAINT FK_Tareas_CatPrioridades
        FOREIGN KEY (IdPrioridad)
        REFERENCES dbo.CatPrioridades(IdPrioridad),

    CONSTRAINT FK_Tareas_CatEstatus
        FOREIGN KEY (IdEstatus)
        REFERENCES dbo.CatEstatus(IdEstatus),

    CONSTRAINT FK_Tareas_Usuarios
        FOREIGN KEY (IdUsuarioResponsable)
        REFERENCES dbo.Usuarios(IdUsuario),

    CONSTRAINT CK_Tareas_Titulo
        CHECK (LEN(LTRIM(RTRIM(Titulo))) > 0),

    CONSTRAINT CK_Tareas_FechaFinalizacion
        CHECK
        (
            FechaFinalizacion IS NULL
            OR FechaInicio IS NULL
            OR FechaFinalizacion >= FechaInicio
        )
);
GO

/*-----------------------------------------------------------------------------
  Auditoría de cambios de estatus
-----------------------------------------------------------------------------*/
CREATE TABLE dbo.AuditoriaEstatusTarea
(
    IdAuditoria BIGINT IDENTITY(1,1) NOT NULL,
    IdTarea INT NOT NULL,
    IdEstatusAnterior INT NOT NULL,
    IdEstatusNuevo INT NOT NULL,
    FechaCambio DATETIME2(0) NOT NULL
        CONSTRAINT DF_AuditoriaEstatusTarea_FechaCambio
        DEFAULT (SYSUTCDATETIME()),
    UsuarioBaseDatos NVARCHAR(128) NOT NULL
        CONSTRAINT DF_AuditoriaEstatusTarea_UsuarioBaseDatos
        DEFAULT (SUSER_SNAME()),

    CONSTRAINT PK_AuditoriaEstatusTarea
        PRIMARY KEY CLUSTERED (IdAuditoria),

    CONSTRAINT FK_AuditoriaEstatusTarea_Tareas
        FOREIGN KEY (IdTarea)
        REFERENCES dbo.Tareas(IdTarea),

    CONSTRAINT FK_AuditoriaEstatusTarea_EstatusAnterior
        FOREIGN KEY (IdEstatusAnterior)
        REFERENCES dbo.CatEstatus(IdEstatus),

    CONSTRAINT FK_AuditoriaEstatusTarea_EstatusNuevo
        FOREIGN KEY (IdEstatusNuevo)
        REFERENCES dbo.CatEstatus(IdEstatus),

    CONSTRAINT CK_AuditoriaEstatusTarea_CambioReal
        CHECK (IdEstatusAnterior <> IdEstatusNuevo)
);
GO

/*=============================================================================
  3. DATOS INICIALES
=============================================================================*/

BEGIN TRY
    BEGIN TRANSACTION;

    /* Catálogo de estatus */
    SET IDENTITY_INSERT dbo.CatEstatus ON;

    INSERT INTO dbo.CatEstatus (IdEstatus, Nombre)
    VALUES
        (1, N'Pendiente'),
        (2, N'En Progreso'),
        (3, N'Terminada');

    SET IDENTITY_INSERT dbo.CatEstatus OFF;

    /* Catálogo de prioridades */
    SET IDENTITY_INSERT dbo.CatPrioridades ON;

    INSERT INTO dbo.CatPrioridades (IdPrioridad, Nombre)
    VALUES
        (1, N'Alta'),
        (2, N'Media'),
        (3, N'Baja');

    SET IDENTITY_INSERT dbo.CatPrioridades OFF;

    /* Usuarios de prueba.
       IMPORTANTE: las contraseñas están en texto plano solo para fines de examen. */
    SET IDENTITY_INSERT dbo.Usuarios ON;

    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (1, N'Juan Pérez', N'juan.perez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (2, N'María López', N'maria.lopez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (3, N'Carlos Hernández', N'carlos.hernandez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (4, N'Ana González', N'ana.gonzalez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (5, N'Luis Ramírez', N'luis.ramirez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (6, N'Fernanda Torres', N'fernanda.torres@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (7, N'Miguel Sánchez', N'miguel.sanchez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (8, N'Sofía Martínez', N'sofia.martinez@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (9, N'Jorge Castillo', N'jorge.castillo@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')
    INSERT dbo.Usuarios ([IdUsuario], [NombreCompleto], [CorreoElectronico], [Activo], [FechaCreacion], [Password]) VALUES (10, N'Valeria Navarro', N'valeria.navarro@empresa.com', 1, CAST(N'2026-07-16T10:15:55.0000000' AS DateTime2), N'123456')

    SET IDENTITY_INSERT dbo.Usuarios OFF;

    /* Tareas de prueba */
    SET IDENTITY_INSERT dbo.Tareas ON;

    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (2, N'Crear API de usuarios', N'Implementar los endpoints para administrar usuarios.', 1, CAST(N'2026-07-08T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-09' AS Date), NULL, CAST(N'2026-07-21' AS Date), 1, 1)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (18, N'Agregar manejo global de excepciones', N'Crear middleware para centralizar errores de la API.', 1, CAST(N'2026-07-14T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-26' AS Date), 1, 6)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (25, N'Crear pruebas para el servicio de tareas', N'Agregar pruebas unitarias para creación y actualización.', 1, CAST(N'2026-07-13T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-24' AS Date), 1, 9)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (3, N'Documentar módulo de seguridad', N'Crear la documentación técnica del módulo de autenticación.', 2, CAST(N'2026-07-11T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-24' AS Date), 1, 1)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (5, N'Crear servicio de recuperación de contraseña', N'Implementar la recuperación de contraseña por correo.', 2, CAST(N'2026-07-09T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-22' AS Date), 1, 2)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (9, N'Implementar paginación', N'Agregar paginación al endpoint de consulta de tareas.', 2, CAST(N'2026-07-12T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-23' AS Date), 1, 3)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (15, N'Crear consulta de auditoría', N'Crear una consulta para visualizar el historial de cambios.', 2, CAST(N'2026-07-13T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-25' AS Date), 1, 5)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (26, N'Probar filtros de búsqueda', N'Validar filtros individuales y combinados.', 2, CAST(N'2026-07-14T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-25' AS Date), 1, 9)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (12, N'Exportar reporte a Excel', N'Agregar una opción para exportar el reporte en formato Excel.', 3, CAST(N'2026-07-13T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-28' AS Date), 1, 4)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (21, N'Optimizar carga de catálogos', N'Cargar prioridades, estatus y usuarios en paralelo.', 3, CAST(N'2026-07-15T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-27' AS Date), 1, 7)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (29, N'Crear archivo README', N'Agregar instrucciones para ejecutar la solución.', 3, CAST(N'2026-07-14T10:21:12.5070000' AS DateTime2), NULL, NULL, CAST(N'2026-07-26' AS Date), 1, 10)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (1, N'Diseñar pantalla de inicio de sesión', N'Crear la interfaz responsive para el inicio de sesión.', 1, CAST(N'2026-07-06T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-07' AS Date), NULL, CAST(N'2026-07-19' AS Date), 2, 1)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (4, N'Implementar autenticación JWT', N'Generar y validar tokens JWT desde la Web API.', 1, CAST(N'2026-07-04T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-05' AS Date), NULL, CAST(N'2026-07-14' AS Date), 2, 2)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (7, N'Crear CRUD de tareas', N'Implementar alta, consulta, actualización y eliminación de tareas.', 1, CAST(N'2026-07-07T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-08' AS Date), NULL, CAST(N'2026-07-18' AS Date), 2, 3)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (14, N'Implementar trigger de auditoría', N'Registrar automáticamente los cambios de estatus de las tareas.', 1, CAST(N'2026-07-07T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-08' AS Date), NULL, CAST(N'2026-07-15' AS Date), 2, 5)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (30, N'Preparar entrega del examen', N'Revisar base de datos, API, frontend y documentación.', 1, CAST(N'2026-07-16T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-16' AS Date), NULL, CAST(N'2026-07-20' AS Date), 2, 10)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (8, N'Agregar filtros al listado', N'Agregar filtros por prioridad, estatus, usuario y fechas.', 2, CAST(N'2026-07-10T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-11' AS Date), NULL, CAST(N'2026-07-20' AS Date), 2, 3)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (10, N'Diseñar módulo de reportes', N'Crear la pantalla del reporte de tareas pendientes.', 2, CAST(N'2026-07-08T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-09' AS Date), NULL, CAST(N'2026-07-21' AS Date), 2, 4)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (17, N'Configurar Swagger', N'Documentar y probar los endpoints de la API.', 2, CAST(N'2026-07-09T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-10' AS Date), NULL, CAST(N'2026-07-17' AS Date), 2, 6)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (20, N'Agregar validaciones de formularios', N'Validar los campos requeridos antes de consumir la API.', 2, CAST(N'2026-07-10T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-11' AS Date), NULL, CAST(N'2026-07-19' AS Date), 2, 7)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (22, N'Diseñar estilos del sistema', N'Crear estilos modernos para controles, tablas y tarjetas.', 2, CAST(N'2026-07-08T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-09' AS Date), NULL, CAST(N'2026-07-15' AS Date), 2, 8)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (23, N'Agregar diseño responsive', N'Optimizar las pantallas para dispositivos móviles.', 2, CAST(N'2026-07-11T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-12' AS Date), NULL, CAST(N'2026-07-22' AS Date), 2, 8)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (28, N'Preparar documentación técnica', N'Documentar arquitectura, endpoints y configuración.', 2, CAST(N'2026-07-12T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-13' AS Date), NULL, CAST(N'2026-07-23' AS Date), 2, 10)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (6, N'Corregir validación de login', N'Corregir el mensaje mostrado cuando las credenciales son incorrectas.', 1, CAST(N'2026-07-01T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-02' AS Date), CAST(N'2026-07-04' AS Date), CAST(N'2026-07-04' AS Date), 3, 2)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (11, N'Crear procedimiento de tareas pendientes', N'Crear el procedimiento almacenado sp_GetPendingTasks.', 1, CAST(N'2026-07-05T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-06' AS Date), CAST(N'2026-07-08' AS Date), CAST(N'2026-07-08' AS Date), 3, 4)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (13, N'Crear tabla de auditoría', N'Crear la tabla para registrar cambios de estatus.', 1, CAST(N'2026-07-03T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-04' AS Date), CAST(N'2026-07-06' AS Date), CAST(N'2026-07-06' AS Date), 3, 5)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (16, N'Configurar CORS', N'Permitir las solicitudes realizadas desde la aplicación MVC.', 1, CAST(N'2026-07-02T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-03' AS Date), CAST(N'2026-07-03' AS Date), CAST(N'2026-07-03' AS Date), 3, 6)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (19, N'Crear cliente HTTP reutilizable', N'Centralizar las peticiones GET, POST, PUT y DELETE.', 1, CAST(N'2026-07-06T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-07' AS Date), CAST(N'2026-07-09' AS Date), CAST(N'2026-07-09' AS Date), 3, 7)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (27, N'Probar procedimiento almacenado', N'Validar pendientes y vencidas por usuario.', 2, CAST(N'2026-07-10T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-11' AS Date), CAST(N'2026-07-12' AS Date), CAST(N'2026-07-12' AS Date), 3, 9)
    INSERT dbo.Tareas ([IdTarea], [Titulo], [Descripcion], [IdPrioridad], [FechaCreacion], [FechaInicio], [FechaFinalizacion], [FechaLimite], [IdEstatus], [IdUsuarioResponsable]) VALUES (24, N'Agregar iconos de Font Awesome', N'Agregar iconos a botones, menús y estados.', 3, CAST(N'2026-07-12T10:21:12.5070000' AS DateTime2), CAST(N'2026-07-13' AS Date), CAST(N'2026-07-14' AS Date), CAST(N'2026-07-14' AS Date), 3, 8)

    SET IDENTITY_INSERT dbo.Tareas OFF;

    COMMIT TRANSACTION;
    PRINT N'Datos iniciales insertados correctamente.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    IF OBJECTPROPERTY(OBJECT_ID(N'dbo.CatEstatus'), 'TableHasIdentity') = 1
        SET IDENTITY_INSERT dbo.CatEstatus OFF;

    THROW;
END CATCH;
GO

/*=============================================================================
  4. ÍNDICES
=============================================================================*/

/* Evita títulos duplicados para un mismo usuario responsable. */
CREATE UNIQUE NONCLUSTERED INDEX UX_Tareas_UsuarioResponsable_Titulo
ON dbo.Tareas
(
    IdUsuarioResponsable,
    Titulo
);
GO

/* Optimiza filtros y reporte por usuario, estatus y fecha límite. */
CREATE NONCLUSTERED INDEX IX_Tareas_Usuario_Estatus_FechaLimite
ON dbo.Tareas
(
    IdUsuarioResponsable,
    IdEstatus,
    FechaLimite
)
INCLUDE
(
    Titulo,
    IdPrioridad,
    FechaCreacion
);
GO

/* Optimiza consultas por estatus y vencimiento. */
CREATE NONCLUSTERED INDEX IX_Tareas_Estatus_FechaLimite
ON dbo.Tareas
(
    IdEstatus,
    FechaLimite
)
INCLUDE
(
    IdUsuarioResponsable,
    IdPrioridad,
    Titulo
);
GO

/* Optimiza listado y paginación ordenados por fecha de creación. */
CREATE NONCLUSTERED INDEX IX_Tareas_FechaCreacion
ON dbo.Tareas
(
    FechaCreacion DESC
)
INCLUDE
(
    Titulo,
    IdPrioridad,
    IdEstatus,
    IdUsuarioResponsable,
    FechaLimite
);
GO

/* Optimiza la consulta del historial de una tarea. */
CREATE NONCLUSTERED INDEX IX_AuditoriaEstatusTarea_IdTarea_FechaCambio
ON dbo.AuditoriaEstatusTarea
(
    IdTarea,
    FechaCambio DESC
)
INCLUDE
(
    IdEstatusAnterior,
    IdEstatusNuevo,
    UsuarioBaseDatos
);
GO

/*=============================================================================
  5. STORED PROCEDURE: REPORTE DE PENDIENTES Y VENCIDAS
=============================================================================*/

IF OBJECT_ID(N'dbo.sp_GetPendingTasks', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetPendingTasks;
GO

CREATE PROCEDURE dbo.sp_GetPendingTasks
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdPendiente INT;
    DECLARE @IdTerminada INT;

    SELECT @IdPendiente = IdEstatus
    FROM dbo.CatEstatus
    WHERE Nombre = N'Pendiente';

    SELECT @IdTerminada = IdEstatus
    FROM dbo.CatEstatus
    WHERE Nombre = N'Terminada';

    IF @IdPendiente IS NULL OR @IdTerminada IS NULL
    BEGIN
        THROW 50002,
              'No se encontraron los estatus Pendiente y Terminada.',
              1;
    END;

    SELECT
        u.IdUsuario,
        u.NombreCompleto AS Usuario,
        SUM
        (
            CASE
                WHEN t.IdEstatus = @IdPendiente THEN 1
                ELSE 0
            END
        ) AS TotalPendientes,
        SUM
        (
            CASE
                WHEN t.IdEstatus <> @IdTerminada
                     AND t.FechaLimite < CONVERT(DATE, GETDATE()) THEN 1
                ELSE 0
            END
        ) AS TotalVencidas
    FROM dbo.Usuarios AS u
    LEFT JOIN dbo.Tareas AS t
        ON t.IdUsuarioResponsable = u.IdUsuario
    WHERE u.Activo = 1
    GROUP BY
        u.IdUsuario,
        u.NombreCompleto
    ORDER BY
        u.NombreCompleto;
END;
GO

/*=============================================================================
  6. TRIGGER: AUDITORÍA DE CAMBIOS DE ESTATUS
=============================================================================*/

IF OBJECT_ID(N'dbo.trg_Tareas_AuditoriaEstatus', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Tareas_AuditoriaEstatus;
GO

CREATE TRIGGER dbo.trg_Tareas_AuditoriaEstatus
ON dbo.Tareas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.AuditoriaEstatusTarea
    (
        IdTarea,
        IdEstatusAnterior,
        IdEstatusNuevo,
        FechaCambio,
        UsuarioBaseDatos
    )
    SELECT
        i.IdTarea,
        d.IdEstatus,
        i.IdEstatus,
        SYSUTCDATETIME(),
        SUSER_SNAME()
    FROM inserted AS i
    INNER JOIN deleted AS d
        ON d.IdTarea = i.IdTarea
    WHERE i.IdEstatus <> d.IdEstatus;
END;
GO

/*=============================================================================
  7. VALIDACIONES FINALES
=============================================================================*/

DECLARE @TotalEstatus INT = (SELECT COUNT(*) FROM dbo.CatEstatus);
DECLARE @TotalPrioridades INT = (SELECT COUNT(*) FROM dbo.CatPrioridades);
DECLARE @TotalUsuarios INT = (SELECT COUNT(*) FROM dbo.Usuarios);
DECLARE @TotalTareas INT = (SELECT COUNT(*) FROM dbo.Tareas);

IF @TotalEstatus <> 3
    THROW 50003, 'La instalación no creó correctamente los estatus.', 1;

IF @TotalPrioridades <> 3
    THROW 50004, 'La instalación no creó correctamente las prioridades.', 1;

IF @TotalUsuarios <> 10
    THROW 50005, 'La instalación no creó correctamente los usuarios.', 1;

IF @TotalTareas <> 30
    THROW 50006, 'La instalación no creó correctamente las tareas.', 1;

IF OBJECT_ID(N'dbo.sp_GetPendingTasks', N'P') IS NULL
    THROW 50007, 'No se creó el stored procedure sp_GetPendingTasks.', 1;

IF OBJECT_ID(N'dbo.trg_Tareas_AuditoriaEstatus', N'TR') IS NULL
    THROW 50008, 'No se creó el trigger de auditoría.', 1;

PRINT N'==============================================================';
PRINT N'Instalación completada correctamente.';
PRINT N'Estatus: ' + CONVERT(NVARCHAR(10), @TotalEstatus);
PRINT N'Prioridades: ' + CONVERT(NVARCHAR(10), @TotalPrioridades);
PRINT N'Usuarios: ' + CONVERT(NVARCHAR(10), @TotalUsuarios);
PRINT N'Tareas: ' + CONVERT(NVARCHAR(10), @TotalTareas);
PRINT N'==============================================================';
GO

/* Consulta opcional de verificación del reporte. */
EXEC dbo.sp_GetPendingTasks;
GO
