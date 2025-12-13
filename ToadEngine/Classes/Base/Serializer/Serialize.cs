using System.Collections;
using System.Reflection;
using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Serializer;

public class SerializedValue
{
    public required Type Type;
    public object? Value;
    public Dictionary<string, SerializedValue>? Fields;
    public List<SerializedValue>? Elements;
}

public class Serialize
{
    private static bool IsPrimitive(Type type)
    {
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(Vector2)
               || type == typeof(Vector3)
               || type == typeof(Vector4);
    }

    public static SerializedValue Object(object obj, HashSet<object>? visited = null)
    {
        visited ??= new HashSet<object>(ReferenceEqualityComparer.Instance);

        var type = obj.GetType();

        if (!type.IsValueType && !visited.Add(obj)) return new SerializedValue { Type = type };
        if (IsPrimitive(type))
            return new SerializedValue
            {
                Type = type,
                Value = obj
            };

        if (obj is IEnumerable enumerable && type != typeof(string))
        {
            var elements = (from object item in enumerable
                select item == null
                    ? new SerializedValue { Type = typeof(object) }
                    : Object(item, visited)).ToList();

            return new SerializedValue
            {
                Type = type,
                Elements = elements
            };
        }

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var dict = new Dictionary<string, SerializedValue>();

        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            if (value == null) continue;

            dict[field.Name] = Object(value, visited);
        }

        return new SerializedValue
        {
            Type = type,
            Fields = dict
        };
    }

    public static void ApplySerializedValue(object target, SerializedValue data)
    {
        var type = target.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (data.Fields == null) continue;
            if (!data.Fields.TryGetValue(field.Name, out var serialized)) continue;

            var fieldType = field.FieldType;
            if (IsPrimitive(fieldType))
            {
                field.SetValue(target, serialized.Value);
                continue;
            }

            if (typeof(IList).IsAssignableFrom(fieldType))
            {
                var list = (IList?)field.GetValue(target);
                if (list == null || serialized.Elements == null) continue;

                list.Clear();
                var elementType = fieldType.GetGenericArguments()[0];

                foreach (var element in serialized.Elements)
                {
                    var instance = Activator.CreateInstance(elementType)!;
                    ApplySerializedValue(instance, element);
                    list.Add(instance);
                }
                continue;
            }

            var obj = field.GetValue(target);
            if (obj == null)
            {
                obj = Activator.CreateInstance(fieldType)!;
                field.SetValue(target, obj);
            }

            ApplySerializedValue(obj, serialized);
        }
    }

    public static void DumpGameObjectSerializableTree(GameObject gameObject, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        writer.WriteLine($"GameObject: {gameObject.Name}");
        writer.WriteLine("{");

        DumpSerializableObject(gameObject.Transform, writer, visited, indent: 1, "Transform");

        foreach (var component in gameObject.Components)
            DumpSerializableObject(component, writer, visited, indent: 1, component.GetType().Name);

        writer.WriteLine("}");
    }

    private static void DumpSerializableObject(object obj, StreamWriter writer, 
        HashSet<object> visited, int indent, string name)
    {
        var pad = new string(' ', indent * 2);
        var type = obj.GetType();

        if (!type.IsValueType && !visited.Add(obj))
        {
            writer.WriteLine($"{pad}{name} : {type.Name} (CYCLE)");
            return;
        }

        if (IsPrimitive(type))
        {
            writer.WriteLine($"{pad}{name} : {type.Name} = {obj}");
            return;
        }

        if (obj is IEnumerable enumerable && type != typeof(string))
        {
            writer.WriteLine($"{pad}{name} : {type.Name}");
            writer.WriteLine($"{pad}[");

            var i = 0;
            foreach (var item in enumerable)
            {
                if (item == null)
                {
                    writer.WriteLine($"{pad}  [{i}] = null");
                    i++;
                    continue;
                }

                DumpSerializableObject(item, writer, visited, indent: indent + 1, $"[{i}]");
                i++;
            }

            writer.WriteLine($"{pad}]");
            return;
        }

        writer.WriteLine($"{pad}{name} : {type.Name}");
        writer.WriteLine($"{pad}{{");

        var fields = type.GetFields(
            BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            if (value == null)
            {
                writer.WriteLine($"{pad}  {field.Name} : {field.FieldType.Name} = null");
                continue;
            }
            DumpSerializableObject(value, writer, visited, indent: indent + 1, field.Name);
        }

        writer.WriteLine($"{pad}}}");
    }
}
