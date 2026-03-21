using System.Linq.Expressions;
using MiniORM.Core;
using Microsoft.Data.Sqlite;
using MiniORM.Infrastructure;
using MiniORM.Query.ExpressionParser;
using Playground;
using Playground.Tests;


SQLitePCL.Batteries.Init();


var connectionString = "Data Source=miniorm.db";


var context = new OrmContext(connectionString, cs => new DbExecutor(cs));

CountTest.Run(context);