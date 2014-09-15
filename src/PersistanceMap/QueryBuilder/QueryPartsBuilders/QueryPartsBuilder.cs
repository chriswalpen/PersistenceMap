﻿using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    public class QueryPartsBuilder
    {
        protected QueryPartsBuilder()
        {
        }

        protected IQueryPart AppendSimpleQueryPart(IQueryPartsMap queryParts, OperationType operation)
        {
            var part = new SimpleQueryPart(operation);

            queryParts.Add(part);

            return part;
        }

        protected IQueryPart AppendQueryPart(IQueryPartsMap queryParts, OperationType operation, Func<string> predicate)
        {
            var part = new PredicateQueryPart(operation, predicate);

            queryParts.Add(part);

            return part;
        }

        internal EntityQueryPart<T> AppendEntityQueryPart<T>(IQueryPartsMap queryParts, OperationType operation)
        {
            var type = typeof(T);
            var entity = new EntityQueryPart<T>(type.Name)
            {
                OperationType = operation
            };

            queryParts.Add(entity);

            return entity;
        }

        internal EntityQueryPart<T> AppendEntityQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T, bool>> predicate, OperationType operation)
        {
            var expression = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { expression }, operation);
        }

        internal EntityQueryPart<T> AppendEntityQueryPart<T, T2>(IQueryPartsMap queryParts, Expression<Func<T, T2, bool>> predicate, OperationType operation)
        {
            var expression = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { expression }, operation);
        }

        internal EntityQueryPart<T> AppendEntityQueryPart<T>(IQueryPartsMap queryParts, IExpressionQueryPart[] parts, OperationType maptype)
        {
            var operationParts = parts.Where(p => p.OperationType == OperationType.On || p.OperationType == OperationType.And || p.OperationType == OperationType.Or).ToArray();

            var type = typeof(T);
            var entity = new EntityQueryPart<T>(type.Name, null, operationParts)
            {
                OperationType = maptype
            };

            queryParts.Add(entity);

            return entity;
        }

        protected IExpressionQueryPart AppendExpressionQueryPart(IQueryPartsMap queryParts, LambdaExpression predicate, OperationType operation)
        {
            var part = new ExpressionQueryPart(operation, predicate);
            queryParts.Add(part);

            return part;
        }

        protected IFieldQueryPart AppendFieldQueryMap(IQueryPartsMap queryParts, string field, string alias, string entity, string entityalias)
        {
            var part = new FieldQueryPart(field, alias, null /*EntityAlias*/, entity)
            {
                OperationType = OperationType.Include
            };

            queryParts.Add(part);

            return part;
        }

        internal static void AddFiedlParts(SelectQueryPartsMap queryParts, IFieldQueryPart[] fields)
        {
            foreach (var part in queryParts.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                foreach (var field in fields)
                {
                    if (map.Parts.Any(f => f is IFieldQueryPart && ((IFieldQueryPart)f).Field == field.Field || ((IFieldQueryPart)f).FieldAlias == field.Field))
                        continue;

                    map.Add(field);
                }
            }
        }
    }
}