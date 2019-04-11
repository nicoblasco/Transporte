USE [Transporte]
GO

/****** Object:  Table [dbo].[Audits]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Audits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[WindowId] [int] NOT NULL,
	[Accion] [nvarchar](max) NULL,
	[Fecha] [datetime] NOT NULL,
	[Clave] [nvarchar](max) NULL,
	[Entidad] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Audits] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Countries]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Predeterminado] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Fields]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Fields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Referencia] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[IsArray] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Fields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Modules]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Modules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Modules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Notifications]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Notifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Documento] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Notifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[NotificationTags]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NotificationTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NombreAtributo] [nvarchar](max) NULL,
	[Tag] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.NotificationTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[People]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[People](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransportId] [int] NOT NULL,
	[PersonTypeId] [int] NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Apellido] [nvarchar](max) NULL,
	[Dni] [nvarchar](max) NULL,
	[CalleReal] [nvarchar](max) NULL,
	[CalleConstituido] [nvarchar](max) NULL,
	[Telefono] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[VtoLicencia] [datetime] NULL,
	[VtoLibreta] [datetime] NULL,
	[Enable] [bit] NOT NULL,
	[PartidoReal] [nvarchar](max) NULL,
	[PartidoConstituido] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.People] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Permissions]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Permissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WindowId] [int] NOT NULL,
	[RolId] [int] NOT NULL,
	[Consulta] [bit] NOT NULL,
	[AltaModificacion] [bit] NOT NULL,
	[Baja] [bit] NOT NULL,
	[Fecha] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Permissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[PersonTypes]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PersonTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.PersonTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Rols]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Rols](
	[RolId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[IsAdmin] [bit] NOT NULL,
	[WindowId] [int] NULL,
 CONSTRAINT [PK_dbo.Rols] PRIMARY KEY CLUSTERED 
(
	[RolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Settings]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Settings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [nvarchar](max) NULL,
	[Texto1] [nvarchar](max) NULL,
	[Texto2] [nvarchar](max) NULL,
	[Numero1] [int] NULL,
	[Numero2] [int] NULL,
	[Logico1] [bit] NULL,
	[Logico2] [bit] NULL,
	[Fecha1] [datetime] NULL,
	[Fecha2] [datetime] NULL,
 CONSTRAINT [PK_dbo.Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Streets]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Streets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Baja] [nvarchar](max) NULL,
	[DescripcionOficial] [nvarchar](max) NULL,
	[Codigo] [nvarchar](max) NULL,
	[DescripcionGoogle] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Streets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Transports]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransportTypeId] [int] NOT NULL,
	[Expediente] [nvarchar](max) NULL,
	[Dominio] [nvarchar](max) NULL,
	[Observaciones] [nvarchar](max) NULL,
	[VtoPoliza] [datetime] NULL,
	[ReciboPagoSeguro] [nvarchar](max) NULL,
	[VtoVTV] [datetime] NULL,
	[VtoMatafuego] [datetime] NULL,
	[VtoConstanciaAFIP] [datetime] NULL,
	[Marca] [nvarchar](max) NULL,
	[Modelo] [nvarchar](max) NULL,
	[FechaInscripcionInicial] [datetime] NULL,
	[Desinfeccion] [datetime] NULL,
	[Constatacion] [datetime] NULL,
	[FechaAlta] [datetime] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[Enable] [bit] NOT NULL,
	[ParadaNro] [nvarchar](max) NULL,
	[PlazaNro] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Transports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[TransportTypes]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TransportTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TransportTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Usuarios]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[UsuarioId] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[Apellido] [nvarchar](max) NULL,
	[Nombreusuario] [nvarchar](max) NULL,
	[Contraseña] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
	[RolId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Usuarios] PRIMARY KEY CLUSTERED 
(
	[UsuarioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Windows]    Script Date: 29/03/2019 12:38:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Windows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Enable] [bit] NOT NULL,
	[Url] [nvarchar](max) NULL,
	[Orden] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Windows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[People] ADD  DEFAULT ((0)) FOR [Enable]
GO

ALTER TABLE [dbo].[Transports] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [FechaAlta]
GO

ALTER TABLE [dbo].[Transports] ADD  DEFAULT ((0)) FOR [UsuarioId]
GO

ALTER TABLE [dbo].[Transports] ADD  DEFAULT ((0)) FOR [Enable]
GO

ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Audits_dbo.Usuarios_UsuarioId] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([UsuarioId])
GO

ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_dbo.Audits_dbo.Usuarios_UsuarioId]
GO

ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Audits_dbo.Windows_WindowId] FOREIGN KEY([WindowId])
REFERENCES [dbo].[Windows] ([Id])
GO

ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_dbo.Audits_dbo.Windows_WindowId]
GO

ALTER TABLE [dbo].[People]  WITH CHECK ADD  CONSTRAINT [FK_dbo.People_dbo.PersonTypes_PersonTypeId] FOREIGN KEY([PersonTypeId])
REFERENCES [dbo].[PersonTypes] ([Id])
GO

ALTER TABLE [dbo].[People] CHECK CONSTRAINT [FK_dbo.People_dbo.PersonTypes_PersonTypeId]
GO

ALTER TABLE [dbo].[People]  WITH CHECK ADD  CONSTRAINT [FK_dbo.People_dbo.Transports_TransportId] FOREIGN KEY([TransportId])
REFERENCES [dbo].[Transports] ([Id])
GO

ALTER TABLE [dbo].[People] CHECK CONSTRAINT [FK_dbo.People_dbo.Transports_TransportId]
GO

ALTER TABLE [dbo].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Permissions_dbo.Rols_RolId] FOREIGN KEY([RolId])
REFERENCES [dbo].[Rols] ([RolId])
GO

ALTER TABLE [dbo].[Permissions] CHECK CONSTRAINT [FK_dbo.Permissions_dbo.Rols_RolId]
GO

ALTER TABLE [dbo].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Permissions_dbo.Windows_WindowId] FOREIGN KEY([WindowId])
REFERENCES [dbo].[Windows] ([Id])
GO

ALTER TABLE [dbo].[Permissions] CHECK CONSTRAINT [FK_dbo.Permissions_dbo.Windows_WindowId]
GO

ALTER TABLE [dbo].[Rols]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Rols_dbo.Windows_WindowId] FOREIGN KEY([WindowId])
REFERENCES [dbo].[Windows] ([Id])
GO

ALTER TABLE [dbo].[Rols] CHECK CONSTRAINT [FK_dbo.Rols_dbo.Windows_WindowId]
GO

ALTER TABLE [dbo].[Transports]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transports_dbo.TransportTypes_TransportTypeId] FOREIGN KEY([TransportTypeId])
REFERENCES [dbo].[TransportTypes] ([Id])
GO

ALTER TABLE [dbo].[Transports] CHECK CONSTRAINT [FK_dbo.Transports_dbo.TransportTypes_TransportTypeId]
GO

ALTER TABLE [dbo].[Transports]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transports_dbo.Usuarios_UsuarioId] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([UsuarioId])
GO

ALTER TABLE [dbo].[Transports] CHECK CONSTRAINT [FK_dbo.Transports_dbo.Usuarios_UsuarioId]
GO

ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Usuarios_dbo.Rols_RolId] FOREIGN KEY([RolId])
REFERENCES [dbo].[Rols] ([RolId])
GO

ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_dbo.Usuarios_dbo.Rols_RolId]
GO

ALTER TABLE [dbo].[Windows]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Windows_dbo.Modules_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([Id])
GO

ALTER TABLE [dbo].[Windows] CHECK CONSTRAINT [FK_dbo.Windows_dbo.Modules_ModuleId]
GO

