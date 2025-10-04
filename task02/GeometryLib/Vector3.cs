using System.Diagnostics.CodeAnalysis;

namespace GeometryLib;

public readonly struct Vector3(double x, double y, double z)
    : IEquatable<Vector3>
{
    // Максимальное отклонение, при котором координаты всё ещё считаются равными.
    public const double Tolerance = 1e-10;

    // Количество знаков после запятой у максимального отклонения.
    public const int Precision = 10;

    /// <summary>
    ///  Координата по оси Ox.
    /// </summary>
    public double X { get; } = x;

    /// <summary>
    ///  Координата по оси Oy.
    /// </summary>
    public double Y { get; } = y;

    /// <summary>
    ///  Координата по оси Oz.
    /// </summary>
    public double Z { get; } = z;

    /// <summary>
    ///  Длина вектора (то есть модуль вектора).
    /// </summary>
    public double Length => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

    public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);

    public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);

    /// <summary>
    ///  Нормализует вектор, возвращая вектор с тем же направлением и длиной 1.
    /// </summary>
    public Vector3 Normalize()
    {
        double length = Length;
        if (length <= Tolerance)
        {
            throw new InvalidOperationException("Cannot normalize vector with zero length");
        }

        return new Vector3(X / length, Y / length, Z / length);
    }

    /// <summary>
    ///  Складывает два вектора.
    /// </summary>
    public Vector3 Add(Vector3 o)
    {
        return new Vector3(X + o.X, Y + o.Y, Z + o.Z);
    }

    /// <summary>
    ///  Возвращает скалярное произведение векторов.
    /// </summary>
    public double DotProduct(Vector3 o)
    {
        return X * o.X + Y * o.Y + Z * o.Z;
    }

    /// <summary>
    ///  Проверяет, являются ли вектора ортогональными.
    /// </summary>
    public static bool AreOrthogonal(Vector3 x, Vector3 y)
    {
        return Math.Abs(x.DotProduct(y)) <= Tolerance;
    }

    /// <summary>
    ///  Производит проекцию текущего вектора на указанный вектор.
    /// </summary>
    public Vector3 Project(Vector3 o)
    {
        double oLengthSquared = o.DotProduct(o);
        if (oLengthSquared < Tolerance)
        {
            return new Vector3(0, 0, 0);
        }

        double scale = DotProduct(o) / oLengthSquared;
        return new Vector3(o.X * scale, o.Y * scale, o.Z * scale);
    }

    /// <summary>
    ///  Проверяет равенство двух векторов.
    /// </summary>
    public bool Equals(Vector3 other)
    {
        return Math.Abs(X - other.X) < Tolerance
               && Math.Abs(Y - other.Y) < Tolerance
               && Math.Abs(Z - other.Z) < Tolerance;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Vector3 other)
        {
            return Equals(other);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (X, Y, Z).GetHashCode();
    }

    /// <summary>
    ///  Возвращает строковое представление текущего вектора в формате (x, y, z).
    /// </summary>
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}