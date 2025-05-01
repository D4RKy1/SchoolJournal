using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

public class DatabaseHelper
{
    public string connectionString = "Server=localhost;Database=SchoolJournal;Uid=root;Pwd=root;";

    public DataTable GetData(string query)
    {
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }
    }

    public void ExecuteQuery(string query)
    {
        using (var conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}