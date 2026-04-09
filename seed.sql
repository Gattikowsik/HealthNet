
use HealthNetDataBase
select * from LabTest
select * from Cases;
-- Check if you have any users
SELECT * FROM Users
SELECT * FROM Role

select * from Outbreak;
insert into Outbreak values ( 'Influenza', 'New York', '2024-01-01', '2024-02-28', 'active', 1)
insert into Outbreak values ( 'COVID-19', 'California', '2024-03-01', '2024-04-30', 'resolved', 0)
insert into Outbreak values ( 'Norovirus', 'Florida', '2024-05-01', '2024-06-30', 'active', 1)
insert into Outbreak values ( 'Ebola', 'Texas', '2024-07-01', '2024-08-31', 'resolved', 0)
insert into Outbreak values ( 'Zika Virus', 'Florida', '2024-09-01', '2024-10-31', 'active', 1)
DELETE FROM Outbreak WHERE OutbreakID = 1

delete from ComplianceRecord where EntityId = 1
select * from ComplianceRecord

INSERT INTO Action (ActionName) VALUES('Login')
INSERT INTO Action (ActionName) VALUES('Create')
INSERT INTO Action (ActionName) VALUES('Read')
INSERT INTO Action (ActionName) VALUES('Update')
INSERT INTO Action (ActionName) VALUES('Delete')
 
select * from AuditLog   
delete from AuditLog where UserId = 1 


SELECT * FROM Role