using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ImIn
{
    class DBConnection
    {
        private MySqlConnection connection;

        // Set up the database object ==================================================================================
        /// <summary>
        /// Set up the database object
        /// </summary>
        /// <param name="server"> The server name to connect to </param>
        /// <param name="dbName"> The name of the database on the server </param>
        /// <param name="user"> The username to access the database </param>
        /// <param name="password"> The password accociated with the username </param>
        public DBConnection(string server, string dbName, string user, string password)
        {
            Initialize(server, dbName, user, password);
        }

        public DBConnection()
        {
            Initialize("162.241.24.176", "cammymcn_restaurantsuite", "cammymcn_ImInAdm", "ImInPassword");
        }

        /// <summary>
        /// Initialize the connection to the database using the information passed when creating the object
        /// </summary>
        /// <param name="server"> The server name/IP to connect to </param>
        /// <param name="dbName"> The name of the database on the server </param>
        /// <param name="user"> The username to access the database </param>
        /// <param name="password"> The password accociated with the username </param>
        private void Initialize(string server, string dbName, string user, string password)
        {            
            // Create connection string with the required data
            string connectionString = "SERVER=" + server + ";" 
                                    + "DATABASE=" + dbName + ";" 
                                    + "UID=" + user + ";" 
                                    + "PASSWORD=" + password + ";";

            //Console.WriteLine("Connection made with string: " + connectionString);

            // Set the connection to a new instance
            connection = new MySqlConnection(connectionString);

            // Test the cconnection by opening and closing the connection
            if (OpenConnection())
                if (CloseConnection())
                    Console.WriteLine("Connection Succesful");
        }

        /// <summary>
        /// Tries to open the database connection to allow for commands to be run on it
        /// </summary>
        /// <returns> True if the connection is successful, false if it fails </returns>
        private bool OpenConnection()
        {
            try
            {
                // Attempt to open the connection and return result
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                // Check the error number and display appropriate message
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.");
                        break;

                    case 1045:
                        Console.WriteLine("Incorrect username/password, please try again");
                        break;
                }

                return false;
            }
        }
        
        /// <summary>
        /// Tries to close the database connection.
        /// </summary>
        /// <returns> True if the connection closes successfully, false if something goes wrong </returns>
        private bool CloseConnection()
        {
            try
            {
                // Attempt to close the connection and return the result
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                // Display the error message
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Checks to see if the connection with the database is open or not
        /// </summary>
        /// <returns> True if the connection is open, false if closed </returns>
        public bool IsOpen()
        {
            // Return the current state of the database connection
            if (connection.State == System.Data.ConnectionState.Open) { return true; };
            return false;
        }


        // Misc Functions ==============================================================================================
        /// <summary>
        /// Check is a string entered is valid
        /// This limits entered values to text only, preventing sql injection.
        /// All user input should be run through this.
        /// </summary>
        /// <param name="input"> The user input string </param>
        /// <returns> Boolean true if the string is valid, false if not. </returns>
        private bool CheckStringValid(string input)
        {
            bool valid = false;

            foreach (char j in input)
            {
                int i = (int)j;
                if (i <= 122 && i >= 65)
                {
                    if (i <= 90 || i >= 97)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }
                else
                {
                    valid = false;
                    break;
                }
            }

            return valid;
        }

        /// <summary>
        /// Get the column names for the table specified
        /// </summary>
        /// <param name="table_name"> Name of the table to get column names from </param>
        /// <returns> List of strings containing the column names </returns>
        public List<string>[] GetColumnNames(string table_name)
        {
            string queryGetColumns = "SHOW COLUMNS FROM " + table_name;
            List<string>[] list;         

            if (OpenConnection())
            {
                try
                {
                    MySqlDataReader dataReader = new MySqlCommand(queryGetColumns, connection).ExecuteReader();

                    if (dataReader.Read())
                    {
                        list = new List<string>[dataReader.FieldCount];
                        for (int i = 0; i < list.Length; i++)
                        {
                            list[i] = new List<string>();
                        }

                        int j = 0;
                        while (dataReader.Read())
                        {
                            list[j].Add(dataReader[0].ToString());
                            j++;
                        }

                        dataReader.Close();
                        CloseConnection();
                        return list;
                    } else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    list = new List<string>[1];
                    list[0] = new List<string>();
                    list[0].Add(ex.Message);
                    CloseConnection();
                    return list;
                }
            } else
            {
                return null;
            }
        }


        // Run Select Query ============================================================================================
        /// <summary>
        /// Run a command on the database
        /// </summary>
        /// <param name="command"> The command to be run </param>
        /// <returns> True if successful, false if fails </returns>
        private bool RunCommand(string command)
        {
            try
            {
                connection.Open();
                MySqlCommand comm = connection.CreateCommand();
                comm.CommandText = command;
                comm.ExecuteNonQuery();

                connection.Close();

                Console.WriteLine("Completed Successfully");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED");
                Console.WriteLine(e.Message);
                connection.Close();
                return false;
            }
        }


        // Run Select Query ============================================================================================
        /// <summary>
        /// Select specific data from a table
        /// </summary>
        /// <param name="table_name"> The name of the table to access </param>
        /// <param name="fields"> Array of column headers to get data from </param>
        /// <returns> 2-Dimentional list of data retrieved from the database </returns>
        public List<string>[] Select(string table_name, string[] fields)
        {
            if (CheckStringValid(table_name))
                return RunSelectQuery(SelectQueryBuilder(table_name, fields), fields.Length);
            return null;
        }

        /// <summary>
        /// Run SELECT query on database
        /// </summary>
        /// <param name="query"> The complete query to run </param>
        /// <returns> 2-Dimentional list of data retrieved from the database </returns>
        public List<string>[] Select(string query)
        {
            string query_check = query.ToLower();
            int count = 0, pos = 0;
            bool star = false;

            if (query_check.Contains("select") && query_check.Contains("from"))
            {
                string[] elements = query.Split(' ');

                if (elements.Contains("*"))
                {
                    star = true;
                }

                foreach (string s in elements)
                {
                    pos++;
                    if (s.ToLower() == "from") break;
                    if (s.ToLower() != "select") count++;
                }

                if (star)
                {
                    string queryGetColumns = "SELECT COUNT(*) FROM information_schema.columns WHERE table_name='" + elements.ElementAt(pos).ToString() + "'";

                    if (OpenConnection())
                    {
                        try
                        {
                            MySqlDataReader dataReader = new MySqlCommand(queryGetColumns, connection).ExecuteReader();
                            if (dataReader.Read()) count = int.Parse(dataReader[0].ToString());

                            dataReader.Close();
                            CloseConnection();
                        } catch (Exception)
                        {
                            count = 1;
                        }
                    }
                }
            }
            
            return RunSelectQuery(query, count);
        }

        /// <summary>
        /// Run the select query passed in.
        /// </summary>
        /// <param name="query"> The string containing the query. (Can be hand written or made using the query builder) </param>
        /// <param name="size"> The number of columns that the query will return </param>
        /// <returns></returns>
        private List<string>[] RunSelectQuery(string query, int size)
        {
            List<string>[] list = new List<string>[size];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = new List<string>();
            }

            if (OpenConnection())
            {
                try
                {
                    MySqlDataReader dataReader = new MySqlCommand(query, connection).ExecuteReader();

                    list = new List<string>[dataReader.FieldCount];
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        list[i] = new List<string>();
                    }


                    while (dataReader.Read())
                    {
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            list[i].Add(dataReader[i] + " ");
                        }
                    }

                    dataReader.Close();

                    CloseConnection();

                    return list;
                }
                catch (Exception ex)
                {
                    list = new List<string>[1];
                    list[0] = new List<string>();
                    list[0].Add(ex.Message);
                    CloseConnection();
                    return list;
                }
            }
            else
            {
                return list;
            }
        }


        // Select Query Builders =======================================================================================
        /// <summary>
        /// Build a SELECT query using the column headers and the table name
        /// </summary>
        /// <param name="table_name"> The table to be queried </param>
        /// <param name="fields"> Array of strings containing the column headers required </param>
        /// <returns> A string containing the completed SELECT statement </returns>
        public string SelectQueryBuilder(string table_name, string[] fields)
        {
            // Start select statement
            string output = "SELECT ";

            if (fields.Contains("*") || fields.Contains("ALL"))
            {
                output += "*";
            }
            else
            {
                // Add each of the fields to the statement
                foreach (string s in fields)
                {
                    output += s + ", ";
                }

                // Remove the last comma
                output = output.Substring(0, output.Length - 2);
            }

            // Add the table to select from
            output += " FROM " + table_name;

            // Return completed select statement
            return output;
        }

        /// <summary>
        /// Build a SELECT query using a single column and the table name
        /// </summary>
        /// <param name="table_name"> The table to be queried </param>
        /// <param name="field"> A string containing the column header required </param>
        /// <returns> A string containing the completed SELECT statement </returns>
        public string SelectQueryBuilder(string table_name, string field)
        {
            // Start select statement
            string output = "SELECT ";

            // Add each of the fields to the statement
            output += field + ", ";

            // Remove the last comma
            output = output.Substring(0, output.Length - 2);

            // Add the table to select from
            output += " FROM " + table_name;

            // Return completed select statement
            return output;
        }

        /// <summary>
        /// Build a SELECT query using the column headers, the table name with a WHERE clause
        /// </summary>
        /// <param name="table_name"> The table to be queried </param>
        /// <param name="fields"> Array of strings containing the column headers required </param>
        /// <param name="where"> A string containing the WHERE of the query </param>
        /// <returns> A string containing the completed SELECT statement </returns>
        public string SelectQueryBuilder(string table_name, string[] fields, string where)
        {
            // Create a where statement with the builder, then concatonate that with the where statement
            // Return the result
            return SelectQueryBuilder(table_name, fields) + " WHERE " + where;
        }

        /// <summary>
        /// Build a SELECT query using the column headers, the table name with a WHERE clause and GROUP BY a field
        /// </summary>
        /// <param name="table_name"> The table to be queried </param>
        /// <param name="fields"> Array of strings containing the column headers required </param>
        /// <param name="where"> A string containing the WHERE of the query </param>
        /// <param name="groupField"> The field header to group the data by </param>
        /// <returns> A string containing the completed SELECT statement </returns>
        public string SelectQueryBuilder(string table_name, string[] fields, string where, string groupField)
        {
            // Create a where statement with the builder, then concatonate that with the where statement
            // Return the result
            return SelectQueryBuilder(table_name, fields, where) + " GROUP BY " + groupField;
        }

        /// <summary>
        /// Build a SELECT query using the column headers, the table name with a WHERE clause and GROUP BY a field
        /// </summary>
        /// <param name="table_name"> The table to be queried </param>
        /// <param name="fields"> Array of strings containing the column headers required </param>
        /// <param name="where"> A string containing the WHERE of the query </param>
        /// <param name="groupField"> The field header to group the data by </param>
        /// <param name="ASC"> Boolean value determining if the output is sorted in acending or decending order </param>
        /// <returns> A string containing the completed SELECT statement </returns>
        public string SelectQueryBuilder(string table_name, string[] fields, string where, string groupField, bool ASC)
        {
            if (ASC)
                return SelectQueryBuilder(table_name, fields, where, groupField) + " ASC";
            else
                return SelectQueryBuilder(table_name, fields, where, groupField) + " DESC";
        }

        /// <summary>
        /// Build a SELECT query using the column headers, the table name with a WHERE clause and GROUP BY a field
        /// </summary>
        /// <param name="table_name"> The table to be queried </param>
        /// <param name="fields"> Array of strings containing the column headers required </param>
        /// <param name="groupField"> The field header to group the data by </param>
        /// <param name="ASC"> Boolean value determining if the output is sorted in acending or decending order </param>
        /// <returns> A string containing the completed SELECT statement </returns>
        public string SelectQueryBuilder(string table_name, string[] fields, string groupField, bool ASC)
        {
            string output = SelectQueryBuilder(table_name, fields) + " GROUP BY " + groupField;

            if (ASC)
                return output + " ACS";
            else
                return output + " DESC";
        }

        
        // Run Insert ==================================================================================================
        /// <summary>
        /// Runs an insert command on the database
        /// </summary>
        /// <param name="query"> A string containing the insert command </param>
        /// <returns> True if successful, false if it fails </returns>
        public bool Insert(string query)
        {
            return RunCommand(query);
        }

        
        // Insert Builders =============================================================================================
        /// <summary>
        /// Creates an INSERT statement, for a table, from the column names and values given.
        /// </summary>
        /// <param name="table_name"> Name of the table to be inserted into </param>
        /// <param name="fields"> Array of column names to be added to </param>
        /// <param name="values"> Array of values to be added to the database, MUST be in the same order as column names </param>
        /// <returns> A completed insert command as a string </returns>
        public string InsertBuilder(string table_name, string[] fields, string[] values)
        {
            string output = "INSERT INTO " + table_name + "(";

            foreach (string s in fields)
            {
                output += s + ", ";
            }

            output = output.Substring(0, output.Length - 2);

            output += ") VALUES('";

            foreach (string s in values)
            {
                output += s + "', '";
            }

            output = output.Substring(0, output.Length - 3);

            output += ")";

            return output;
        }

        /// <summary>
        /// Creates an INSERT statement, for a table, from the values given.
        /// NOTE: This can only be used if ALL the columns are being added
        /// </summary>
        /// <param name="table_name"> Name of the table to be inserted into </param>
        /// <param name="values"> Array of values to be added to database </param>
        /// <returns> A completed insert command as a string </returns>
        public string InsertBuilder(string table_name, string[] values)
        {
            string output = "INSERT INTO " + table_name;
            
            output += " VALUES('";

            foreach (string s in values)
            {
                output += s + "', '";
            }

            output = output.Substring(0, output.Length - 3);

            output += ")";

            return output;
        }

        
        // Run Update ==================================================================================================
        /// <summary>
        /// Runs an update command on the database
        /// </summary>
        /// <param name="query"> A string containing the update command </param>
        /// <returns> True if successful, false if it fails </returns>
        public bool Update(string query)
        {
            return RunCommand(query);
        }

        
        // Update Builders =============================================================================================
        /// <summary>
        /// Create an update command using table name, fields and values
        /// NOTE: ONLY use this if you want to update EVERY ROW
        /// </summary>
        /// <param name="table_name"> The table to be updated </param>
        /// <param name="fields"> The fields to be edited </param>
        /// <param name="values"> The values to update the edited fields to </param>
        /// <returns> A string of the completed statement </returns>
        public string UpdateBuilder(string table_name, string[] fields, string[] values)
        {
            string output = "UPDATE " + table_name + " SET ";

            for (int i = 0; i < fields.Length; i++)
            {
                output += fields[i] + "='" + values[i] + "', ";
            }

            output = output.Substring(0, output.Length - 2);

            return output;
        }

        /// <summary>
        /// Create an update command using table name, fields, values and a where clause
        /// </summary>
        /// <param name="table_name"> The table to be updated </param>
        /// <param name="fields"> The fields to be edited </param>
        /// <param name="values"> The values to update the edited fields to </param>
        /// <param name="where"> Where command to specify which rows to update </param>
        /// <returns> A string of the completed statement </returns>
        public string UpdateBuilder(string table_name, string[] fields, string[] values, string where)
        {
            return UpdateBuilder(table_name, fields, values) + " WHERE " + where;
        }

        
        // Run Deletion ================================================================================================
        /// <summary>
        /// Runs a delete command on the database
        /// </summary>
        /// <param name="query"> A string containing the delete command </param>
        /// <returns> True if successful, false if it fails </returns>
        public bool Delete(string query)
        {
            return RunCommand(query);
        }

        
        // Delete Builders =============================================================================================
        /// <summary>
        /// Create a delete command on a table
        /// NOTE: ONLY use this if you want to delete EVERY ROW
        /// </summary>
        /// <param name="table_name"> The table to be deleted </param>
        /// <returns> A string of the completed statement </returns>
        public string DeleteBuilder(string table_name)
        {
            return "DELETE FROM " + table_name;
        }

        /// <summary>
        /// Create a delete command using table name and a where clause
        /// </summary>
        /// <param name="table_name"> The table to be deleted from </param>
        /// <param name="where"> Where command to specify which rows to delete </param>
        /// <returns> A string of the completed statement </returns>
        public string DeleteBuilder(string table_name, string where)
        {
            return DeleteBuilder(table_name) + " WHERE " + where;
        }

    }
}
