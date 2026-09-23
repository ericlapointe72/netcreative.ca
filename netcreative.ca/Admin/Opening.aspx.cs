using System;
using System.Configuration;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    public partial class Opening : System.Web.UI.Page
    {
        private string connectionString = string.Empty;
        private string opening_message_save = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            Button_Opening_Save.Text = Global.Admin_SaveButton;
            opening_message_save = Global.Admin_OpeningSavedSuccess;

            if (!IsPostBack)
            {
                Opening_Load();
            }
        }

        private void Opening_Load()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT * FROM OPENING";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();

                        SqlDataReader dr = cmd.ExecuteReader();

                        dr.Read();
                        mon_oh.Text = dr["MON"].ToString();
                        tue_oh.Text = dr["TUE"].ToString();
                        wed_oh.Text = dr["WED"].ToString();
                        thu_oh.Text = dr["THU"].ToString();
                        fri_oh.Text = dr["FRI"].ToString();
                        sat_oh.Text = dr["SAT"].ToString();
                        sun_oh.Text = dr["SUN"].ToString();

                        dr.Read();
                        mon_om.Text = dr["MON"].ToString();
                        tue_om.Text = dr["TUE"].ToString();
                        wed_om.Text = dr["WED"].ToString();
                        thu_om.Text = dr["THU"].ToString();
                        fri_om.Text = dr["FRI"].ToString();
                        sat_om.Text = dr["SAT"].ToString();
                        sun_om.Text = dr["SUN"].ToString();

                        dr.Read();
                        mon_ch.Text = dr["MON"].ToString();
                        tue_ch.Text = dr["TUE"].ToString();
                        wed_ch.Text = dr["WED"].ToString();
                        thu_ch.Text = dr["THU"].ToString();
                        fri_ch.Text = dr["FRI"].ToString();
                        sat_ch.Text = dr["SAT"].ToString();
                        sun_ch.Text = dr["SUN"].ToString();

                        dr.Read();
                        mon_cm.Text = dr["MON"].ToString();
                        tue_cm.Text = dr["TUE"].ToString();
                        wed_cm.Text = dr["WED"].ToString();
                        thu_cm.Text = dr["THU"].ToString();
                        fri_cm.Text = dr["FRI"].ToString();
                        sat_cm.Text = dr["SAT"].ToString();
                        sun_cm.Text = dr["SUN"].ToString();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        private void Opening_Save()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommand1 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='OPEN_HOUR'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand1, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", mon_oh.Text);
                        cmd.Parameters.AddWithValue("@TUE", tue_oh.Text);
                        cmd.Parameters.AddWithValue("@WED", wed_oh.Text);
                        cmd.Parameters.AddWithValue("@THU", thu_oh.Text);
                        cmd.Parameters.AddWithValue("@FRI", fri_oh.Text);
                        cmd.Parameters.AddWithValue("@SAT", sat_oh.Text);
                        cmd.Parameters.AddWithValue("@SUN", sun_oh.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    string sqlCommand2 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='OPEN_MIN'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand2, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", string.Format("{0:00}", Convert.ToInt32(mon_om.Text)));
                        cmd.Parameters.AddWithValue("@TUE", string.Format("{0:00}", Convert.ToInt32(tue_om.Text)));
                        cmd.Parameters.AddWithValue("@WED", string.Format("{0:00}", Convert.ToInt32(wed_om.Text)));
                        cmd.Parameters.AddWithValue("@THU", string.Format("{0:00}", Convert.ToInt32(thu_om.Text)));
                        cmd.Parameters.AddWithValue("@FRI", string.Format("{0:00}", Convert.ToInt32(fri_om.Text)));
                        cmd.Parameters.AddWithValue("@SAT", string.Format("{0:00}", Convert.ToInt32(sat_om.Text)));
                        cmd.Parameters.AddWithValue("@SUN", string.Format("{0:00}", Convert.ToInt32(sun_om.Text)));

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    string sqlCommand3 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='CLOSE_HOUR'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand3, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", mon_ch.Text);
                        cmd.Parameters.AddWithValue("@TUE", tue_ch.Text);
                        cmd.Parameters.AddWithValue("@WED", wed_ch.Text);
                        cmd.Parameters.AddWithValue("@THU", thu_ch.Text);
                        cmd.Parameters.AddWithValue("@FRI", fri_ch.Text);
                        cmd.Parameters.AddWithValue("@SAT", sat_ch.Text);
                        cmd.Parameters.AddWithValue("@SUN", sun_ch.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    string sqlCommand4 = "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE='CLOSE_MIN'";

                    using (SqlCommand cmd = new SqlCommand(sqlCommand4, connection))
                    {
                        cmd.Parameters.AddWithValue("@MON", string.Format("{0:00}", Convert.ToInt32(mon_cm.Text)));
                        cmd.Parameters.AddWithValue("@TUE", string.Format("{0:00}", Convert.ToInt32(tue_cm.Text)));
                        cmd.Parameters.AddWithValue("@WED", string.Format("{0:00}", Convert.ToInt32(wed_cm.Text)));
                        cmd.Parameters.AddWithValue("@THU", string.Format("{0:00}", Convert.ToInt32(thu_cm.Text)));
                        cmd.Parameters.AddWithValue("@FRI", string.Format("{0:00}", Convert.ToInt32(fri_cm.Text)));
                        cmd.Parameters.AddWithValue("@SAT", string.Format("{0:00}", Convert.ToInt32(sat_cm.Text)));
                        cmd.Parameters.AddWithValue("@SUN", string.Format("{0:00}", Convert.ToInt32(sun_cm.Text)));

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                Response.Write("<script>showToast('" + opening_message_save + "', 'success');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>showToast('" + ex.Message + "', 'error');</script>");
            }
        }

        protected void Button_Opening_Save_Click(object sender, EventArgs e)
        {
            Opening_Save();
            Opening_Load();
        }
    }
}
