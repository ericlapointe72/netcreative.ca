using System;
using System.Data;
using System.Data.SqlClient;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // One day of the opening hours. The times are minutes after midnight (8:30 is 510); Open is false for a closed day.
    public class OpeningDay
    {
        public bool Open;
        public int OpensAt;
        public int ClosesAt;
    }

    // The opening hours of the shop. They live in the OPENING table: four rows (OPEN_HOUR, OPEN_MIN, CLOSE_HOUR, CLOSE_MIN) and one
    // column per day (MON first). A day whose OPEN_HOUR is 0 is closed. The admin page "Opening hours" writes them; the footer of every
    // page and the preview of the admin page show them with Line(), so the two always read the same.
    public static class OpeningHours
    {
        private static readonly string[] RowKeys = { "OPEN_HOUR", "OPEN_MIN", "CLOSE_HOUR", "CLOSE_MIN" };
        private static readonly string[] DayColumns = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };

        public static int DayCount
        {
            get { return DayColumns.Length; }
        }

        public static string DayName(int day)
        {
            switch (day)
            {
                case 0: return Global.Master_Monday;
                case 1: return Global.Master_Tuesday;
                case 2: return Global.Master_Wednesday;
                case 3: return Global.Master_Thursday;
                case 4: return Global.Master_Friday;
                case 5: return Global.Master_Saturday;
                default: return Global.Master_Sunday;
            }
        }

        // the seven days, Monday first
        public static OpeningDay[] Load(string connectionString)
        {
            var values = new int[RowKeys.Length, DayColumns.Length];

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM OPENING", connection))
            {
                connection.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int row = Array.IndexOf(RowKeys, Convert.ToString(dr["OPEN_CLOSE"]).Trim());
                        if (row < 0)
                        {
                            continue;
                        }

                        for (int d = 0; d < DayColumns.Length; d++)
                        {
                            int n;
                            int.TryParse(Convert.ToString(dr[DayColumns[d]]).Trim(), out n);
                            values[row, d] = n;
                        }
                    }
                }
            }

            var days = new OpeningDay[DayColumns.Length];

            for (int d = 0; d < days.Length; d++)
            {
                days[d] = new OpeningDay
                {
                    Open = values[0, d] != 0,
                    OpensAt = values[0, d] * 60 + values[1, d],
                    ClosesAt = values[2, d] * 60 + values[3, d]
                };
            }

            return days;
        }

        // The four rows are saved together: either all four change or none does. A closed day keeps no other hour (0, 0, 0, 0).
        // The hours are stored as the site shows them (8, not 08) and the minutes on two digits (00).
        public static void Save(string connectionString, OpeningDay[] days)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    for (int r = 0; r < RowKeys.Length; r++)
                    {
                        using (var cmd = new SqlCommand(
                            "UPDATE OPENING SET MON=@MON, TUE=@TUE, WED=@WED, THU=@THU, FRI=@FRI, SAT=@SAT, SUN=@SUN WHERE OPEN_CLOSE=@OPEN_CLOSE", connection, transaction))
                        {
                            cmd.Parameters.Add("@OPEN_CLOSE", SqlDbType.VarChar, 12).Value = RowKeys[r];

                            for (int d = 0; d < DayColumns.Length; d++)
                            {
                                int minutes = r < 2 ? days[d].OpensAt : days[d].ClosesAt;
                                int n = !days[d].Open ? 0 : (r % 2 == 0 ? minutes / 60 : minutes % 60);

                                cmd.Parameters.Add("@" + DayColumns[d], SqlDbType.VarChar, 2).Value = (r % 2 == 0) ? n.ToString() : n.ToString("00");
                            }

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        // "Lundi: 8h30 à 16h45", "Monday: 8:30am to 4:45pm", "Dimanche: Fermé" (the words and signs come from the resources)
        public static string Line(int day, OpeningDay hours, bool english)
        {
            string name = DayName(day) + ": ";

            if (!hours.Open)
            {
                return name + Global.Master_Closed;
            }

            return name + TimeText(hours.OpensAt, english) + Global.Master_HourSeparator + TimeText(hours.ClosesAt, english);
        }

        // 8h30 or 16h45 in French; 8:30am or 4:45pm in English (12:00 is 12:00pm, not 0:00pm)
        public static string TimeText(int minutes, bool english)
        {
            int hour = minutes / 60;
            string sign = string.Empty;

            if (english)
            {
                sign = hour < 12 ? Global.Master_TimeAm : Global.Master_TimePm;
                hour = hour % 12 == 0 ? 12 : hour % 12;
            }

            return hour + Global.Master_TimeSeparator + (minutes % 60).ToString("00") + sign;
        }
    }
}
