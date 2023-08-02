namespace MinimalApiEndpoints.Extensions;

public static class TypeExtensions
{
    public static bool IsSubclassOfOpenGeneric(this Type typeToCheck, Type baseType)
    {
        while (typeToCheck != typeof(object))
        {
            var currentType = 
                typeToCheck is { IsGenericType: true } ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
            
            if (baseType == currentType)
                return true;

            if (typeToCheck.BaseType is null)
                return false;

            typeToCheck = typeToCheck.BaseType;
            
        }
     
        return false;
    }
}