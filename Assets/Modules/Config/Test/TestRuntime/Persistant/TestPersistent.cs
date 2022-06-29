using System.Collections;
using System.Collections.Generic;
using Cr7Sund.Core.Persistance;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
using Mono.Data.Sqlite;
using System.Data;

[PrebuildSetup(nameof(TestPersistentInit))]
public class TestPersistent
{

    // A test with the [RequiresPlayMode] tag ensures that the test is always run inside PlayMode.
    [UnityTest]
    [RequiresPlayMode]
    // [Performance]
    public IEnumerator TestBinary()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.

        using (var writer = new GameDataWriter(TestPersistentInit.PersistantName))
        //short
        {
            for (int i = 0; i < TestPersistentInit.intLength; i++)
            {
                writer.Write(i);
            }
            for (int i = 0; i < TestPersistentInit.floatLength; i++)
            {
                writer.Write((float)i);
            }
            for (int i = 0; i < TestPersistentInit.stringLength; i++)
            {
                writer.Write(i.ToString());
            }
        }
        //writer.Close(); Or dispose write explictly


        using (var reader = new GameDataReader(TestPersistentInit.PersistantName))
        {
            for (int i = 0; i < TestPersistentInit.intLength; i++)
            {
                Assert.AreEqual(i, reader.ReadInt());
            }
            for (int i = 0; i < TestPersistentInit.floatLength; i++)
            {
                Assert.AreEqual((float)i, reader.ReadFloat());
            }
            for (int i = 0; i < TestPersistentInit.stringLength; i++)
            {
                Assert.AreEqual(i.ToString(), reader.ReadString());
            }
        }


        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCreateDB()
    {
        CreateAndOpenDatabase();

        yield return null;
    }

    private void CreateTable(SqliteConnection databaseConnection)
    {
        using (databaseConnection)
        {
            databaseConnection.Open();
            var databaseCommand = databaseConnection.CreateCommand();
            string sqlQuery = "CREATE TABLE IF NOT EXISTS[table_Gebruikers] (id INTEGER PRI$$anonymous$$ARY KEY AUTOINCRE$$anonymous$$ENT NOT NULL, voornaam VARCHAR(20) NOT NULL, andereNamen varchar(50), achternaam VARCHAR(20) NOT NULL, wachtwoord VARCHAR(255) NOT NULL);";
            string queryCreateAdd = "INSERT INTO table_Gebruikers (voornaam, achternaam, wachtwoord) VALUES ('ad$$anonymous$$', 'ad$$anonymous$$Achternaam', 'ad$$anonymous$$');";


            databaseCommand.CommandText = sqlQuery;
            databaseCommand.ExecuteScalar();
            databaseCommand.CommandText = queryCreateAdd;
            databaseCommand.ExecuteScalar();
            databaseConnection.Close();
        }
    }

    /// <summary>
    /// Create and Opening a database connection
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateAndOpenDatabase() // 3
    {
        // Open a connection to the database 
        string dbUri = "URI=file:MyDatabase.sqlite";
        var dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        var openTask = dbConnection.OpenAsync();

        // Create a table for the hit count in the database if it does not exist yet.
        var dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS MYTable (id INTEGER PRIMARY KEY, hit INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        return dbConnection;
    }
}
