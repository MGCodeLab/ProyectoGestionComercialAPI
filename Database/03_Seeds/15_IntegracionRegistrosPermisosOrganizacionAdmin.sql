insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'organization', 'create', 'Crear Organizaciones', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'organization', 'edit', 'Editar Organizaciones', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'organization', 'delete', 'Eliminar Organizaciones', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'organization', 'read', 'Leer Organizaciones', 1, '2026-05-24T01:57:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())