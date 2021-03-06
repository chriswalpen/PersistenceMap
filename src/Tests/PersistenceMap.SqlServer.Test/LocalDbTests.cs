﻿using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistenceMap.Test;
using PersistenceMap.Test.LocalDb;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;

namespace PersistenceMap.SqlServer.Test
{
    //[TestClass]
    public class LocalDbTests
    {
        private static LocalDbManager _localDbManager;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            _localDbManager = new LocalDbManager("Northwind");

            //var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");
            //string script = file.OpenText().ReadToEnd();
            //_localDbManager.ExecuteString(script);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _localDbManager.Dispose();
        }

        [TestMethod]
        public void TestWithLocalDbTest()
        {
            var provider = new SqlContextProvider(_localDbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");
                string script = file.OpenText().ReadToEnd();
                context.Execute(script);

                var query = context.From<Orders>().Map(o => o.OrdersID).Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID);

                var sql = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                var expected = query.CompileQuery();

                // check the compiled sql
                Assert.AreEqual(expected.Flatten(), sql);

                // execute the query
                var orders = query.Select();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }
    }
}
