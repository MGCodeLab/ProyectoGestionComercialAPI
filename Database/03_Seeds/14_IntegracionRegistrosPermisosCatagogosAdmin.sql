insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'catalogs', 'create', 'Crear Catalogos', 1, '2026-05-20T22:37:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'catalogs', 'edit', 'Editar Catalogos', 1, '2026-05-20T22:37:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'catalogs', 'delete', 'Eliminar Catalogos', 1, '2026-05-20T22:37:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())
go
insert into [seguridad].[Permisos] ([PublicId], [Recurso], [Accion], [Descripcion], [Activo], [FechaRegistro], [FechaActualizacion])
values 
(UPPER(NEWID()), 'catalogs', 'read', 'Leer Catalogos', 1, '2026-05-20T22:37:50.6000000', null)

insert into [seguridad].[RolPermisos]
values (1, SCOPE_IDENTITY())