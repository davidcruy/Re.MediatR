namespace ReMediatr;

internal static class AssignableExtensions
{
    /// <summary>
    /// Determines whether the <paramref name="genericType"/> is assignable from
    /// <paramref name="givenType"/> taking into account generic definitions
    /// </summary>
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        return givenType == genericType
               || givenType.MapsToGenericTypeDefinition(genericType)
               || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
               || (givenType.BaseType?.IsAssignableToGenericType(genericType) ?? false);
    }

    private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type givenType, Type genericType)
    {
        return givenType
            .GetInterfaces()
            .Where(it => it.IsGenericType)
            .Any(it => it.GetGenericTypeDefinition() == genericType);
    }

    private static bool MapsToGenericTypeDefinition(this Type givenType, Type genericType)
    {
        return genericType.IsGenericTypeDefinition
               && givenType.IsGenericType
               && givenType.GetGenericTypeDefinition() == genericType;
    }
}