using System.Linq.Expressions;
using System.Reflection;
using MiniORM.Helpers;
using MiniORM.Query.Model;

namespace MiniORM.Query.ExpressionParser;

public static class SelectExpressionParser
{
    public static List<ProjectionModel> Parse<T>(Expression<Func<T, object>> expression)
    {
        return expression.Body switch
        {
            NewExpression newExpression => ParseNewExpression(newExpression),
            MemberInitExpression mInitExp => ParseMemberInitExpression(mInitExp),
            _ => throw new NotSupportedException($"[Select Parser] : '{expression.Body.GetType()}' is not supported.")
        };

        // if (expression.Body is MemberInitExpression memberInitExpression)
        // {
        //     return memberInitExpression.Bindings.Select(b => b.Member.Name).ToList();
        //     // Console.WriteLine("YAAAY!" + memberInitExpression.Type);
        //     // Console.WriteLine("YAAAAY2! BINDINGS");
        //     // foreach (var item in memberInitExpression.Bindings)
        //     // {
        //     //     Console.WriteLine(item);
        //     // }
        //     //
        //     // Console.WriteLine("YAAAAY3! PROPERTIES");
        //     // foreach (var item in memberInitExpression.Type.GetProperties())
        //     // {
        //     //     Console.WriteLine(item);
        //     // }
        // }
    }

    private static List<ProjectionModel> ParseNewExpression(NewExpression newExpression)
    {
        var projections = new List<ProjectionModel>();
        for (int i = 0; i < newExpression.Members.Count; i++)
        {
            var member = newExpression.Members[i];
            var argument = newExpression.Arguments[i];

            if (argument is MemberExpression memberExpression)
            {
                projections.Add(new ProjectionModel
                {
                    TargetName = member.Name,
                    ColumnName = EntityMetaDataHelper.GetColumnName((PropertyInfo)memberExpression.Member),
                });
            }
            else
            {
                throw new NotSupportedException("[Select Parser] Only simple member access is supported.");
            }
        }

        return projections;
    }

    private static List<ProjectionModel> ParseMemberInitExpression(MemberInitExpression memberInitExpression)
    {
        var projections = new List<ProjectionModel>();

        foreach (var binding in memberInitExpression.Bindings)
        {
            if (binding is MemberAssignment assignment)
            {
                if (assignment.Expression is MemberExpression memberExpression)
                {
                    projections.Add(new ProjectionModel
                    {
                        TargetName = assignment.Member.Name,
                        ColumnName = EntityMetaDataHelper.GetColumnName((PropertyInfo)memberExpression.Member),
                    });
                }
            }
        }

        return projections;
    }
}