using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Sql : System.Web.UI.Page
    {
        private string connectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            TextBox_Sql_Query.Attributes["placeholder"] = Global.Admin_SqlPlaceholder;
            Button_Sql_Execute.Text = Global.Admin_SqlExecuteButton;
            Button_Sql_Execute.OnClientClick = "return confirm('" + Global.Admin_SqlConfirmMessage.Replace("'", "\\'") + "');";
        }

        protected void Button_Sql_Execute_Click(object sender, EventArgs e)
        {
            string query = TextBox_Sql_Query.Text.Trim();

            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                GridView_Sql_Result.DataSource = table;
                                GridView_Sql_Result.DataBind();
                                Label_Sql_Status.Text = Global.Format(Global.Admin_SqlRowsReturned, table.Rows.Count);
                            }
                        }
                        else
                        {
                            int affected = cmd.ExecuteNonQuery();

                            GridView_Sql_Result.DataSource = null;
                            GridView_Sql_Result.DataBind();
                            Label_Sql_Status.Text = Global.Format(Global.Admin_SqlRowsAffected, affected);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GridView_Sql_Result.DataSource = null;
                GridView_Sql_Result.DataBind();
                Label_Sql_Status.Text = ex.Message;
            }
        }
    }
}
