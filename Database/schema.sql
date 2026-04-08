IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Bookings] (
    [BookingID] int NOT NULL,
    [CarID] int NOT NULL,
    [CustomerName] nvarchar(max) NULL,
    [CustomerEmail] nvarchar(max) NULL,
    [CustomerPhone] nvarchar(max) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Booked] bit NOT NULL,
    [TotalCost] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingID])
);

CREATE TABLE [Cars] (
    [Id] int NOT NULL,
    [Make] nvarchar(max) NULL,
    [Model] nvarchar(max) NULL,
    [Registration] nvarchar(max) NULL,
    [PricePerDay] decimal(18,2) NOT NULL,
    [Seats] int NOT NULL,
    [IsAvailable] bit NOT NULL,
    CONSTRAINT [PK_Cars] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260327100356_InitialCreate', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260327202247_AddIdentity', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Cars] ADD [PhotoUrl] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260328220130_AddCarPhotoUrl', N'10.0.5');

COMMIT;
GO

-- ============================================================
-- SEED DATA — 20 cars inserted by application on first startup
-- (Program.cs seeds these if the Cars table is empty)
-- ============================================================
INSERT INTO [Cars] ([Id], [Make], [Model], [Registration], [PricePerDay], [Seats], [IsAvailable], [PhotoUrl]) VALUES
(1,  'Toyota',     'Corolla',    'AB12CDE', 35.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Toyota+Corolla'),
(2,  'BMW',        'X5',         'XY99ZZZ', 95.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=BMW+X5'),
(3,  'Ford',       'Fiesta',     'FD21ABC', 28.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Ford+Fiesta'),
(4,  'Mercedes',   'C-Class',    'MC55DEF', 120.00, 5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Mercedes+C-Class'),
(5,  'Vauxhall',   'Astra',      'VA33GHI', 45.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Vauxhall+Astra'),
(6,  'Porsche',    'Taycan',     'PO45CH3', 75.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Porsche+Taycan'),
(7,  'Ford',       'Raptor',     'RA970R',  45.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Ford+Raptor'),
(8,  'Land Rover', 'Defender',   'M0N37',   150.00, 7, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Land+Rover+Defender'),
(9,  'Volkswagen', 'Golf',       'B4NG37',  20.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Volkswagen+Golf'),
(10, 'MiniCooper', 'Countryman', 'QW09OP',  30.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=MiniCooper+Countryman'),
(11, 'Audi',       'A3',         'AU11DI3', 55.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Audi+A3'),
(12, 'Nissan',     'Qashqai',    'NI55SAN', 60.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Nissan+Qashqai'),
(13, 'Honda',      'Civic',      'HO13NDA', 40.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Honda+Civic'),
(14, 'Tesla',      'Model 3',    'TE14SLA', 85.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Tesla+Model+3'),
(15, 'Hyundai',    'Tucson',     'HY15UND', 50.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Hyundai+Tucson'),
(16, 'Kia',        'Sportage',   'KI16ASP', 48.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Kia+Sportage'),
(17, 'Jeep',       'Wrangler',   'JE17EPW', 80.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Jeep+Wrangler'),
(18, 'Volvo',      'XC40',       'VO18LVO', 70.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Volvo+XC40'),
(19, 'Skoda',      'Octavia',    'SK19ODA', 32.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Skoda+Octavia'),
(20, 'Renault',    'Zoe',        'RE20NAU', 25.00,  5, 1, 'https://placehold.co/600x260/0f172a/ffffff?text=Renault+Zoe');
GO

