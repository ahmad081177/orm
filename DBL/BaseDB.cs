using System.Data.Common;

namespace DBL
{
    public abstract class BaseDB<T> : DB
    {
        private const string WHERE_KW = "WHERE ";
        private const string AND = "AND";
       
        protected abstract string GetTableName();
        protected abstract T GetRowByPK(object pk);
        protected abstract Task<T> GetRowByPKAsync(object pk);
        protected abstract T CreateModel(object[] row);
        protected abstract Task<T> CreateModelAsync(object[] row);
        protected abstract List<T> CreateListModel(List<object[]> rows);
        protected abstract Task<List<T>> CreateListModelAsync(List<object[]> rows);

        public List<T> SelectAll()
        {
            return SelectAll("", new Dictionary<string, string>());
        }
        public List<T> SelectAll(Dictionary<string, string> parameters)
        {
            return SelectAll("", parameters);
        }
        public List<T> SelectAll(string query)
        {
            return SelectAll(query, new Dictionary<string, string>());
        }
        public List<T> SelectAll(string query, Dictionary<string, string> parameters)
        {
            List<object[]> list = StingListSelectAll(query, parameters);
            return CreateListModel(list);
        }
        protected List<object[]> StingListSelectAll(string query, Dictionary<string, string> parameters)
        {
            List<object[]> list = new List<object[]>();

            string where = ParamsToWhereQuery(parameters);

            string sqlCommand = $"{query} {where}";
            if (String.IsNullOrEmpty(query))
                sqlCommand = $"SELECT * FROM {GetTableName()} {where}";

            PreQuery(sqlCommand);

            try
            {
                reader = cmd.ExecuteReader();
                int size = reader.GetColumnSchema().Count;
                object[] row;
                while (reader.Read())
                {
                    row = new object[size];
                    reader.GetValues(row);
                    list.Add(row);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nsql:" + cmd.CommandText);
                list.Clear();
            }
            finally
            {
                PostQuery();
            }
            return list;
        }

        //for ExecuteNonQuery
        protected int ExecNonQuery(string query)
        {
            if (String.IsNullOrEmpty(query))
                return 0;

            PreQuery(query);

            int rowsEffected = 0;
            try
            {
                rowsEffected = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nsql:" + cmd.CommandText);
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                PostQuery();
            }
            return rowsEffected;
        }

        // exeScalar
        public object? ExecScalar(string query)
        {
            if (String.IsNullOrEmpty(query))
                return null;

            PreQuery(query);
            object obj = null;
            try
            {
                obj = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nsql:" + cmd.CommandText);
            }
            finally
            {
                PostQuery();
            }
            return obj;
        }

        // Dictionary<string, string> FildValue - ערכים של שדות
        // return -  מספר שדות שעודכנו
        protected int Insert(Dictionary<string, string> keyAndValue)
        {
            if (keyAndValue == null || keyAndValue.Count == 0)
                return 0;
            string InKey, InValue;
            PrepareInKeyAndValueToInsertQuery(keyAndValue, out InKey, out InValue);

            string sqlCommand = $"INSERT INTO {GetTableName()}  {InKey} {InValue}";
            return ExecNonQuery(sqlCommand);
        }

        protected object InsertGetObj(Dictionary<string, string> keyAndValue)
        {
            if (keyAndValue != null && keyAndValue.Count > 0)
            {
                string InKey, InValue;
                PrepareInKeyAndValueToInsertQuery(keyAndValue, out InKey, out InValue);

                string sqlCommand = $"INSERT INTO {GetTableName()}  {InKey} {InValue};" +
                                    $"SELECT LAST_INSERT_ID();";
                object res = ExecScalar(sqlCommand);
                if (res != null)
                {
                    //TODO: Why thses casting?
                    ulong qkwl = (ulong)res;
                    int Id = (int)qkwl;
                    return GetRowByPK(Id);
                }
            }
            return null;
        }


        // Dictionary<string, string> FildValue - ערכים של שדות
        // Dictionary<string, string> parameters - תנאים לעדכון
        // return -  מספר שדות שעודכנו

        protected int Update(Dictionary<string, string> keyAndValue, Dictionary<string, string> parameters)
        {
            if (keyAndValue == null || keyAndValue.Count == 0)
                return 0;

            string InKeyValue = PrepareKeysEqValuesInQuery(keyAndValue);
            string where = ParamsToWhereQuery(parameters);

            string sqlCommand = $"UPDATE {GetTableName()} SET {InKeyValue}  {where}";
            return ExecNonQuery(sqlCommand);
        }

        // Dictionary<string, string> parameters - תנאים לעדכון
        // return -  מספר שדות שעודכנו

        protected int Delete(Dictionary<string, string> parameters)
        {
            string where = ParamsToWhereQuery(parameters);

            string sqlCommand = $"DELETE FROM {GetTableName()} {where}";
            return ExecNonQuery(sqlCommand);
        }


        public async Task<List<T>> SelectAllAsync()
        {
            return await SelectAllAsync("", new Dictionary<string, string>());
        }

        public async Task<List<T>> SelectAllAsync(Dictionary<string, string> parameters)
        {
            return await SelectAllAsync("", parameters);
        }
        public async Task<List<T>> SelectAllAsync(string query)
        {
            return await SelectAllAsync(query, new Dictionary<string, string>());
        }
        public async Task<List<T>> SelectAllAsync(string query, Dictionary<string, string> parameters)
        {
            List<object[]> list = await StingListSelectAllAsync(query, parameters);
            return CreateListModel(list);
        }

        protected async Task<List<object[]>> StingListSelectAllAsync(string query, Dictionary<string, string> parameters)
        {
            List<object[]> list = new List<object[]>();
            string where = ParamsToWhereQuery(parameters);

            string sqlCommand = $"{query} {where}";
            if (String.IsNullOrEmpty(query))
                sqlCommand = $"SELECT * FROM {GetTableName()} {where}";

            PreQuery(sqlCommand);

            try
            {
                reader = await cmd.ExecuteReaderAsync();
                var readOnlyData = await reader.GetColumnSchemaAsync();
                int size = readOnlyData.Count;
                object[] row;
                while (reader.Read())
                {
                    row = new object[size];
                    reader.GetValues(row);
                    list.Add(row);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nsql:" + cmd.CommandText);
                list.Clear();
            }
            finally
            {
                PostQuery();
            }
            return list;
        }

        // exeNONquery
        protected async Task<int> ExecNonQueryAsync(string query)
        {
            if (String.IsNullOrEmpty(query))
                return 0;
            PreQuery(query);
            int rowsEffected = 0;
            try
            {
                rowsEffected = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nsql:" + cmd.CommandText);
            }
            finally
            {
                PostQuery();
            }
            return rowsEffected;
        }

        // exeScalar
        protected async Task<object> ExecScalarAsync(string query)
        {
            if (String.IsNullOrEmpty(query))
                return null;
            PreQuery(query);
            object obj = null;
            try
            {
                obj = await cmd.ExecuteScalarAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nsql:" + cmd.CommandText);
            }
            finally
            {
                PostQuery();
            }
            return obj;
        }

        
        // Dictionary<string, string> FildValue - ערכים של שדות
        // return -  מספר שדות שעודכנו
        protected async Task<int> InsertAsync(Dictionary<string, string> keyAndValue)
        {
            if (keyAndValue == null || keyAndValue.Count == 0)
                return 0;

            string InKey, InValue;
            PrepareInKeyAndValueToInsertQuery(keyAndValue, out InKey, out InValue);

            string sqlCommand = $"INSERT INTO {GetTableName()}  {InKey} {InValue}";
            return await ExecNonQueryAsync(sqlCommand);
        }

        protected object InsertGetObjAsync(Dictionary<string, string> keyAndValue)
        {
            if (keyAndValue == null || keyAndValue.Count == 0)
                return null;

            string InKey, InValue;
            PrepareInKeyAndValueToInsertQuery(keyAndValue, out InKey, out InValue);

            string sqlCommand = $"INSERT INTO {GetTableName()}  {InKey} {InValue};" +
                                $"SELECT LAST_INSERT_ID();";
            object res = ExecScalarAsync(sqlCommand);
            if (res != null)
            {
                //TODO: why all these castings?
                ulong qkwl = (ulong)res;
                int Id = (int)qkwl;
                return GetRowByPK(Id);
            }
            else
                return null;
        }


        // Dictionary<string, string> FildValue - ערכים של שדות
        // Dictionary<string, string> parameters - תנאים לעדכון
        // return -  מספר שדות שעודכנו

        protected async Task<int> UpdateAsync(Dictionary<string, string> FildValue, Dictionary<string, string> parameters)
        {
            string where = ParamsToWhereQuery(parameters);

            string InKeyValue = PrepareKeysEqValuesInQuery(FildValue);
            if (string.IsNullOrEmpty(InKeyValue))
                return 0;

            string sqlCommand = $"UPDATE {GetTableName()} SET {InKeyValue}  {where}";
            return await ExecNonQueryAsync(sqlCommand);
        }

        // Dictionary<string, string> parameters - תנאים לעדכון
        // return -  מספר שדות שעודכנו

        protected async Task<int> DeleteAsync(Dictionary<string, string> parameters)
        {
            string where = ParamsToWhereQuery(parameters);

            string sqlCommand = $"DELETE FROM {GetTableName()} {where}";
            return await ExecNonQueryAsync(sqlCommand);
        }

        protected void AddParameterToCommand(string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = "@"+name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }
        /// <summary>
        /// Prepare command and Connection before executing SQL command
        /// </summary>
        /// <param name="sqlCommandStr"></param>
        private void PreQuery(string sqlCommandStr)
        {
            cmd.CommandText = sqlCommandStr;
            if (DB.conn.State != System.Data.ConnectionState.Open)
                DB.conn.Open();
            if (cmd.Connection?.State != System.Data.ConnectionState.Open)
                cmd.Connection = DB.conn;
        }
        
        /// <summary>
        /// Make cleanup after sql command was executed
        /// </summary>
        private void PostQuery()
        {
            if(reader!=null && !reader.IsClosed)
                reader?.Close();

            cmd.Parameters.Clear();
            if (DB.conn.State == System.Data.ConnectionState.Open)
                DB.conn.Close();
        }

        /// <summary>
        /// Generate the List of keys and values as params for Insert SQL Query
        /// </summary>
        /// <param name="keyAndValue">Field name and value as map</param>
        /// <param name="InKey">Field names</param>
        /// <param name="InValue">Field values</param>
        /// <example>if <paramref name="keyAndValue"/> is ('Name','Jhon') then <paramref name="InKey"/>will be ('Name')
        /// and <paramref name="InValue"/>will be "VALUES(@0)" and will add Param(@0, 'Jhon') to the command.</example>
        private void PrepareInKeyAndValueToInsertQuery(Dictionary<string, string> keyAndValue, out string InKey, out string InValue)
        {
            InKey = $"(" + string.Join(",", keyAndValue.Keys) + ")";
            //add param values
            InValue = "VALUES(";
            for (int i = 0; i < keyAndValue.Values.Count; i++)
            {
                string pn = "@" + i;
                InValue += pn + ',';
                AddParameterToCommand(pn, keyAndValue.Values.ElementAt(i));
            }
            InValue = InValue.Remove(InValue.Length - 1);//remove last ,
            InValue += ")";
        }

        /// <summary>
        /// Prepare SQL Where closure from the given paremeters dictionary
        /// </summary>
        /// <param name="parameters">Key & Value</param>
        /// <example>Where p1=v1 AND p2=v2</example>
        /// <returns>String of SQL Where closure</returns>
        private string ParamsToWhereQuery(Dictionary<string, string> parameters)
        {
            string where = BaseDB<T>.WHERE_KW;

            if (parameters != null && parameters.Count > 0)
            {
                int i = 0;
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    where += $"{param.Key} = @{i} {AND} ";
                    AddParameterToCommand("@" + i, param.Value);
                    i++;
                }
                where = where.Remove(where.Length - AND.Length-2);//remove last ' AND '
            }
            else
                where = "";
            return where;
        }
        /// <summary>
        /// Extract keys and values from the dictionary and prepare a string of k1=v1,k2=v2
        /// to be part of a query
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private string PrepareKeysEqValuesInQuery(Dictionary<string, string> fields)
        {
            string InKeyValue = "";
            if (fields != null && fields.Count > 0)
            {
                int i = 0;
                foreach (KeyValuePair<string, string> param in fields)
                {
                    InKeyValue += $"{param.Key} = @{i},";
                    AddParameterToCommand("@" + i, param.Value);
                    i++;
                }
                InKeyValue = InKeyValue.Remove(InKeyValue.Length - 1); //remove last ,
            }
            return InKeyValue;
        }
    }
}