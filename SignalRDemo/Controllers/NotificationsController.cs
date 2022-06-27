using SignalRDemo.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRDemo.Controllers
{
    public class NotificationsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Get()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["conSignalr"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT ID,Name,Price FROM [dbo].[Flower]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    //command.ExecuteNonQuery();
                    SqlDataReader reader = command.ExecuteReader();

                    var listEmp = reader.Cast<IDataRecord>()
                            .Select(x => new
                            {
                                ID = (int)x["ID"],
                                Name = (string)x["Name"],
                                Price = (int)x["Price"],
                            }).ToList();

                    return Json(new { listEmp = listEmp }, JsonRequestBehavior.AllowGet);
                    //return Json("" , JsonRequestBehavior.AllowGet);

                }
            }
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            NotificationHub.Show();
        }
    }
}