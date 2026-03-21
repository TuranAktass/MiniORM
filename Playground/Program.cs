using System.Linq.Expressions;
using MiniORM.Core;
using Microsoft.Data.Sqlite;
using MiniORM.Infrastructure;
using MiniORM.Query.ExpressionParser;
using Playground;


SQLitePCL.Batteries.Init();


var connectionString = "Data Source=miniorm.db";


var context = new OrmContext(connectionString, cs => new DbExecutor(cs));

UnaryOperatorTests.Run(context);