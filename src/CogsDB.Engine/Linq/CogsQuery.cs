using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CogsDB.Engine.Linq
{
    public class CogsQuery<T>: IOrderedQueryable<T>
    {
        private CogsQueryProvider _provider;
        private Expression _expression;

        public CogsQuery(CogsQueryProvider provider)
        {
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public CogsQuery(CogsQueryProvider provider, Expression expression)
        {
            _provider = provider;
            _expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public Type ElementType
        {
            get { return typeof (T); }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }
    }
}
