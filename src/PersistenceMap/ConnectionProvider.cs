﻿using System;
using System.Data;

namespace PersistenceMap
{
    public abstract class ConnectionProvider : IConnectionProvider
    {
        private readonly Func<string, IDbConnection> _connectionFactory;
        private string _connectionString;
        private ConnectionStringBuilder _connectionStringBuilder;
        private IQueryCompiler _queryCompiler;

        public ConnectionProvider(string connectionString, Func<string, IDbConnection> connectionFactory)
        {
            // format the string
            _connectionString = connectionString;
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// The connectionstring
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                return ConnectionStringBuilder.FormatConnectionString(_connectionString);
            }
            private set
            {
                _connectionString = value;
            }
        }

        internal ConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                if (_connectionStringBuilder == null)
                {
                    _connectionStringBuilder = new ConnectionStringBuilder();
                }

                return _connectionStringBuilder;
            }
        }

        /// <summary>
        /// The name of the database
        /// </summary>
        public string Database
        {
            get
            {
                return ConnectionStringBuilder.GetDatabase(ConnectionString);
            }
            set
            {
                // set new database name
                ConnectionString = ConnectionStringBuilder.SetDatabase(value, ConnectionString);
            }
        }

        /// <summary>
        /// The querycompiler that is needed to compiel a querypartscontainer to a sql statement
        /// </summary>
        public virtual IQueryCompiler QueryCompiler
        {
            get
            {
                if (_queryCompiler == null)
                {
                    _queryCompiler = new QueryCompiler();
                }

                return _queryCompiler;
            }
            protected set
            {
                _queryCompiler = value;
            }
        }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IDataReaderContext Execute(string query)
        {
            var connection = _connectionFactory(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Connection = connection;

            return new DataReaderContext(command.ExecuteReader(), connection, command);
        }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The amount of afected rows</returns>
        public virtual int ExecuteNonQuery(string query)
        {
            using (var connection = _connectionFactory(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    IsDisposed = true;
                }
            }
        }

        #endregion
    }
}
