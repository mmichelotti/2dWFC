using System;
using System.Numerics;

#region interfaces
public interface IDimension { int Size { get; } }
public readonly struct D1 : IDimension { public readonly int Size => 1; }
public readonly struct D2 : IDimension { public readonly int Size => 2; }
public readonly struct D3 : IDimension { public readonly int Size => 3; }
public readonly struct D4 : IDimension { public readonly int Size => 4; }
#endregion

public struct Vector<D, T> : IFormattable 
    where D : IDimension, new() 
    where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
{
    private T[] components;
    public T X
    {
        get
        {
            return HasDimension(0) ? components[0] : default;
        }
        set
        {
            EnsureDimension(1);
            components[0] = value;
        }
    }

    public T Y
    {
        get
        {
            return HasDimension(1) ? components[1] : default;
        }
        set
        {
            EnsureDimension(2);
            components[1] = value;
        }
    }

    public T Z
    {
        get
        {
            return HasDimension(2) ? components[2] : default;
        }
        set
        {
            EnsureDimension(3);
            components[2] = value;
        }
    }

    public T W
    {
        get
        {
            return HasDimension(3) ? components[3] : default;
        }
        set
        {
            EnsureDimension(4);
            components[3] = value;
        }
    }

    public T this[int index] { get => components[index]; set => components[index] = value; }

    public Vector(params T[] values)
    {
        int size = new D().Size;
        if (values.Length > size) throw new ArgumentException($"Cant be bigger of {size}");
        components = new T[size];
        Array.Copy(values, components, values.Length);
    }

    public Vector(T value)
    {
        int size = new D().Size;
        components = new T[size];
        for (int i = 0; i < size; i++) components[i] = value;
    }




    private bool HasDimension(int requiredDimension) => new D().Size > requiredDimension;
    private void EnsureDimension(int requiredDimension)
    {
        if (!HasDimension(requiredDimension))
        {
            throw new InvalidOperationException($"Vector dimension {new D().Size} does not support this property.");
        }
    }
    #region operators
    public Vector2 ToVector2()
    {
        if (typeof(D) != typeof(D2) || typeof(T) != typeof(float))
        {
            throw new InvalidOperationException("This method can only be used for Vector<D2, float>.");
        }
        return new Vector2(Convert.ToSingle(X), Convert.ToSingle(Y));
    }



    public static Vector<D, T> operator +(Vector<D, T> a, Vector<D, T> b) => AddSameDimensions(a, b);
    public static Vector<D, T> operator +(Vector<D, T> a, object b)
    {
        return b switch
        {
            Vector<D1, T> v1 => AddDifferentDimensions(a, v1),
            Vector<D2, T> v2 => AddDifferentDimensions(a, v2),
            Vector<D3, T> v3 => AddDifferentDimensions(a, v3),
            Vector<D4, T> v4 => AddDifferentDimensions(a, v4),
            _ => throw new ArgumentException("The second operand is not a compatible Vector.")
        };
    }
    #endregion
    private static Vector<D, T> AddSameDimensions(Vector<D, T> a, Vector<D, T> b)
    {
        int size = new D().Size;
        T[] resultComponents = new T[size];

        for (int i = 0; i < size; i++) resultComponents[i] = Add(a[i], b[i]);

        return new(resultComponents);
    }
    private static Vector<D, T> AddDifferentDimensions<V>(Vector<D, T> a, Vector<V, T> b) where V : IDimension, new()
    {
        int maxSize = Math.Max(new D().Size, new V().Size);
        T[] resultComponents = new T[maxSize];

        for (int i = 0; i < maxSize; i++)
        {
            T aValue = i < a.components.Length ? a[i] : default;
            T bValue = i < b.components.Length ? b[i] : default;
            resultComponents[i] = Add(aValue, bValue);
        }

        return new(resultComponents);
    }
    private static T Add(T a, T b)
    {
        return a switch
        {
            int ai when b is int bi => (T)(object)(ai + bi),
            float af when b is float bf => (T)(object)(af + bf),
            double ad when b is double bd => (T)(object)(ad + bd),
            // Add more type checks and operations as needed
            _ => throw new InvalidOperationException("Unsupported type")
        };
    }
    public readonly string ToString(string format, IFormatProvider formatProvider) => this.ToString();
    public readonly override string ToString() => $"Vector<{typeof(D).Name}, {typeof(T).Name}>({string.Join(", ", components)})";
}
