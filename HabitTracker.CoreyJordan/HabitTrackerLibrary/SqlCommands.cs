﻿using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitTrackerLibrary;
public static class SqlCommands
{
    public static int DeleteRecord(int entryId)
    {
        int rowCount;
        using (var conn = new SqliteConnection(DataConnection.ConnString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"DELETE FROM drinking_water WHERE Id = {entryId}";
            rowCount = cmd.ExecuteNonQuery();
        }
        return rowCount;
    }

    public static List<DrinkingWater> GetAllRecords()
    {
        List<DrinkingWater> drinks = new();

        using (var conn = new SqliteConnection(DataConnection.ConnString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"SELECT * FROM drinking_water";
            SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                drinks.Add(new DrinkingWater
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.ParseExact(reader.GetString(1), "MM-dd-yy", new CultureInfo("en-US")),
                    Quantity = reader.GetInt32(2)
                });
            }
        }
        return drinks;
    }

    public static void InitializeDB(string connString)
    {
        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water
        (Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Date TEXT,
        Quantity INTEGER)";

            cmd.ExecuteNonQuery();
        }
    }

    public static void InsertRecord(string date, int quantity)
    {
        using (var conn = new SqliteConnection(DataConnection.ConnString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";
            cmd.ExecuteNonQuery();
        }
    }

    public static bool RecordExists(int entryId)
    {
        bool recordExists = true;
        int checkQuery;
        using (var conn = new SqliteConnection(DataConnection.ConnString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {entryId})";
            checkQuery = Convert.ToInt32(cmd.ExecuteScalar());
        }

        if (checkQuery == 0)
        {
            recordExists = false;
        }

        return recordExists;
    }

    public static void UpdateRecord(DrinkingWater drink)
    {
        using (var conn = new SqliteConnection(DataConnection.ConnString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"UPDATE drinking_water SET date = '{drink.Date:MM-dd-yy}', quantity = {drink.Quantity} WHERE Id = {drink.Id}";
            cmd.ExecuteNonQuery();
        }
    }
}
