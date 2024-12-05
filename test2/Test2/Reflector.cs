// <copyright file="Reflector.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>
namespace ReflectorSpace;

using System.Reflection;

/// <summary>
/// Reflector class.
/// </summary>
public class Reflector()
{
    /// <summary>
    /// creates a file named <class name>.cs. It describes the class <class name> with all fields, methods.
    /// </summary>
    /// <param name="someClass">Class to be described.</param>
    /// <exception cref="ArgumentException">Throws if someClass is not a class.</exception>
    public void PrintStructure(Type someClass)
    {
        if (!someClass.IsClass)
        {
            throw new ArgumentException("Is not a class.");
        }

        string className = someClass.Name;
        string modifiers = GetClassVisibility(someClass) + GetClassStatic(someClass);
        string fileName = $"{className}.cs";

        using StreamWriter writer = new StreamWriter(fileName);
        var interfaces = someClass.GetInterfaces();
        string interfaceList = interfaces.Length > 0 ? $" : {string.Join(", ", interfaces.Select(i => i.Name))}" : string.Empty;
        writer.WriteLine($"{modifiers}class {className}{interfaceList}");
        writer.WriteLine("{");

        var fields = someClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var field in fields)
        {
            writer.WriteLine($"    {GetVisibility(field)} {GetStatic(field)}{GetTypeName(field.FieldType)} {field.Name};");
        }

        var methods = someClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName && m.DeclaringType == someClass); // Исключаем методы свойств
        foreach (var method in methods)
        {
            writer.WriteLine($"    {GetVisibility(method)} {GetStatic(method)}{GetTypeName(method.ReturnType)} {method.Name}({GetParameters(method)}) {{ }}");
        }

        writer.WriteLine("}");
    }

    /// <summary>
    /// Outputs all fields and methods that differ in the two classes.
    /// </summary>
    /// <param name="a">First class.</param>
    /// <param name="b">Second class.</param>
    /// <exception cref="ArgumentException">Throws when not a class given.</exception>
    public void DiffClasses(Type a, Type b)
    {
        if (!a.IsClass || !b.IsClass)
        {
            throw new ArgumentException("Both types must be classes.");
        }

        var fieldsA = a.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        var fieldsB = b.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        var methodsA = a.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName && m.DeclaringType == a);
        var methodsB = b.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName && m.DeclaringType == b);

        CompareFields(fieldsA, fieldsB);
        CompareMethods(methodsA.ToArray(), methodsB.ToArray());
    }

    private static string GetVisibility(MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            return field.IsPublic ? "public" : field.IsFamily ? "protected" : field.IsAssembly ? "internal" : "private";
        }
        else if (member is MethodInfo method)
        {
            return method.IsPublic ? "public" : method.IsFamily ? "protected" : method.IsAssembly ? "internal" : "private";
        }

        return string.Empty;
    }

    private static string GetStatic(MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            return field.IsStatic ? "static " : string.Empty;
        }
        else if (member is MethodInfo method)
        {
            return method.IsStatic ? "static " : string.Empty;
        }

        return string.Empty;
    }

    private static string GetParameters(MethodBase method)
    {
        var parameters = method.GetParameters();
        return string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
    }

    private static string GetClassStatic(Type type)
    {
        return type.IsAbstract && type.IsSealed ? "static " : string.Empty;
    }

    private static string GetClassVisibility(Type type)
    {
        if (type.IsPublic)
        {
            return "public ";
        }
        else if (type.IsNotPublic)
        {
            return "private ";
        }
        else if (type.IsNestedFamily)
        {
            return "protected ";
        }
        else if (type.IsNestedAssembly)
        {
            return "internal ";
        }

        return string.Empty;
    }

    private static string GetTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            string baseType = type.GetGenericTypeDefinition().Name;
            string genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
            return $"{baseType[..baseType.LastIndexOf('`')]}<{genericArgs}>";
        }

        return type.Name;
    }

    private static void CompareFields(FieldInfo[] fieldsA, FieldInfo[] fieldsB)
    {
        var fieldNamesA = fieldsA.Select(f => f.Name).ToHashSet();
        var fieldNamesB = fieldsB.Select(f => f.Name).ToHashSet();

        foreach (var field in fieldNamesA.Except(fieldNamesB))
        {
            Console.WriteLine($"Field '{field}' exists in class A but not in class B.");
        }

        foreach (var field in fieldNamesB.Except(fieldNamesA))
        {
            Console.WriteLine($"Field '{field}' exists in class B but not in class A.");
        }
    }

    private static bool AreMethodsEquivalent(MethodInfo methodA, MethodInfo methodB)
    {
        return methodA.ReturnType == methodB.ReturnType &&
               methodA.GetParameters().Select(p => p.ParameterType).SequenceEqual(methodB.GetParameters().Select(p => p.ParameterType));
    }

    private static void CompareMethods(MethodInfo[] methodsA, MethodInfo[] methodsB)
    {
        var methodNamesA = methodsA.Select(m => m.Name).ToHashSet();
        var methodNamesB = methodsB.Select(m => m.Name).ToHashSet();

        foreach (var method in methodNamesA.Except(methodNamesB))
        {
            Console.WriteLine($"Method '{method}' exists in class A but not in class B.");
        }

        foreach (var method in methodNamesB.Except(methodNamesA))
        {
            Console.WriteLine($"Method '{method}' exists in class B but not in class A.");
        }

        foreach (var methodA in methodsA)
        {
            if (methodNamesB.Contains(methodA.Name))
            {
                var methodB = methodsB.First(m => m.Name == methodA.Name);
                if (!AreMethodsEquivalent(methodA, methodB))
                {
                    Console.WriteLine($"Method '{methodA.Name}' differs between classes.");
                }
            }
        }
    }
}