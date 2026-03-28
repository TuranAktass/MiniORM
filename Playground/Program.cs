using MiniORM.Core;
using MiniORM.Infrastructure;
using MiniORM.Logging;
using Playground;
using Playground.Tests;


SQLitePCL.Batteries.Init();


var connectionString = "Data Source=miniorm.db";


var context = new OrmContext(
    connectionString,
    new ConsoleOrmLogger()
);
ProjectionTests.Run(context);