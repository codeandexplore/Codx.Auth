using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Codx.Auth.Extensions
{
    /// <summary>
    /// Extensions for IQueryable to support dynamic sorting and filtering
    /// </summary>
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> query, 
            string propertyName, 
            string direction = "asc")
        {
            if (string.IsNullOrEmpty(propertyName))
                return query;

            // Get property info using reflection
            var propertyInfo = typeof(T).GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null)
                return query;

            // Create expression parameter
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            // Create method name based on sort direction
            string methodName = direction.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";

            // Create generic method
            var orderByMethod = typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                .Where(m => m.GetParameters().Length == 2)
                .Single();

            var genericMethod = orderByMethod.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);

            // Apply ordering to query
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda });
        }

        public static IQueryable<T> ApplySearch<T>(
            this IQueryable<T> query,
            string searchTerm,
            params string[] propertyNames)
        {
            if (string.IsNullOrEmpty(searchTerm) || propertyNames.Length == 0)
                return query;

            // Create parameter for the lambda expression
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression combinedExpression = null;

            foreach (var propertyName in propertyNames)
            {
                // Get the property using reflection
                var property = typeof(T).GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null || property.PropertyType != typeof(string))
                    continue;

                // Create expression for the property
                var propertyAccess = Expression.Property(parameter, property);
                var searchConstant = Expression.Constant(searchTerm, typeof(string));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                
                // Create expression for Contains check
                var containsExpression = Expression.Call(propertyAccess, containsMethod, searchConstant);
                
                // Combine with OR expressions
                combinedExpression = combinedExpression == null
                    ? containsExpression
                    : Expression.OrElse(combinedExpression, containsExpression);
            }

            if (combinedExpression == null)
                return query;

            // Create the lambda expression
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            
            // Apply the filter
            return query.Where(lambda);
        }
    }
}
