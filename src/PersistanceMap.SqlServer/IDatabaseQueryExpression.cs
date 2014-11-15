﻿using System;
using System.Linq.Expressions;

namespace PersistanceMap.SqlServer
{
    public interface IDatabaseQueryExpression : IQueryExpression
    {
        void Create();

        ITableQueryExpression<T> Table<T>();
    }

    public interface ITableQueryExpression<T> : IQueryExpression
    {
        /// <summary>
        /// Create a create table expression
        /// </summary>
        void Create();

        /// <summary>
        /// Create a alter table expression
        /// </summary>
        void Alter();

        /// <summary>
        /// Creates a expression to rename a table
        /// </summary>
        /// <typeparam name="TNew">The type of the new table</typeparam>
        void RenameTo<TNew>();

        /// <summary>
        /// Drops the table
        /// </summary>
        void Drop();

        /// <summary>
        /// Ignore the field when creating the table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field);

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false);

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields);

        /// <summary>
        /// Marks a field to be a foreignkey column
        /// </summary>
        /// <typeparam name="TRef">The referenced table for the foreign key</typeparam>
        /// <param name="field">The foreign key field</param>
        /// <param name="reference">The key field in the referenced table</param>
        /// <returns></returns>
        ITableQueryExpression<T> ForeignKey<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference);

        ///// <summary>
        ///// Drops the key definition.
        ///// </summary>
        ///// <param name="keyFields">All items that make the key</param>
        ///// <returns></returns>
        //ITableQueryExpression<T> DropKey(params Expression<Func<T, object>>[] keyFields);

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="Column">The column to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        ITableQueryExpression<T> Column(Expression<Func<T, object>> field, FieldOperation operation, string precision = null, bool isNullable = true);
    }
}