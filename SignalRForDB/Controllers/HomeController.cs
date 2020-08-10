using SignalRForDB.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalRForDB.Models;

namespace SignalRForDB.Controllers
{
    public class HomeController : Controller
    {
        private LabEntities db = new LabEntities();
        readonly string _connString = ConfigurationManager.ConnectionStrings["SignalRConnection"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT [id],[value] FROM [dbo].[SignalR]", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();
                }
            }

            return View(db.SignalRs.ToList());
        }
               
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.Test();
            }
        }


    }
}