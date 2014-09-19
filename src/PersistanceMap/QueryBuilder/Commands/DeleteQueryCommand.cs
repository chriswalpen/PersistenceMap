﻿
namespace PersistanceMap.QueryBuilder.Commands
{
    internal class DeleteQueryCommand : IQueryCommand
    {
        public DeleteQueryCommand(IQueryPartsMap map)
        {
            QueryPartsMap = map;
        }

        public IQueryPartsMap QueryPartsMap { get; private set; }

        public void Execute(IDatabaseContext context)
        {
            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);
            context.Execute(query);
        }
    }
}