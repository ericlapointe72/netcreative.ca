using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class ContactLog : System.Web.UI.Page
    {
        private const int PageSize = 20;

        private string connectionString = string.Empty;

        // the page of messages shown (0 = the newest), kept between requests
        private int PageIndex
        {
            get { return ViewState["PageIndex"] is int ? (int)ViewState["PageIndex"] : 0; }
            set { ViewState["PageIndex"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Newer.Text = Global.Text("Admin_NewerButton");
            Button_Older.Text = Global.Text("Admin_OlderButton");

            if (!IsPostBack)
            {
                Contact_Load();
            }
        }

        // Only one page of messages is read (the newest first), not the whole table.
        private void Contact_Load()
        {
            try
            {
                int total;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM CONTACT", connection))
                    {
                        total = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    int page = Math.Max(0, Math.Min(PageIndex, (Math.Max(total, 1) - 1) / PageSize));
                    PageIndex = page;

                    var contact = new List<Contact>();

                    using (var cmd = new SqlCommand(
                        "SELECT * FROM CONTACT ORDER BY AUTO_NUMBER DESC OFFSET @OFFSET ROWS FETCH NEXT @SIZE ROWS ONLY", connection))
                    {
                        cmd.Parameters.Add("@OFFSET", SqlDbType.Int).Value = page * PageSize;
                        cmd.Parameters.Add("@SIZE", SqlDbType.Int).Value = PageSize;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var c = new Contact();
                                c.Number = Convert.ToInt32(dr["AUTO_NUMBER"]);
                                c.When = ((DateTime)dr["DATE"]).ToString("yyyy-MM-dd") + " · " + ((TimeSpan)dr["TIME"]).ToString(@"hh\:mm");
                                c.Name = (dr["FIRST_NAME"].ToString() + " " + dr["LAST_NAME"].ToString()).Trim();
                                c.Company = dr["COMPANY"].ToString();
                                c.Email = dr["EMAIL"].ToString();
                                c.Phone = dr["PHONE"].ToString();
                                c.ProjectType = dr["PROJECT_TYPE"].ToString();
                                c.Budget = dr["PROJECT_BUDGET"].ToString();
                                c.Deadline = dr["PROJECT_DEADLINE"].ToString();
                                c.Message = dr["DESCRIPTION"].ToString();
                                contact.Add(c);
                            }
                        }
                    }

                    Repeater.DataSource = contact;
                    Repeater.DataBind();

                    Button_Newer.Visible = page > 0;
                    Button_Older.Visible = (page + 1) * PageSize < total;
                    Label_Contact_Info.Text = total == 0
                        ? string.Empty
                        : Global.Format(Global.Text("Admin_PageInfo"), page * PageSize + 1, Math.Min(total, page * PageSize + PageSize), total);
                }
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        // each message has its own Delete button, with a question first (the message to delete used to be typed as a number in a box)
        protected void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var button = (Button)e.Item.FindControl("Button_Row_Delete");
                var contact = (Contact)e.Item.DataItem;

                button.Text = Global.Admin_Delete;
                button.OnClientClick = AdminUi.ConfirmScript(Global.Format(Global.Text("Admin_RowDeleteConfirm"), contact.Number));
            }
        }

        protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "delete")
            {
                return;
            }

            int number;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out number))
            {
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM CONTACT WHERE AUTO_NUMBER = @NUMBER", connection))
                {
                    cmd.Parameters.Add("@NUMBER", SqlDbType.Int).Value = number;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                AdminAudit.Write("Contact message deleted", "number " + number);
                Contact_Load();
                AdminUi.Toast(this, Global.Format(Global.Admin_ContactDeletedSuccess, number), true);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        protected void Button_Newer_Click(object sender, EventArgs e)
        {
            PageIndex = Math.Max(0, PageIndex - 1);
            Contact_Load();
        }

        protected void Button_Older_Click(object sender, EventArgs e)
        {
            PageIndex = PageIndex + 1;
            Contact_Load();
        }

        // the details go to the error log (App_Data\logs), never to the browser
        private void Fail(Exception ex)
        {
            ErrorLogger.Log(ex, "contact log page");
            AdminUi.Toast(this, Global.Text("Admin_Error"), false);
        }
    }
}
