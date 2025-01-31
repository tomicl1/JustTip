-- Clear existing data
DELETE FROM TipAllocations;
DELETE FROM Shifts;
DELETE FROM Employees;
DELETE FROM Businesses;
GO

-- Reset identity columns
DBCC CHECKIDENT ('TipAllocations', RESEED, 0);
DBCC CHECKIDENT ('Shifts', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);
DBCC CHECKIDENT ('Businesses', RESEED, 0);
GO

-- Insert Businesses
INSERT INTO Businesses (Name) VALUES
('Restaurant A'),  -- Will be ID: 1
('Cafe B'),        -- Will be ID: 2
('Bar C');         -- Will be ID: 3
GO

-- Insert Employees
INSERT INTO Employees (BusinessId, Name, Email, Position, HireDate, Phone) VALUES
-- Restaurant A employees
(1, 'John Smith', 'john@example.com', 'Server', '2024-01-01', '555-0101'),       -- Will be ID: 1 
(1, 'Jane Doe', 'jane@example.com', 'Bartender', '2024-06-15', '555-0102'),      -- Will be ID: 2
(1, 'Bob Wilson', 'bob@example.com', 'Server', '2025-01-01', '555-0103'),        -- Will be ID: 3

-- Cafe B employees
(2, 'Alice Brown', 'alice@example.com', 'Barista', '2024-03-01', '555-0201'),    -- Will be ID: 4
(2, 'Charlie Davis', 'charlie@example.com', 'Server', '2024-07-01', '555-0202'),  -- Will be ID: 5

-- Bar C employees
(3, 'Eve Wilson', 'eve@example.com', 'Bartender', '2024-02-01', '555-0301'),     -- Will be ID: 6
(3, 'Frank Miller', 'frank@example.com', 'Server', '2024-08-01', '555-0302');    -- Will be ID: 7
GO

-- Insert Shifts 
INSERT INTO Shifts (EmployeeId, Year, Month, WorkedDays) VALUES
-- Restaurant A shifts
(1, 2025, 2, '5,8,9,10,11,12,15,16,17,18,19,22,23,24,25,26'),     
(2, 2025, 2, '4,5,8,9,10,11,12,15,16,17,18,19'),                      
(3, 2025, 2, '17,18,19,22,23,24,25,26'),                             

-- Cafe B shifts 
(4, 2025, 2, '1,3,5,8,10,12,15,17,19,22,24,26'),
(4, 2025, 3, '1,3,5,8,22,24,26'),                          
(5, 2025, 2, '6,7,13,14,20,21,27,28'),
(5, 2025, 3, '1,2,3,4,5,8,9,10,11,12,15,16,17,18,19,22,23,24,25,26,29,30,31'),                                   

-- Bar C shifts
(7, 2025, 2, '6,7,13,14,20,21,27,28'), 
(7, 2025, 3, '1,2,3,4,5,8,9,10,11,12,15,16,17,18,19,22,23,24,25,26,29,30,31');                                  


-- Insert Tip Allocations (dummy data for home screen grid)
INSERT INTO TipAllocations (EmployeeId, Amount, DistributionDate, PeriodStart, PeriodEnd, DaysWorked) VALUES
-- Cafe B
(4, 250.00, '2025-01-15', '2025-01-01', '2025-01-14', 6),
(5, 200.00, '2025-01-15', '2025-01-01', '2025-01-14', 4);

GO 