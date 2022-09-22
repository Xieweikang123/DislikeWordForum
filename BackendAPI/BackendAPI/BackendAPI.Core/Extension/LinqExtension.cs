using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Core
{
    public static class LinqExtension
    {

        /// <summary>
        /// 根据指定属性名字去orderby
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderByProperty"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc)
        {

            BindingFlags flag = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance;
            //var propertyInfo = typeof(EnglishWord).GetProperty(dto.prop, flag);

            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty, flag);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
        //private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        //{
        //    MyExpressionVisitor visitor = new MyExpressionVisitor(first.Parameters[0]);
        //    Expression bodyone = visitor.Visit(first.Body);
        //    Expression bodytwo = visitor.Visit(second.Body);
        //    return Expression.Lambda<T>(merge(bodyone, bodytwo), first.Parameters[0]);
        //}
        //public static Expression<Func<T, bool>> ExpressionAnd<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        //{
        //    return first.Combine(second, Expression.And);
        //}
        //public static Expression<Func<T, bool>> ExpressionOr<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        //{
        //    return first.Combine(second, Expression.Or);
        //}

    }

    //public class MyExpressionVisitor : ExpressionVisitor
    //{
    //    public ParameterExpression _Parameter { get; set; }

    //    public MyExpressionVisitor(ParameterExpression Parameter)
    //    {
    //        _Parameter = Parameter;
    //    }
    //    protected override Expression VisitParameter(ParameterExpression p)
    //    {
    //        return _Parameter;
    //    }

    //    public override Expression Visit(Expression node)
    //    {
    //        return base.Visit(node);//Visit会根据VisitParameter()方法返回的Expression修改这里的node变量
    //    }
    //}
}
