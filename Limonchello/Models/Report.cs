using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Limonchello.Models
{
    /*
     * Класс, описывающий модель данных, возвращаемых Хранимой процедурой формирования отчета
     * Каждое поле класса - столбец таблицы
    */

    public class Report
    {
        public string cName { get; set; }

        public string cPhoneNumber { get; set; }

        public string cPosition { get; set; }

        public string eName { get; set; }

        public string ePosition { get; set; }

        public string eIssueDate { get; set; }

        public int tPeriod { get; set; }

        public string sState { get; set; }

        public string dName { get; set; }

        public string dVerificationDate { get; set; }

        public string dScore { get; set; }

        public string ResultScore { get; set; } 
    }
}