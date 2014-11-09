﻿using System;

namespace PersistanceMap
{
    public interface IConnectionProvider : IDisposable
    {
        //string ConnectionString { get; }

        IQueryCompiler QueryCompiler { get; }

        IReaderContext Execute(string query);

        IReaderContext ExecuteNonQuery(string query);
    }
}
