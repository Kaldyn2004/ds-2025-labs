using System.Diagnostics.CodeAnalysis;

namespace GeometryLib;

public readonly struct Point3D(double x, double y, double z)
    : IEquatable<Point3D>
{
    // Максимальное отклонение, при котором координаты считаются равными.
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

    public static bool operator ==(Point3D left, Point3D right) => left.Equals(right);

    public static bool operator !=(Point3D left, Point3D right) => !(left == right);

    /// <summary>
    ///  Растояние до другой точки
    /// </summary>
    public double DistanceTo(Point3D otherPoint)
    {
        double dx = X - otherPoint.X;
        double dy = Y - otherPoint.Y;
        double dz = Z - otherPoint.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    /// <summary>
    ///  Проверяет равенство двух точек.
    /// </summary>
    public bool Equals(Point3D other)
    {
        return Math.Abs(X - other.X) < Tolerance
               && Math.Abs(Y - other.Y) < Tolerance
               && Math.Abs(Z - other.Z) < Tolerance;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Point3D other)
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
    ///  Возвращает строковое представление текущей точкив формате (x, y, z).
    /// </summary>
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}