﻿using PersistanceMap.Compiler;
using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public static class DatabaseContextExtensions
    {
        #region Select Expressions

        public static IEnumerable<T> Select<T>(this IDatabaseContext context)
        {
            var queryParts = new SelectQueryPartsMap();
            
            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Select);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            return context.Execute<T>(query);
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>();
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context, string alias)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>(alias);
        }

        public static ISelectQueryProvider<TJoin> From<T, TJoin>(this IDatabaseContext context, Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>()
                .Join<TJoin>(predicate);
        }

        public static IEnumerable<T> Select<T>(this IDatabaseContext context, string queryString)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            return context.Execute<T>(query);
        }

        #endregion

        #region Delete Expressions

        public static void Delete<T>(this IDatabaseContext context)
        {
            var queryParts = new SelectQueryPartsMap();

            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Delete);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            context.Execute(query);
        }

        public static void Delete<T>(this IDatabaseContext context, Expression<Func<T, bool>> predicate)
        {
            var queryParts = new SelectQueryPartsMap();

            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Delete);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            QueryPartsFactory.AppendExpressionQueryPart(queryParts, predicate, OperationType.Where);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            context.Execute(query);
        }

        #endregion

        #region Procedure Expressions

        public static IProcedureQueryProvider Procedure(this IDatabaseContext context, string procName)
        {
            return new ProcedureQueryProvider(context, procName);
        }

        #endregion
    }
}
