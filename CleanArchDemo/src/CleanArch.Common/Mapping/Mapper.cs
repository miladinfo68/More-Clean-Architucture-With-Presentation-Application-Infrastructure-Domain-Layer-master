

using System.Linq.Expressions;
using System.Reflection;

namespace CleanArch.Common.Mapping;

public static class MapperExtention
{

    // reflection --> object to object
    public static TU Mapper<T, TU>(this T source)
        where T : class
        where TU : class, new()
    {
        var sourceType = typeof(T);
        var destinationType = typeof(TU);

        var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var destination = new TU();

        foreach (var sourceProperty in sourceProperties)
        {
            var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);

            if (destinationProperty != null && destinationProperty.CanWrite)
            {
                var sourceValue = sourceProperty.GetValue(source);
                var destinationValue = Convert.ChangeType(sourceValue, destinationProperty.PropertyType);
                destinationProperty.SetValue(destination, destinationValue);
            }
        }

        return destination;
    }


    //reflection --> list to list
    public static IEnumerable<TU> Mapper2<T, TU>(this IEnumerable<T> source)
    where T : class
    where TU : class, new()
    {
        var sourceType = typeof(T);
        var destinationType = typeof(TU);

        var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceItem in source)
        {
            var destination = new TU();

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);

                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    var sourceValue = sourceProperty.GetValue(sourceItem);
                    var destinationValue = Convert.ChangeType(sourceValue, destinationProperty.PropertyType);
                    destinationProperty.SetValue(destination, destinationValue);
                }
            }

            yield return destination;
        }
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


    //class to record by Expression   faster than reflection --> object to object
    public static TU Mapper3<T, TU>(this T source)
    {
        // Get the PropertyInfo objects for the properties in T and U
        var tProperties = typeof(T).GetProperties();
        var uProperties = typeof(TU).GetProperties();

        // Create the parameter expression for the source object
        var parameter = Expression.Parameter(typeof(T), "source");

        // Create the property bindings for U based on the properties in T
        var propertyBindings = uProperties.Select(uProperty =>
        {
            var tProperty = tProperties.FirstOrDefault(p => p.Name.ToLower() == uProperty.Name.ToLower() && p.PropertyType == uProperty.PropertyType);
            if (tProperty != null)
            {
                // Create the property access expression for the source property
                var propertyAccess = Expression.Property(parameter, tProperty);

                // Check if the source property value is the default value
                var isDefaultValue = Expression.Equal(propertyAccess, Expression.Default(tProperty.PropertyType));

                // Create the condition to skip property assignment if it's the default value
                var condition = Expression.IfThen(isDefaultValue, Expression.Return(Expression.Label(), Expression.Default(uProperty.PropertyType)));

                // Convert the property access to the target property type
                var convertedValue = Expression.Convert(propertyAccess, uProperty.PropertyType);

                // Create the binding expression for the target property with the condition
                return Expression.Bind(uProperty, Expression.Block(condition, convertedValue));
            }
            return null;
        }).Where(binding => binding != null);

        // Create the MemberInit expression for creating U object
        var memberInit = Expression.MemberInit(Expression.New(typeof(TU)), propertyBindings);

        // Create the lambda expression
        var lambda = Expression.Lambda<Func<T, TU>>(memberInit, parameter);

        // Compile the lambda expression
        var converter = lambda.Compile();

        // Convert the source object of type T to U using the compiled converter
        return converter(source);
    }




    //class to record by Expression  --> list to list

    public static IEnumerable<TU> Mapper4<T, TU>(this IEnumerable<T> source)
    {
        // Get the PropertyInfo objects for the properties in T and U
        var tProperties = typeof(T).GetProperties();
        var uProperties = typeof(TU).GetProperties();

        // Create the parameter expression for the source item
        var parameter = Expression.Parameter(typeof(T), "item");

        // Create the property bindings for U based on the properties in T
        var propertyBindings = uProperties.Select(uProperty =>
        {
            var tProperty = tProperties.FirstOrDefault(p => p.Name == uProperty.Name && p.PropertyType == uProperty.PropertyType);
            if (tProperty != null)
            {
                Expression propertyAccess;

                // Special handling for DateTime properties
                //if (tProperty.PropertyType == typeof(DateTime) && uProperty.PropertyType == typeof(DateTime))
                //{
                //    // Use Expression.Convert to cast the DateTime to nullable DateTime
                //    propertyAccess = Expression.Convert(Expression.Property(parameter, tProperty), typeof(DateTime?));
                //}
                //else
                //{
                    propertyAccess = Expression.Property(parameter, tProperty);
                //}

                // Convert the property access to the target property type
                var convertedValue = Expression.Convert(propertyAccess, uProperty.PropertyType);

                // Create the binding expression for the target property
                return Expression.Bind(uProperty, convertedValue);
            }
            return null;
        }).Where(binding => binding != null);

        // Create the MemberInit expression for creating U objects
        var memberInit = Expression.MemberInit(Expression.New(typeof(TU)), propertyBindings);

        // Create the lambda expression
        var lambda = Expression.Lambda<Func<T, TU>>(memberInit, parameter);

        // Compile the lambda expression
        var converter = lambda.Compile();

        // Convert each item in the source IEnumerable to U using the compiled converter
        return source.Select(converter);
    }



}


















