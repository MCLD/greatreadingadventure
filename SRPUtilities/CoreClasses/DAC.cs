using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

[assembly: CLSCompliant(true)]
namespace STG.CMS.Portal.Core
{
    public class DAC : IDisposable
    {
/*
        private String _defaultConnection = null;
*/
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        /// <summary>
        /// ////////////////////////////////////////////////////
        /// </summary>
        private ConnectionStringSettings _connectionSettings = null;
        private IDataReader _reader = null;
        private string _identifier = Guid.NewGuid().ToString();

        public DAC()
        {
            _connectionSettings = ConfigurationManager.ConnectionStrings["STG.CMS.DB"];
            _connection = CreateConnection();
            _connection.Open();
        }

        public DAC(string dataSourceName)
        {
            _connectionSettings = ConfigurationManager.ConnectionStrings[dataSourceName];
            _connection = CreateConnection();
            _connection.Open();
        }

        ~DAC()
        {
            if(_reader != null && _reader.IsClosed == false)
                _reader.Close();

            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }
            


        public string ConnectionString
        {
            get { return _connectionSettings.ConnectionString; }
            set { _connectionSettings.ConnectionString = value; }
        }


        public IDbConnection GetConnection()
        {

            if (_connectionSettings == null)
            {

            }
            IDbConnection connection = null;

            //Inspect the provider type and create an instance of the matching connection object.
            //Use Activator.CreateInstance for easy management.
            //If performance becomes an issue, replace this line with hard coded direct object type creation code.
            if (_connectionSettings != null)
                if (_connectionSettings.ProviderName.ToLower().Contains("sql"))
                { connection = new SqlConnection(_connectionSettings.ConnectionString); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("ole"))
                { connection = new OleDbConnection(_connectionSettings.ConnectionString); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("odbc"))
                { connection = new OdbcConnection(_connectionSettings.ConnectionString); }
                else
                { connection = new SqlConnection(_connectionSettings.ConnectionString); }
            return connection;
        }

        public IDbDataAdapter GetAdapter()
        {

            if (_connectionSettings == null)
            {

            }
            IDbDataAdapter adapter = null;

            //Inspect the provider type and create an instance of the matching connection object.
            //Use Activator.CreateInstance for easy management.
            //If performance becomes an issue, replace this line with hard coded direct object type creation code.
            if (_connectionSettings != null)
                if (_connectionSettings.ProviderName.ToLower().Contains("sql"))
                { adapter = new SqlDataAdapter(); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("ole"))
                { adapter = new OleDbDataAdapter(); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("odbc"))
                { adapter = new OdbcDataAdapter(); }
                else
                { adapter = new SqlDataAdapter(); }

            return adapter;

        }




        public IDataReader ExecuteReader(string storedProcedure, List<IDbDataParameter> parameters)
        {
            try
            {

                //Establish Connection
                IDbCommand dbCmd = InitializeCommandAndConnection(ref parameters, storedProcedure);
                _connection.Open();
                _reader = dbCmd.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (Exception ex)
            {
                throw new Exception("Error reading data.", ex);
            }

            return _reader;
        }

        public void ExecuteNonQuery(string storedProcedure, List<IDbDataParameter> parameters)
        {
            try
            {
                //Establish Connection
                IDbCommand dbCmd = InitializeCommandAndConnection(ref parameters, storedProcedure);
                _connection.Open();
                dbCmd.ExecuteNonQuery();
                _connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Error occured in performing the data command.", ex);
            }
        }



        public object ExecuteScalar(string storedProcedure, List<IDbDataParameter> parameters)
        {
            object returnValue;
            try
            {
                //Establish Connection
                IDbCommand dbCmd = InitializeCommandAndConnection(ref parameters, storedProcedure);

                _connection.Open();
                returnValue = dbCmd.ExecuteScalar();
                _connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Error communicating with data source.", ex);
            }
            return returnValue;
        }


        public void FillDataSet(string storedProcedure, List<IDbDataParameter> parameters, ref DataSet ds)
        {
            //Dataset specific initialization
            if (ds == null)
            {
                ds = new DataSet();
            }

            try
            {
                //Establish Connection
                IDbCommand dbCmd = InitializeCommandAndConnection(ref parameters, storedProcedure);
                IDbDataAdapter da = GetAdapter();
                da.SelectCommand = dbCmd;

                _connection.Open();
                da.Fill(ds);
                _connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured in getting the data.", ex);
            }
        }

        public void FillTypedDataSet(string storedProcedure, List<IDbDataParameter> parameters, ref DataSet ds, Type datasetType, ITableMappingCollection tableMapCollection)
        {
            //Dataset specific initialization
            if (ds == null)
            {
                ds = (DataSet)Activator.CreateInstance(datasetType);
            }
            try
            {
                //Establish Connection and command
                IDbCommand dbCmd = InitializeCommandAndConnection(ref parameters, storedProcedure);

                IDbDataAdapter da = GetAdapter();

                da.SelectCommand = dbCmd;
                foreach (ITableMapping tableMap in tableMapCollection)
                { da.TableMappings.Add(new System.Data.Common.DataTableMapping(tableMap.SourceTable, tableMap.DataSetTable)); }

                _connection.Open();
                da.Fill(ds);
                _connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting data.", ex);
            }
        }

        private IDbCommand InitializeCommandAndConnection(ref List<IDbDataParameter> parameters, string storedProcedure)
        {
            _connection = GetConnection();

            //Create command and load parameters
            IDbCommand dbCmd = _connection.CreateCommand();
            if (parameters != null)
                foreach (IDbDataParameter p in parameters)
                { dbCmd.Parameters.Add(p); }

            dbCmd.CommandText = storedProcedure;
            dbCmd.CommandType = CommandType.StoredProcedure;
            return dbCmd;
        }




        ////////////////////////////////////////////////////
        /// 
        /// 
        /// 
        /// 

        public IDbConnection CreateConnection()
        {
            return GetConnection();
        }

        public DataTable CreateDataTable(IDbCommand command)
        {
            DataTable dataTable = new DataTable();
            if (_connectionSettings != null)
                if (_connectionSettings.ProviderName.ToLower().Contains("sql"))
                { new SqlDataAdapter((SqlCommand)command).Fill(dataTable); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("ole"))
                { new OleDbDataAdapter((OleDbCommand)command).Fill(dataTable); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("odbc"))
                { new OdbcDataAdapter((OdbcCommand)command).Fill(dataTable); }
                else
                { return dataTable; }
            return dataTable;

        }

        public string CreateSqlParameterName(string paramName)
        {
            if (_connectionSettings != null)
                if (_connectionSettings.ProviderName.ToLower().Contains("sql"))
                { return String.Format("@{0}", paramName); }
                else if (_connectionSettings.ProviderName.ToLower().Contains("ole"))
                { return "?"; }
                else if (_connectionSettings.ProviderName.ToLower().Contains("odbc"))
                { return "?"; }
                else
                { return "?"; }
            return "?";
        }

        public string CreateCollectionParameterName(string baseParamName)
        {
            return String.Format("@{0}", baseParamName);
        }

//        protected internal virtual IDataReader ExecuteReader(IDbCommand command)
        public  virtual IDataReader ExecuteReader(IDbCommand command)
        {
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        //public virtual IDataReader ExecuteReader(IDbCommand command)
        //{
        //    if (command.Connection.State == ConnectionState.Closed) command.Connection.Open();
        //    return command.ExecuteReader(CommandBehavior.CloseConnection);
        //}

        public IDbDataParameter AddParameter(IDbCommand cmd, string paramName,
                                               DbType dbType, object value)
        {
            IDbDataParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = CreateCollectionParameterName(paramName);
            parameter.DbType = dbType;
            parameter.Value = null == value ? DBNull.Value : value;
            cmd.Parameters.Add(parameter);
            return parameter;
        }

        public IDbConnection Connection
        {
            get { return _connection; }
        }

        public IDbTransaction BeginTransaction()
        {
            CheckTransactionState(false);
            _transaction = _connection.BeginTransaction();
            return _transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            CheckTransactionState(false);
            _transaction = _connection.BeginTransaction(isolationLevel);
            return _transaction;
        }

        public void CommitTransaction()
        {
            CheckTransactionState(true);
            _transaction.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            CheckTransactionState(true);
            _transaction.Rollback();
            _transaction = null;
        }

        private void CheckTransactionState(bool mustBeOpen)
        {
            if (mustBeOpen)
            {
                if (null == _transaction)
                    throw new InvalidOperationException("Transaction is not open.");
            }
            else
            {
                if (null != _transaction)
                    throw new InvalidOperationException("Transaction is already open.");
            }
        }

        public IDbCommand CreateCommand(string sqlText)
        {
            return CreateCommand(sqlText, false);
        }

        public IDbCommand CreateCommand(string sqlText, bool procedure)
        {
            if (_connectionSettings == null)
            {
                _connectionSettings = ConfigurationManager.ConnectionStrings["STG.CMS.DB"];
                _connection = CreateConnection();
                _connection.Open();
            }
            if (_connection.State == ConnectionState.Closed)
            {
                _connection = CreateConnection();
                _connection.Open();
            } 
            IDbCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sqlText;
            cmd.Transaction = _transaction;
            if (procedure)
                cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }

        public virtual void Close()
        {
            if (_reader != null && _reader.IsClosed == false)
                _reader.Close();

            if (null != _connection)
                _connection.Close();
        }

        public virtual void Dispose()
        {
            Close();
            if (null != _connection)
                _connection.Dispose();
        }


    }
}
