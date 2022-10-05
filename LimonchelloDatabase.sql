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
	('89507127723','Менеджер по продажам','Семенов Игорь Борисович'), 
	('89617122613','Повар','Иванов Виталий Алексеевич'), 
	('89527772691','Специалист по технике безопасности','Крестинин Данил Алексеевич'),
	('89528239976','Менеджер по продажам','Покровский Виктор Глебович')

insert into Employee Values
	('Борисов','Бухгалтер'), 
	('Захаров','Повар'), 
	('Живов','Менеджер по продажам')

insert into Stat Values 
	('Задание получено'),
	('Задание сдано на проверку'),
	('Заданию выставлена оценка сотрудником'),
	('Истекло время выполнения задания')

insert into Director Values
	('Воронин'),
	('Зиновьев'),
	('Максименко')


-- Хранимые процедуры
-- --------------------------------------

CREATE PROCEDURE [dbo].[InsertNewTask]
	-- Процедура для добавления записи о новом собеседовании - выдача нового задания (в таблицу Task)
	@idEmployee int, @idCandidate int, @issue date, @period int
AS
	insert into Task values (null, 1, @idEmployee, @idCandidate, @issue, @period, null, null)

------------------------------------------------------------------


CREATE PROCEDURE [dbo].[SelectAllTasks]
	-- Процедура выборки всех записей таблицы Task
AS
	select * from Task

-------------------------------------------------------------------

CREATE PROCEDURE [dbo].[DeleteTask] 
	-- Процедура удаления записи 
	@idTask int
AS
	delete from Task where idTask = @idTask

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EditTask]
	-- Процедура редактирования задания
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
		raiserror('Записи с таким id не существует', 1, 1)

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[GetTaskById]
	-- Процедура получения записи по идентификатору
	@id int
AS
	Select * from Task where idTask = @id

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[InsertDateOfCheck]
	-- Добавление записи о том, что определенный руководитель получил задание на проверку (дата)
	@date datetime, @idDir int, @idTask int
AS
	if (exists (select * from Task where idTask = @idTask)) and (exists (select * from Director where idDirector = @idDir))

		update Task set Stat_idStat = 2, VerificationDate = @date, Director_idDirector = @idDir where idTask = @idTask

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SetBadScore]
	-- Процедура проверки на предмет истечения срока выполнения задания
	@currentDate date
AS
	update Task set Score = 0, Stat_idStat = 4 where dateadd(day, Period, IssueDate) <= @currentDate and VerificationDate is null

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SetScore]
	-- Процедура выставления оценки
	@idTask int, @score char
AS
	if exists (select * from Task where idTask = @idTask)

		update Task set Stat_idStat = 3, Score = @score where idTask = @idTask

------------------------------------------------------------------

CREATE PROCEDURE [dbo].[CreateReport]
	-- Процедура формирования отчета
	@startDate date, @endDate date
AS
	Select 
		c.Name as 'ФИО соискателя', 
		c.PhoneNumber as 'Номер телефона', 
		c.Position as 'Должность',
		e.Name as 'Фамилия сотрудника',
		e.Position as 'Должность сотрудника',
		t.IssueDate as 'Дата собеседования',
		t.Period as 'Отведенное время',
		s.State as 'Статус',
		d.Name as 'Фамилия руководителя',
		t.VerificationDate as 'Дата проверки',
		t.Score as 'Оценка',
		case 
			when Score != 0 then Round(Cast(Score as float) / Cast(datediff(day, t.IssueDate, t.VerificationDate) as float),2)
			else 0
		end 'Итоговая оценка'

		FROM Task as t
			join Stat as s on t.Stat_idStat = s.idStat
			join Candidate as c on t.Candidate_idCandidate = c.idCandidate
			join Employee as e on t.Employee_idEmployee = e.idEmployee
			left join Director as d on t.Director_idDirector = d.idDirector
		where t.IssueDate between @startDate and @endDate 


-- Добавим записи в таблицу Task, используя хранимую процедуру
exec InsertNewTask '1', '1', '10.10.2022', '5'
exec InsertNewTask '2', '3', '11.10.2022', '3'
exec InsertNewTask '3', '2', '12.10.2022', '1'



-- !!! Установим enable_broker = true для получения уведоомлений в WebApi
ALTER DATABASE ServiceBrokerTest SET ENABLE_BROKER; 