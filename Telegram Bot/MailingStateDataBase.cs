using System;

using System.Data.SQLite;
using System.Data;

namespace Telegram_Bot
{
    class MailingStateDataBase
    {
        static SQLiteCommand command;
        public static void CreateTable()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=mailing_state.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = "CREATE TABLE IF NOT EXISTS [mailing_state]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [chatid] INTAGER UNIQUE, [state_text] TEXT, [state_photo] TEXT);"
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void NewStateChatID(long chat_id) {
            SQLiteConnection connection = new SQLiteConnection("Data Source=mailing_state.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = $"INSERT INTO mailing_state (chatid) VALUES ( \"{chat_id}\")"
        };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void StateText(long chat_id, string text)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=mailing_state.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = $"UPDATE mailing_state SET state_text = \"{text}\" WHERE chatid = \"{chat_id}\""
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void StatePhoto(long chat_id, string photo)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=mailing_state.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = $"UPDATE mailing_state SET state_photo = \"{photo}\" WHERE chatid = \"{chat_id}\""
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static string[] GetStateData(long chat_id) {
            SQLiteConnection connection = new SQLiteConnection("Data Source=mailing_state.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command.CommandText = $"SELECT * FROM mailing_state WHERE chatid = {chat_id}";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            string state_text, state_photo;

            foreach (DataRow row in data.Rows)
            {
                state_text = row.Field<string>("state_text");
                state_photo = row.Field<string>("state_photo");
                connection.Close();
                string[] mailing_data = { state_text, state_photo };
                return mailing_data;
            }
            return null;
        }

        public static void DeleteState(long chat_id)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=mailing_state.sqlite;Version=3; FailIfMissing=False");
            connection.Open();
            command = new SQLiteCommand(connection)
            {
                CommandText = $"DELETE FROM mailing_state WHERE chatid = \"{chat_id}\""
            };
            command.ExecuteNonQuery();
            connection.Close();
        }

    }

}
