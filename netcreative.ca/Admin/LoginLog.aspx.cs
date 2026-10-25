using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class LoginLog : System.Web.UI.Page
    {
        private string connectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            ((CommandField)GridView_Login.Columns[0]).DeleteText = Global.Admin_Delete;
            Button_Login_Purge.Text = Global.Text("Admin_LoginPurgeButton");
            Button_Login_Purge.OnClientClick = AdminUi.ConfirmScript(Global.Text("Admin_LoginPurgeConfirm"));

            if (!IsPostBack)
            {
                Login_Load();
            }
        }

        // Only one page of the table is read (the newest rows first): the sign-in log grows with every visit to the sign-in page.
        private void Login_Load()
        {
            try
            {
                int total;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM LOGIN", connection))
                    {
                        total = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    int size = GridView_Login.PageSize;
                    int page = Math.Max(0, Math.Min(GridView_Login.PageIndex, (Math.Max(total, 1) - 1) / size));
                    GridView_Login.PageIndex = page;
                    GridView_Login.VirtualItemCount = total;

                    var login = new List<Login>();

                    using (var cmd = new SqlCommand(
                        "SELECT AUTO_NUMBER, DATE, TIME, IP_ADDRESS, USER_NAME, ACTION FROM LOGIN ORDER BY AUTO_NUMBER DESC OFFSET @OFFSET ROWS FETCH NEXT @SIZE ROWS ONLY", connection))
                    {
                        cmd.Parameters.Add("@OFFSET", SqlDbType.Int).Value = page * size;
                        cmd.Parameters.Add("@SIZE", SqlDbType.Int).Value = size;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var l = new Login();
                                l.Number = dr["AUTO_NUMBER"].ToString();
                                l.Date = ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd");
                                l.Time = ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm\:ss");
                                l.IP_Address = dr["IP_ADDRESS"].ToString();
                                l.User = dr["USER_NAME"].ToString();
                                l.Action = dr["ACTION"].ToString();
                                login.Add(l);
                            }
                        }
                    }

                    GridView_Login.DataSource = login;
                    GridView_Login.DataBind();

                    Label_Login_Info.Text = total == 0
                        ? string.Empty
                        : Global.Format(Global.Text("Admin_PageInfo"), page * size + 1, Math.Min(total, page * size + size), total);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "sign-in log page");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }

        protected void GridView_Login_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var link = e.Row.Cells[0].Controls[0] as LinkButton;
                if (link != null)
                {
                    link.OnClientClick = AdminUi.ConfirmScript(Global.Format(Global.Text("Admin_RowDeleteConfirm"), GridView_Login.DataKeys[e.Row.RowIndex].Value));
                }
            }
        }

        protected void GridView_Login_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Login.PageIndex = e.NewPageIndex;
            Login_Load();
        }

        protected void GridView_Login_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string number = Convert.ToString(GridView_Login.DataKeys[e.RowIndex].Value);
            int id;

            if (!int.TryParse(number, out id))
            {
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM LOGIN WHERE AUTO_NUMBER = @NUMBER", connection))
                {
                    cmd.Parameters.Add("@NUMBER", SqlDbType.Int).Value = id;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                AdminAudit.Write("Sign-in log row deleted", "number " + number);
                Login_Load();
                AdminUi.Toast(this, Global.Format(Global.Admin_LoginDeletedSuccess, number), true);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "sign-in log page");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }

        // The addresses kept in this log are personal information: nothing needs them for more than a year.
        protected void Button_Login_Purge_Click(object sender, EventArgs e)
        {
            try
            {
                int erased;

                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM LOGIN WHERE DATE < DATEADD(month, -12, CAST(GETDATE() AS date))", connection))
                {
                    connection.Open();
                    erased = cmd.ExecuteNonQuery();
                }

                AdminAudit.Write("Sign-in log purged (older than 12 months)", erased + " rows");
                Login_Load();
                AdminUi.Toast(this, Global.Format(Global.Text("Admin_LoginPurged"), erased), true);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "sign-in log page");
                AdminUi.Toast(this, Global.Text("Admin_Error"), false);
            }
        }
    }
}
