
insert into [dbo].[Modules] (Descripcion,Enable) values ('Menu Principal',1);
insert into [dbo].[Modules] (Descripcion,Enable) values ('ABM Maestros',1);
insert into [dbo].[Modules] (Descripcion,Enable) values ('Configuración',1);

insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Transportes',1,'Transports/Index',1,(select Id from [dbo].[Modules] where Descripcion='Menu Principal'))


insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Tipos de Transporte',1,'TransportTypes/Index',1,(select Id from [dbo].[Modules] where Descripcion='ABM Maestros'))


insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Roles',1,'Rols/Index',1,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Usuarios',1,'Usuarios/Index',2,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Permisos',1,'Permissions/Index',3,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Auditoria',1,'Audits/Index',4,(select Id from [dbo].[Modules] where Descripcion='Configuración'))
insert into [dbo].[Windows] (Descripcion,Enable,Url,Orden,ModuleId) values ('Sistema',1,'Settings/Edit',5,(select Id from [dbo].[Modules] where Descripcion='Configuración'))


update Rols
set WindowId= (select Id from Windows where Descripcion= 'Usuarios')
