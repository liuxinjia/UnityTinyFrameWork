using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Text;
using System;

namespace Cr7Sund.Core.Persistance
{
    public class SQLController : IDisposable
    {

        private SqliteConnection dbConnection;
        public StringBuilder QuerySB
        {
            get
            {
                if (_querySB == null) _querySB = new StringBuilder();
                return _querySB;
            }
        }
        private StringBuilder _querySB;



        #region SQL Methods
        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Execute query success or not </returns>
        public string Q(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dataReader = dbCommand.ExecuteReader())
                    {

                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return null;
        }


        public SqliteConnection OpenConnection(string dbName)
        {
            dbConnection = new SqliteConnection($"URI=file:{dbName}");
            dbConnection.Open();
            //PLAN support open async task 
            // var openTask = dbConnection.OpenAsync();
            return dbConnection;
        }

        public void CloseConnection()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.DisposeAsync();
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }



        #region ReadDatas


        public int QueryShort(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetInt16(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return 0;
        }

        public int QueryInt(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetInt32(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return 0;
        }

        public long QueryLong(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetInt64(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return 0;
        }

        public float QueryFloat(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetFloat(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return 0f;
        }

        public double QueryDouble(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetDouble(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return 0d;
        }

        public byte QueryByte(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetByte(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return Byte.MinValue;
        }


        public char QueryChar(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetChar(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return ' ';
        }

        public string QueryString(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetString(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return string.Empty;
        }

        public bool QueryBoolean(string query)
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dr = dbCommand.ExecuteReader())
                    {
                        dr.Read();

                        if (dr.FieldCount > 1) throw new ArgumentOutOfRangeException("Query shoul only select one field");
                        return dr.GetBoolean(0);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            return false;
        }
        #endregion


        #region SQL Methods

        public void InsertValues(string tableName, string[] values)
        {
            QuerySB.Append($"INSERT INTO {tableName} VALUES (");
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];
                char delimiter = i == values.Length - 1 ? ' ' : ',';
                QuerySB.Append($"{value}{delimiter}");
            }
            QuerySB.Append(");");

            string query = QuerySB.ToString();
            QuerySB.Clear();

            var executeMsg = Q(query);
            if (!string.IsNullOrEmpty(executeMsg)) Debug.LogError($"Error: {executeMsg} ; SQLQuery: {query}   ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <param name="colNames"></param>
        /// <param name="colValues"></param>
        /// <param name="conditionStr">add condition query, e.g. inventory.itemId = daily.itemId </param>
        /// <returns></returns>
        public void UpdateValues(string tableName, string[] colNames, string[] colValues, string conditionStr)
        {
            QuerySB.Append($"UPDATE {tableName} SET ");
            for (int i = 0; i < colNames.Length; i++)
            {
                string value = colNames[i];
                char delimiter = i == colNames.Length - 1 ? ' ' : ',';
                QuerySB.Append($"{colNames[i]} = {colValues[i]}{delimiter}");
            }

            QuerySB.Append($" WHERE {conditionStr} ;");
            string query = QuerySB.ToString();
            QuerySB.Clear();
            var executeMsg = Q(query);
            if (!string.IsNullOrEmpty(executeMsg)) Debug.LogError($"Error: {executeMsg} ; SQLQuery: {query}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <param name="conditionStr"> nventory.itemId = daily.itemId </param>
        /// <returns></returns>
        public int DeleteValue(string tableName, string conditionStr)
        {
            QuerySB.Append($"DELETE FROM {tableName} WHERE {conditionStr}");
            string query = QuerySB.ToString();
            QuerySB.Clear();

            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = query;
                try
                {
                    using (var dataReader = dbCommand.ExecuteReader())
                    {
                        return dataReader.FieldCount;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error: {e.Message} ; SQLQuery: {query}");

                }
            }
            return 0;
        }

        public void CreateOrOpenTable(string tableName, string[] colNames, string[] colTypes)
        {
            QuerySB.Append($"CREATE TABLE {tableName} (");
            for (int i = 0; i < colNames.Length; i++)
            {
                char delimiter = i == colNames.Length - 1 ? ' ' : ',';
                string primaryKey = i == 0 ? "PRIMARY KEY" : "";
                QuerySB.Append($"{colNames[i]} {colTypes[i]} {primaryKey} {delimiter} ");
            }
            QuerySB.Append(");");

            string query = QuerySB.ToString();
            QuerySB.Clear();

            var executeMsg = Q(query);
        }


        public void DropTable(string tableName)
        {
            QuerySB.Append($"DROP TABLE {tableName}");

            string query = QuerySB.ToString();
            QuerySB.Clear();

            var executeMsg = Q(query);
        }
        #endregion
        #endregion


    }


    public class SQLControllerExtension
    {

    }
}