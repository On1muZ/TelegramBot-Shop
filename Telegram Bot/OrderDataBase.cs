using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;

namespace Telegram_Bot
{
     class OrderDataBase
    {
        static SQLiteCommand command;
        public static void CreateTable()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=orders.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = "CREATE TABLE IF NOT EXISTS [orders]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [text] TEXT UNIQUE, [photo] TEXT);"
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void AddOrder(string text, string photo)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=orders.sqlite;Version=3; FailIfMissing=False");
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"INSERT INTO orders (text, photo) VALUES (\"{text}\", \"{photo}\")"
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static List<string[]> GetOrders()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=orders.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"SELECT * FROM orders"
            };
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            List<string[]> orders = new List<string[]>();
            string text, photo;

            foreach(DataRow row in data.Rows)
            {
                text = row.Field<string>("text");
                photo = row.Field<string>("photo");

                orders.Add(new string[] { text, photo });
            }

            return orders;
        }

        public static void DeleteOrder(string text)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=orders.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = $"DELETE FROM orders WHERE text = \"{text}\""
            };
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
