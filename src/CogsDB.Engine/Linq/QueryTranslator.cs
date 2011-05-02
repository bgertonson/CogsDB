using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CogsDB.Engine.Linq
{

    public class QueryBuilder
    {
        public string Index { get; set; }
        public int Count { get; set; }
    }
    public class QueryTranslator: ExpressionVisitor
    {
        private QueryBuilder _query = new QueryBuilder();

        internal QueryTranslator()
        {
        }

        internal object Translate(Expression expression)
        {
            Visit(expression);
            return _query;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if(m.Method.Name == "First")
            {
                _query.Count = 1;
                Visit(m.Arguments[0]);
                return m;
            }

            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                // sb.Append("SELECT * FROM (");
                // get the index to search
                this.Visit(m.Arguments[0]);

                // sb.Append(") AS T WHERE ");
                // get the value
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    //sb.Append(" AND ");
                    break;
                case ExpressionType.Equal:
                    //sb.Append(" = ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                _query.Index = q.ElementType.Name;
            }
            else if (c.Value == null)
            {
                //sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        //sb.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
//                        sb.Append("'");
//                        sb.Append(c.Value);
//                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                    default:
//                        sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
//                sb.Append(m.Member.Name);
                return m;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }
    }
}
