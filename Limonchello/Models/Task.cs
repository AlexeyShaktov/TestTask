using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;



namespace Limonchello.Models
{
    /*
     * Класс, описывающий сущность Task в нашей БД
     * Каждое поле класса - столбец таблицы
    */

    public class Task
    {
        public int idTask { get; set; }

        public int Director_idDirector { get; set; }

        public int Stat_idStat { get; set; }

        public int Employee_idEmployee { get; set; }

        public int Candidate_idCandidate { get; set; }

        public DateTime IssueDate { get; set; }

        public int Period { get; set; }

        public DateTime VerificationDate { get; set; }

        public int Score { get; set; }

    }
}