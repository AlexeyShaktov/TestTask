using Limonchello.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI;

namespace Limonchello.Controllers
{
    public class TaskController : ApiController
    {
        // Создаем подключение к БД
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["webapi_con"].ConnectionString);

        // Создаем экземпляр класса Task
        Task task = new Task();

        // Получить текущую дату
        DateTime today = DateTime.Today;

        // GET: api/Task
        public List<Task> Get()
        {
            // Создаем dataadapter для получения данных по хранимой процедуре
            SqlDataAdapter da = new SqlDataAdapter("SelectAllTasks", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            // Выполняем запрос
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Парсим полученный ответ от БД
            List<Task> listTasks = new List<Task>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Task task = new Task();
                    task.idTask = Convert.ToInt32(dt.Rows[i]["idTask"]);
                    if (dt.Rows[i]["Director_idDirector"] is System.DBNull)
                    {
                        task.Director_idDirector = 0;
                    }
                    else
                    {
                        task.Director_idDirector = Convert.ToInt32(dt.Rows[i]["Director_idDirector"]);
                    }
                    task.Stat_idStat = Convert.ToInt32(dt.Rows[i]["Stat_idStat"]);
                    task.Employee_idEmployee = Convert.ToInt32(dt.Rows[i]["Employee_idEmployee"]);
                    task.Candidate_idCandidate = Convert.ToInt32(dt.Rows[i]["Candidate_idCandidate"]);
                    task.IssueDate = Convert.ToDateTime(dt.Rows[i]["IssueDate"]);
                    task.Period = Convert.ToInt32(dt.Rows[i]["Period"]);
                    if (dt.Rows[i]["VerificationDate"] is System.DBNull)
                    {
                        task.VerificationDate = Convert.ToDateTime(null);
                    }
                    else
                    {
                        task.VerificationDate = Convert.ToDateTime(dt.Rows[i]["IssueDate"]);
                    }
                    if (dt.Rows[i]["Score"] is System.DBNull)
                    {
                        task.Score = Convert.ToInt32(0);
                    }
                    else
                    {
                        task.Score = Convert.ToInt32(dt.Rows[i]["Score"]);
                    }
                    listTasks.Add(task);
                    
                }
            }
            if (listTasks.Count > 0)
            {
                return listTasks;
            }
            else
            {
                return null;
            }
        }

        // GET: api/Task/5
        public Task Get(int id)
        {
            // Создаем dataadapter для получения данных по хранимой процедуре
            SqlDataAdapter da = new SqlDataAdapter("GetTaskById", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            // Добавляем необходимые параметры хранимой процедуры
            da.SelectCommand.Parameters.AddWithValue("id", id);

            // Выполняем запрос
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Парсим полученный ответ от БД
            Task task = new Task();
            if (dt.Rows.Count > 0)
            {
                task.idTask = Convert.ToInt32(dt.Rows[0]["idTask"]);
                if (dt.Rows[0]["Director_idDirector"] is System.DBNull)
                {
                    task.Director_idDirector = 0;
                }
                else
                {
                    task.Director_idDirector = Convert.ToInt32(dt.Rows[0]["Director_idDirector"]);
                }
                task.Stat_idStat = Convert.ToInt32(dt.Rows[0]["Stat_idStat"]);
                task.Employee_idEmployee = Convert.ToInt32(dt.Rows[0]["Employee_idEmployee"]);
                task.Candidate_idCandidate = Convert.ToInt32(dt.Rows[0]["Candidate_idCandidate"]);
                task.IssueDate = Convert.ToDateTime(dt.Rows[0]["IssueDate"]);
                task.Period = Convert.ToInt32(dt.Rows[0]["Period"]);
                if (dt.Rows[0]["IssueDate"] is System.DBNull)
                {
                    task.VerificationDate = Convert.ToDateTime(dt.Rows[0]["IssueDate"]);
                }
                else
                {
                    task.VerificationDate = Convert.ToDateTime(null);
                }
                if (dt.Rows[0]["Score"] is System.DBNull)
                {
                    task.Score = Convert.ToInt32(0);
                }
                else
                {
                    task.Score = Convert.ToInt32(dt.Rows[0]["Score"]);
                }
                return task;
            }
            else
            {
                return null;
            }
        }


        // POST: api/Task
        public string Post(Task task)
        {
            string message = "";
            if (task != null)
            {
                // Создаем команду, которая будет обращаться к хранимой процедуре добавления новой записи в таблицу Task на сервере БД
                SqlCommand cmd = new SqlCommand("InsertNewTask", con);

                // Устанавливаем тип команды - хранимая процедура
                cmd.CommandType = CommandType.StoredProcedure;

                // Добавим необходимые параметры процедуры
                cmd.Parameters.AddWithValue("@idEmployee", task.Employee_idEmployee.ToString());
                cmd.Parameters.AddWithValue("@idCandidate", task.Candidate_idCandidate.ToString());
                cmd.Parameters.AddWithValue("@issue", Convert.ToDateTime(task.IssueDate));
                cmd.Parameters.AddWithValue("@period", task.Period.ToString());

                // Выполняем запрос
                con.Open();
                int detector = cmd.ExecuteNonQuery();
                con.Close();

                // Возврат сообщения о результатах работы запроса
                if (detector > 0)
                {
                    message = "Запись добавлена";
                }
                else
                {
                    message = "Ошибка";
                }
            }
            return message;
        }

        // Put метод редактирования записей
        // PUT: api/Task/5
        public string Put(int id, Task task)
        {
            string message = "";
            if (task != null)
            {
                // Создаем команду, которая будет обращаться к хранимой процедуре редактирования записи таблицы Task на сервере БД
                SqlCommand cmd = new SqlCommand("EditTask", con);

                // Устанавливаем тип команды - хранимая процедура
                cmd.CommandType = CommandType.StoredProcedure;

                // Добавим необходимые параметры процедуры
                cmd.Parameters.AddWithValue("idTask", task.idTask.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", task.Employee_idEmployee.ToString());
                cmd.Parameters.AddWithValue("@idCandidate", task.Candidate_idCandidate.ToString());
                cmd.Parameters.AddWithValue("@issue", Convert.ToDateTime(task.IssueDate));
                cmd.Parameters.AddWithValue("@period", task.Period.ToString());

                // Выполняем запрос
                con.Open();
                int detector = cmd.ExecuteNonQuery();
                con.Close();

                // Возврат сообщения о результатах работы запроса
                if (detector > 0)
                {
                    message = "Запись изменена";
                }
                else
                {
                    message = "Ошибка";
                }
            }
            return message;
        }

        // Delete метод для удаления записей
        // DELETE: api/Task/5
        public string Delete(int id)
        {
            string message = "";

            // Создаем и инициализируем новый запрос к БД - хранимую процедуру 
            SqlCommand cmd = new SqlCommand("DeleteTask", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Указываем параметры хранимой процедуры 
            // id - идентификатор записи в таблице
            cmd.Parameters.AddWithValue("idTask", id);

            // Выполняем запрос
            con.Open();
            int detector = cmd.ExecuteNonQuery();
            con.Close();

            // Возврат сообщения о результатах работы запроса
            if (detector > 0)
            {
                message = "Запись удалена";
            }
            else
            {
                message = "Ошибка";
            }
            return message;
        }

        // Post запрос для внесения даты проверки для конкретного задания
        [Route("api/Task/SetVerificationDate")]
        [HttpPost]
        public string SetVerificationDate(Task task)
        {
            
            string message = "";

            // Создаем и инициализируем новый запрос к БД - хранимую процедуру 
            SqlCommand cmd = new SqlCommand("InsertDateOfCheck", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Указываем параметры хранимой процедуры
            cmd.Parameters.AddWithValue("date", Convert.ToDateTime(task.VerificationDate));
            cmd.Parameters.AddWithValue("idDir", task.Director_idDirector.ToString());
            cmd.Parameters.AddWithValue("idTask", task.idTask.ToString());

            // Выполняем запрос
            con.Open();
            int detector = cmd.ExecuteNonQuery();
            con.Close();

            // Возврат сообщения о результатах работы запроса
            if (detector > 0)
            {
                message = "Дата установлена";
            }
            else
            {
                message = "Ошибка";
            }
            

            return message;
        }

        // Post запрос для внесения оценки конкретному заданию
        [Route("api/Task/SetScore")]
        [HttpPost]
        public string SetScore(Task task)
        {
            string message = "";

            // Создаем и инициализируем новый запрос к БД - хранимую процедуру 
            SqlCommand cmd = new SqlCommand("SetScore", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Указываем параметры хранимой процедуры
            cmd.Parameters.AddWithValue("idTask", task.idTask.ToString());
            cmd.Parameters.AddWithValue("score", task.Score.ToString());

            // Выполняем запрос
            con.Open();
            int detector = cmd.ExecuteNonQuery();
            con.Close();


            // Возврат сообщения о результатах работы запроса
            if (detector > 0)
            {
                message = "Оценка выставлена";
            }
            else
            {
                message = "Ошибка";
            }
            return message;
        }

        // Get запрос проверки истечения срока выполнения задания
        // Должен вызываться сторонним сервисом автоматически каждый день
        [Route("api/Task/CheckForExpire")]
        [HttpGet]
        public string CheckForExpire()
        {
            string message = "";

            // Создаем и инициализируем новый запрос к БД - хранимую процедуру 
            SqlCommand cmd = new SqlCommand("SetBadScore", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Указываем необходимые параметры
            cmd.Parameters.AddWithValue("currentDate", today);

            // Выполняем запрос
            con.Open();
            int detector = cmd.ExecuteNonQuery();
            con.Close();

            // Возврат сообщения о результатах работы запроса
            if (detector > 0)
            {
                message = "Заданиям с истекшим временем выполнения выставлена оценка 0";
            }
            else
            {
                message = "Заданий с истекшим сроком выполнения нет";
            }
            return message;
        }
    }
}
