insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'comercial', 'create', 'Crear Comercial', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'comercial', 'edit', 'Editar Comercial', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'comercial', 'delete', 'Eliminar Comercial', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'comercial', 'read', 'Leer Comerciales', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())