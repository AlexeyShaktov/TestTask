CREATE TABLE Candidate (
  idCandidate INTEGER NOT NULL IDENTITY(1,1),
  PhoneNumber VARCHAR(45) NOT NULL,
  Position VARCHAR(255) NOT NULL,
  Name VARCHAR(255) NULL,
  PRIMARY KEY(idCandidate)
);

CREATE TABLE Director (
  idDirector INTEGER  NOT NULL IDENTITY(1,1),
  Name VARCHAR(255) NULL,
  PRIMARY KEY(idDirector)
);

CREATE TABLE Employee (
  idEmployee INTEGER  NOT NULL IDENTITY(1,1),
  Name VARCHAR(255) NOT NULL,
  Position VARCHAR(255) NOT NULL,
  PRIMARY KEY(idEmployee)
);

CREATE TABLE Stat (
  idStat INTEGER  NOT NULL IDENTITY(1,1),
  State VARCHAR(255) NULL,
  PRIMARY KEY(idStat)
);

CREATE TABLE Task (
  idTask INTEGER  NOT NULL IDENTITY(1,1),
  Director_idDirector INTEGER  NOT NULL,
  Stat_idStat INTEGER  NOT NULL,
  Employee_idEmployee INTEGER  NOT NULL,
  Candidate_idCandidate INTEGER NOT NULL,
  IssueDate DATE NULL,
  Period INTEGER  NULL,
  VerificationDate DATETIME NULL,
  Score CHAR NULL,
  PRIMARY KEY(idTask),
  INDEX Task_FKIndex1(Candidate_idCandidate),
  INDEX Task_FKIndex2(Employee_idEmployee),
  INDEX Task_FKIndex3(Stat_idStat),
  INDEX Task_FKIndex4(Director_idDirector)
);

insert into Candidate Values
	('89507127723','�������� �� ��������','������� ����� ���������'), 
	('89617122613','�����','������ ������� ����������'), 
	('89527772691','���������� �� ������� ������������','��������� ����� ����������'),
	('89528239976','�������� �� ��������','���������� ������ ��������')

insert into Employee Values
	('�������','���������'), 
	('�������','�����'), 
	('�����','�������� �� ��������')

insert into Stat Values 
	('������� ��������'),
	('������� ����� �� ��������'),
	('������� ���������� ������ �����������'),
	('������� ����� ���������� �������')

insert into Director Values
	('�������'),
	('��������'),
	('����������')


-- �������� ���������
-- --------------------------------------

CREATE PROCEDURE [dbo].[InsertNewTask]
	-- ��������� ��� ���������� ������ � ����� ������������� - ������ ������ ������� (� ������� Task)
	@idEmployee int, @idCandidate int, @issue date, @period int
AS
	insert into Task values (null, 1, @idEmployee, @idCandidate, @issue, @period, null, null)

------------------------------------------------------------------


CREATE PROCEDURE [dbo].[SelectAllTasks]
	-- ��������� ������� ���� ������� ������� Task
AS
	select * from Task

-------------------------------------------------------------------

CREATE PROCEDURE [dbo].[DeleteTask] 
	-- ��������� �������� ������ 
	@idTask int
AS
	delete from Task where idTask = @idTask

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EditTask]
	-- ��������� �������������� �������
	@idTask int, @idEmployee int, @idCandidate int, @issue date, @period int
AS
	if exists (select * from Task where Task.idTask = @idTask)
		update Task set 
			Task.Employee_idEmployee = @idEmployee,
			Task.Candidate_idCandidate = @idCandidate,
			Task.IssueDate = @issue,
			Task.Period = @period
		WHERE Task.idTask = @idTask
	else 
		raiserror('������ � ����� id �� ����������', 1, 1)

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[GetTaskById]
	-- ��������� ��������� ������ �� ��������������
	@id int
AS
	Select * from Task where idTask = @id

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[InsertDateOfCheck]
	-- ���������� ������ � ���, ��� ������������ ������������ ������� ������� �� �������� (����)
	@date datetime, @idDir int, @idTask int
AS
	if (exists (select * from Task where idTask = @idTask)) and (exists (select * from Director where idDirector = @idDir))

		update Task set Stat_idStat = 2, VerificationDate = @date, Director_idDirector = @idDir where idTask = @idTask

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SetBadScore]
	-- ��������� �������� �� ������� ��������� ����� ���������� �������
	@currentDate date
AS
	update Task set Score = 0, Stat_idStat = 4 where dateadd(day, Period, IssueDate) <= @currentDate and VerificationDate is null

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SetScore]
	-- ��������� ����������� ������
	@idTask int, @score char
AS
	if exists (select * from Task where idTask = @idTask)

		update Task set Stat_idStat = 3, Score = @score where idTask = @idTask

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[CreateReport]
	-- ��������� ������������ ������
	@startDate date, @endDate date
AS
	Select 
		c.Name as '��� ����������', 
		c.PhoneNumber as '����� ��������', 
		c.Position as '���������',
		e.Name as '������� ����������',
		e.Position as '��������� ����������',
		t.IssueDate as '���� �������������',
		t.Period as '���������� �����',
		s.State as '������',
		d.Name as '������� ������������',
		t.VerificationDate as '���� ��������',
		t.Score as '������',
		case 
			when Score != 0 then Round(Cast(Score as float) / Cast(datediff(day, t.IssueDate, t.VerificationDate) as float),2)
			else 0
		end '�������� ������'

		FROM Task as t
			join Stat as s on t.Stat_idStat = s.idStat
			join Candidate as c on t.Candidate_idCandidate = c.idCandidate
			join Employee as e on t.Employee_idEmployee = e.idEmployee
			left join Director as d on t.Director_idDirector = d.idDirector
		where t.IssueDate between @startDate and @endDate 


-- ������� ������ � ������� Task, ��������� �������� ���������
exec InsertNewTask '1', '1', '10.10.2022', '5'
exec InsertNewTask '2', '3', '11.10.2022', '3'
exec InsertNewTask '3', '2', '12.10.2022', '1'



-- !!! ��������� enable_broker = true ��� ��������� ������������ � WebApi
ALTER DATABASE ServiceBrokerTest SET ENABLE_BROKER; 