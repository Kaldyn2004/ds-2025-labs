using System.Diagnostics.CodeAnalysis;

namespace GeometryLib;

public sealed class Sphere3D
{
    public Sphere3D(Point3D center, double radius)
    {
        if (radius < 0)
        {
            throw new ArgumentException("The sphere's radius must be positive.");
        }

        if (radius == 0)
        {
            throw new ArgumentException("The sphere's radius cannot be zero.");
        }

        Center = center;
        Radius = radius;
    }

    /// <summary>
    ///  Координата центра шара.
    /// </summary>
    public Point3D Center { get; }

    /// <summary>
    ///  Радиус шара.
    /// </summary>
    public double Radius { get; }

    /// <summary>
    ///  Диаметр шара.
    /// </summary>
    public double Diameter => 2 * Radius;

    /// <summary>
    ///  Площадь поверхности шара.
    /// </summary>
    public double Area => 4 * Math.PI * Math.Pow(Radius, 2);

    /// <summary>
    /// Объем шара.
    /// </summary>
    public double Volume => (4.0 / 3.0) * Math.PI * Math.Pow(Radius, 3);

    /// <summary>
    /// Возвращающий расстояние от данной точки до ближайшей точки поверхности шара.
    /// </summary>
    public double DistanceTo(Point3D p)
    {
        double distanceToCenter = Center.DistanceTo(p);

        return Math.Abs(distanceToCenter - Radius);
    }

    /// <summary>
    /// возвращающий расстояние между ближайшими друг к другу точками поверхностей двух шаров (двух сфер).
    /// </summary>
    public double DistanceTo(Sphere3D p)
    {
        double centerDistance = Center.DistanceTo(p.Center);

        if (Contains(p))
        {
            return Radius - (centerDistance + p.Radius);
        }
        else if (p.Contains(this))
        {
            return p.Radius - (p.Center.DistanceTo(Center) + Radius);
        }
        else
        {
            return Math.Max(0, centerDistance - Radius - p.Radius);
        }
    }

    /// <summary>
    /// проверка, лежит ли точка внутри шара.
    /// </summary>
    public bool Contains(Point3D p)
    {
        return Radius > Center.DistanceTo(p);
    }

    /// <summary>
    /// проверка, пересекаются ли два шара.
    /// </summary>
    public bool IntersectsWith(Sphere3D other)
    {
        double distanceBeetweenCenters = Center.DistanceTo(other.Center);

        return distanceBeetweenCenters <= Radius + other.Radius;
    }

    /// <summary>
    /// проверка, лежит ли другой шар полностью внутри этого шара.
    /// </summary>
    public bool Contains(Sphere3D other)
    {
        double distanceBeetweenCenters = Center.DistanceTo(other.Center);

        return distanceBeetweenCenters + other.Radius < Radius;
    }
}