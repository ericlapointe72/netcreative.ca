using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Subscribers : System.Web.UI.Page
    {
        private string connectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            ((CommandField)GridView_Subscribe.Columns[0]).DeleteText = Global.Admin_Delete;

            if (!IsPostBack)
            {
                Subscriber_Load();
            }
        }

        // Only one page of the table is read (the newest rows first), not the whole table: it grows with every subscription.
        private void Subscriber_Load()
        {
            try
            {
                int total;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int confirmed;

                    using (var cmd = new SqlCommand("SELECT COUNT(*), COUNT(CONFIRMED) FROM SUBSCRIBE", connection))
                    using (SqlDataReader counts = cmd.ExecuteReader())
                    {
                        counts.Read();
                        total = counts.GetInt32(0);
                        confirmed = counts.GetInt32(1);
                    }

                    int size = GridView_Subscribe.PageSize;
                    int page = Math.Max(0, Math.Min(GridView_Subscribe.PageIndex, (Math.Max(total, 1) - 1) / size));
                    GridView_Subscribe.PageIndex = page;
                    GridView_Subscribe.VirtualItemCount = total;

                    var subscribes = new List<Subscribe>();

                    using (var cmd = new SqlCommand(
                        "SELECT AUTO_NUMBER, DATE, TIME, IP_ADDRESS, EMAIL, CONFIRMED FROM SUBSCRIBE ORDER BY AUTO_NUMBER DESC OFFSET @OFFSET ROWS FETCH NEXT @SIZE ROWS ONLY", connection))
                    {
                        cmd.Parameters.Add("@OFFSET", SqlDbType.Int).Value = page * size;
                        cmd.Parameters.Add("@SIZE", SqlDbType.Int).Value = size;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var s = new Subscribe();
                                s.Number = dr["AUTO_NUMBER"].ToString();
                                s.Date = ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd");
                                s.Time = ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm\:ss");
                                s.IP_Address = dr["IP_ADDRESS"].ToString();
                                s.Email = dr["EMAIL"].ToString();
                                s.Status = dr["CONFIRMED"] == DBNull.Value ? Global.Text("Admin_SubStatusWait") : Global.Text("Admin_SubStatusOk");
                                subscribes.Add(s);
                            }
                        }
                    }

                    GridView_Subscribe.DataSource = subscribes;
                    GridView_Subscribe.DataBind();

                    Label_Subscribe_Info.Text = total == 0
                        ? string.Empty
                        : Global.Format(Global.Text("Admin_PageInfo"), page * size + 1, Math.Min(total, page * size + size), total)
                            + " - " + Global.Format(Global.Text("Admin_SubSummary"), confirmed, total - confirmed);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "subscribers page");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }

        // each row has its own Delete link, with a question first (the row to delete used to be typed as a number in a box)
        protected void GridView_Subscribe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var link = e.Row.Cells[0].Controls[0] as LinkButton;
                if (link != null)
                {
                    link.OnClientClick = AdminUi.ConfirmScript(Global.Format(Global.Text("Admin_RowDeleteConfirm"), GridView_Subscribe.DataKeys[e.Row.RowIndex].Value));
                }
            }
        }

        protected void GridView_Subscribe_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Subscribe.PageIndex = e.NewPageIndex;
            Subscriber_Load();
        }

        protected void GridView_Subscribe_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string number = Convert.ToString(GridView_Subscribe.DataKeys[e.RowIndex].Value);
            int id;

            if (!int.TryParse(number, out id))
            {
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM SUBSCRIBE WHERE AUTO_NUMBER = @AUTO_NUMBER", connection))
                {
                    cmd.Parameters.Add("@AUTO_NUMBER", SqlDbType.Int).Value = id;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                AdminAudit.Write("Subscriber deleted", "number " + number);
                Subscriber_Load();
                AdminUi.Toast(this, Global.Format(Global.Admin_SubscriberDeletedSuccess, number), true);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "subscribers page");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }
    }
}
