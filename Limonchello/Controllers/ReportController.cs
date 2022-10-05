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

namespace Limonchello.Controllers
{
    public class ReportController : ApiController
    {
        // Создаем и инициализируем подключение к БД

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["webapi_con"].ConnectionString);

        public string Get()
        {
            return "Empty";
        }

        // Get запрос формирования отчета по хранимой процедуре на БД CreateReport
        // api/Report/GetReport?start=01.09.2022&end=30.10.2022

        public List<Report> GetReport(string start, string end)
        {
            // Создаем dataadapter для получения данных по хранимой процедуре
            SqlDataAdapter da = new SqlDataAdapter("CreateReport", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            // Указываем необходимые для процедуры параметры
            da.SelectCommand.Parameters.AddWithValue("startDate", Convert.ToDateTime(start));
            da.SelectCommand.Parameters.AddWithValue("endDate", Convert.ToDateTime(end));

            // Выполняем запрос к БД
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Парсим полученный ответ от БД
            // В цикле перебираем полученные данные, по которым создаем новый объект класса Report
            List<Report> listRep = new List<Report>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Report rep = new Report();
                    rep.cName = dt.Rows[i]["ФИО соискателя"].ToString();
                    rep.cPhoneNumber = dt.Rows[i]["Номер телефона"].ToString();
                    rep.cPosition = dt.Rows[i]["Должность"].ToString();
                    rep.eName = dt.Rows[i]["Фамилия сотрудника"].ToString();
                    rep.ePosition = dt.Rows[i]["Должность сотрудника"].ToString();
                    rep.eIssueDate = dt.Rows[i]["Дата собеседования"].ToString();
                    rep.tPeriod = Convert.ToInt32(dt.Rows[i]["Отведенное время"]);
                    rep.sState = dt.Rows[i]["Статус"].ToString();
                    if (dt.Rows[i]["Фамилия руководителя"] is System.DBNull)
                    {
                        rep.dName = "";
                    }
                    else
                    {
                        rep.dName = dt.Rows[i]["Фамилия руководителя"].ToString();
                    }
                    if (dt.Rows[i]["Дата проверки"] is System.DBNull)
                    {
                        rep.dVerificationDate = null;
                    }
                    else
                    {
                        rep.dVerificationDate = dt.Rows[i]["Дата проверки"].ToString();
                    }
                    if (dt.Rows[i]["Оценка"] is DBNull)
                    {
                        rep.dScore = null;
                    }
                    else
                    {
                        rep.dScore = dt.Rows[i]["Оценка"].ToString();
                    }
                    rep.ResultScore = dt.Rows[i]["Итоговая оценка"].ToString();

                    // Добавляем новый объект в коллекцию
                    listRep.Add(rep);
                }
                return listRep;
            }
            else
            {
                return null;
            }
        }
    }
}
