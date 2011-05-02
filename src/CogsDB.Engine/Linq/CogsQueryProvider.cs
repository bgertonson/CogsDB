using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CogsDB.Engine.Linq
{
    public class CogsQueryProvider: IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(CogsQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new CogsQuery<T>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return new QueryTranslator().Translate(expression);
        }

        public T Execute<T>(Expression expression)
        {
            bool IsEnumerable = (typeof(T).Name == "IEnumerable`1");
            var q = new QueryTranslator().Translate(expression);
            return default(T);
        }
    }
}
