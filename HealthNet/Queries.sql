INSERT INTO Role (RoleName) VALUES ('Citizen')
INSERT INTO Role (RoleName) VALUES ('Doctor')
INSERT INTO Role (RoleName) VALUES ('Lab Technician')
INSERT INTO Role (RoleName) VALUES ('Public Health Officer')
INSERT INTO Role (RoleName) VALUES ('Researcher')
INSERT INTO Role (RoleName) VALUES ('Admin')
INSERT INTO Role (RoleName) VALUES ('Compliance Officer')
 
INSERT INTO Users ([Name],RoleId,Email,Phone,[Status],[Password]) VALUES('Pranay',6,'patelpranay577@gmail.com','9398285008',1,'$2a$12$ucyXgkEu9Po4zqEsvh7.juCDJKAHck9ccOVaQkRBDeOxFjCgmG3Uq')
INSERT INTO Users ([Name],RoleId,Email,Phone,[Status],[Password]) VALUES('Test',6,'Pranay@gmail.com','9581888812',1,'$2a$12$ucyXgkEu9Po4zqEsvh7.juCDJKAHck9ccOVaQkRBDeOxFjCgmG3Uq')

SELECT * FROM Users