SELECT * FROM Role
INSERT INTO Role (RoleName) VALUES ('Citizen')
INSERT INTO Role (RoleName) VALUES ('Doctor')
INSERT INTO Role (RoleName) VALUES ('Lab Technician')
INSERT INTO Role (RoleName) VALUES ('Public Health Officer')
INSERT INTO Role (RoleName) VALUES ('Researcher')
INSERT INTO Role (RoleName) VALUES ('Admin')
INSERT INTO Role (RoleName) VALUES ('Compliance Officer')
 
 use HealthNetDataBase
 SELECT * FROM Users
 INSERT INTO Users ([Name],RoleId,Email,Phone,[Status],[Password]) VALUES('Pranay',6,'patelpranay577@gmail.com','9398285008',1,'$2a$12$ucyXgkEu9Po4zqEsvh7.juCDJKAHck9ccOVaQkRBDeOxFjCgmG3Uq')
INSERT INTO Users ([Name],RoleId,Email,Phone,[Status],[Password]) VALUES('Sarthak',7,'sarthak@gmail.com','9398285008',1,'$2a$12$ucyXgkEu9Po4zqEsvh7.juCDJKAHck9ccOVaQkRBDeOxFjCgmG3Uq')
INSERT INTO Users ([Name],RoleId,Email,Phone,[Status],[Password]) VALUES('Manju',5,'Manju@gmail.com','9398285008',1,'$2a$12$ucyXgkEu9Po4zqEsvh7.juCDJKAHck9ccOVaQkRBDeOxFjCgmG3Uq')
