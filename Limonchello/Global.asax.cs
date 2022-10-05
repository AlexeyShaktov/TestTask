using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Limonchello
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        string con = ConfigurationManager.ConnectionStrings["webapi_con"].ConnectionString;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Запуск прослушки уведомлений от БД
            SqlDependency.Start(con);

            // Запуск метода для создания новой зависимости
            // Новый экземпляр класса SqlDependency должен создаваться после получения каждого уведомления
            CreateDependency();

        }

        protected void Application_End()
        {
            SqlDependency.Stop(con);
        }

        public void CreateDependency()
        {
            using (SqlConnection connection = new SqlConnection(con))
            {
                // Создаем новый запрос к БД
                // Выбираем поля, которые хотим мониторить
                SqlCommand cmd = new SqlCommand("select Stat_idStat from dbo.Task", connection);

                // Создаем новый экземпляр SqlDependency
                SqlDependency dep = new SqlDependency(cmd);

                // Подписываемся на хендлер для обработки уведомления об изменениях в БД
                dep.OnChange += new OnChangeEventHandler(Dependendency_OnChange);

                // Выполняем запрос
                connection.Open();
                var s = cmd.ExecuteReader();
            }
        }

        private void Dependendency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            // Проверим тип поступившего уведомления
            // При первом вызове хендлера при запуске приложения e.Type = Suscribe, при изменении в БД e.Type = Change
            if (e.Type != SqlNotificationType.Subscribe)
            {

                // Отправим Post запрос на внешнюю систему с уведомлением об изменениях статуса задания
                using (var client = new HttpClient())
                {
                    var url = "https://httpbin.org/post";

                    var Data = "Состояние некоторых заданий изменено";
                    System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                    req.Method = "POST";
                    req.Timeout = 100000;
                    req.ContentType = "application/x-www-form-urlencoded";
                    byte[] sentData = Encoding.GetEncoding(1251).GetBytes(Data);
                    req.ContentLength = sentData.Length;
                    System.IO.Stream sendStream = req.GetRequestStream();
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();
                }

                // Вызываем метод создания новой SqlDependency
                CreateDependency();
            }

        }

    }
}
