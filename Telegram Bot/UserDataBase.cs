using System;

using System.Data.SQLite;
using System.Data;
using Telegram.Bot.Types;
using System.Collections.Generic;
using System.Linq;

namespace Telegram_Bot
{
    class UserDataBase
    {
        static SQLiteCommand command;
        public static void CreateTable()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = "CREATE TABLE IF NOT EXISTS [users]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [name] TEXT, [chatid] INTAGER UNIQUE, [main_message] INTAGER, [is_admin] BOOLEAN DEFAULT FALSE, [is_user] BOOLEAN DEFAULT TRUE, [state] TEXT DEFAULT no, [order_position] BIGINTAGER DEFAULT 0);"
            };
            command.ExecuteNonQuery();
            connection.Close();
        }
        public static bool IsAdmin(long chat_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command.CommandText = $"SELECT is_admin FROM users WHERE chatid = {chat_id}";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            bool isAdmin = false;

            foreach (DataRow row in data.Rows)
            {
                isAdmin = row.Field<bool>("is_admin");
                connection.Close();
                return isAdmin;
            }
            return isAdmin;
        }
        public static void AddUser(string username, long chat_id, long message_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            
            if (chat_id == 1030563973)
            {
                command.CommandText = $"INSERT INTO users (name, chatid, main_message, is_admin, is_user) VALUES (\"{username}\", \"{chat_id}\", \"{message_id}\", \"{true}\", \"{true}\")";
            }
            else
            {
                command.CommandText = $"INSERT INTO users (name, chatid, main_message, is_admin, is_user) VALUES (\"{username}\", \"{chat_id}\", \"{message_id}\", \"{false}\", \"{true}\")";
            }
            
            
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static bool IsUser(long chat_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command.CommandText = $"SELECT is_user FROM users WHERE chatid = {chat_id}";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            bool isUser = false;

            foreach (DataRow row in data.Rows)
            {
                isUser = row.Field<bool>("is_user");
                connection.Close();
                return isUser;
            }
            return isUser;
        }

        public static string GetState(long chat_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"SELECT state FROM users WHERE chatid = {chat_id}"
            };
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            string state = "no";

            foreach (DataRow row in data.Rows)
            {
                state = row.Field<string>("state");
                connection.Close();
                return state;
            }
            return state;
        }
        
        public static void SetState(long chat_id, string state)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"UPDATE users SET state = \"{state}\" WHERE chatid = \"{chat_id}\""
        };
            command.ExecuteNonQuery();
            connection.Close();

        }

        public static List<long> GetUsers()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open(); 
            command.CommandText = "SELECT chatid FROM users";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            long chat_id;
            var users = new List<long>();

            foreach (DataRow row in data.Rows)
            {
                chat_id = row.Field<long>("chatid");
                users.Add(chat_id);
            }
            connection.Close();
            return users;
        }

        public static void SetOrderPosition(long chat_id, long order_position)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"UPDATE users SET order_position = \"{order_position}\" WHERE chatid = \"{chat_id}\""
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static long GetOrderPosition(long chat_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            command.CommandText = $"SELECT order_position FROM users WHERE chatid = \"{chat_id}\"";
            
            DataTable data = new DataTable();
            new SQLiteDataAdapter(command).Fill(data);

            long order_pos;

            foreach (DataRow row in data.Rows)
            {
                order_pos = row.Field<long>("order_position");
                connection.Close();
                return order_pos;
            }
            return 0;
        }

        public static void SetMainMessage(long chat_id, long message_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"UPDATE users SET main_message = \"{message_id}\" WHERE chatid = \"{chat_id}\""
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static long GetMainMessage(long chat_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=users.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"SELECT main_message FROM users WHERE chatid = \"{chat_id}\""
            };
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            long message_id = 0;

            foreach (DataRow row in data.Rows)
            {
                message_id = row.Field<long>("main_message");
                connection.Close();
                return message_id;
            }
            return message_id;
        }

    }

}
