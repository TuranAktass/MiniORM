using System.Linq.Expressions;
using System.Reflection;

namespace MiniORM.Query.ExpressionParser;

public class PlaygroundForExpression
{
    private Expression<Func<int, bool>> binary = (x => x > 0);
    private Expression<Func<int, bool>> member = (x => x > 0);
    private Expression<Func<int, bool>> constant = (x => x > 0);


    public int Test(BinaryExpression expression)
    {
        return 0;
    }
}