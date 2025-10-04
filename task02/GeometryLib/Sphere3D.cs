using System.Diagnostics.CodeAnalysis;

namespace GeometryLib;

public sealed class Sphere3D
{
    public Sphere3D(Point3D center, double radius)
    {
        if (radius < 0)
        {
            throw new ArgumentException("Радиус шара должен быть положительным.");
        }

        Center = center;
        this.Radius = radius;
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
    ///  Площадь поверхности шара
    /// </summary>
    public double Area => 4 * Math.PI * Math.Pow(Radius, 2);

    /// <summary>
    /// Объем шара
    /// </summary>
    public double Volume => 4 / 3 * Math.PI * Math.Pow(Radius, 3);

    /// <summary>
    /// Возвращающий расстояние от данной точки до ближайшей точки поверхности шара.
    /// </summary>
    public double DistanceTo(Point3D p)
    {
        if (Contains(p))
        {
            return Radius - Center.DistanceTo(p);
        }
        else
        {
            return Center.DistanceTo(p) - Radius;
        }
    }

    /// <summary>
    /// возвращающий расстояние между ближайшими друг к другу точками поверхностей двух шаров (двух сфер).
    /// </summary>
    public double DistanceTo(Sphere3D p)
    {
        if (Contains(p))
        {
            return Radius - (Center.DistanceTo(p.Center) + p.Radius);
        }
        else if (IntersectsWith(p))
        {
            return 0;
        }
        else
        {
            return Center.DistanceTo(p.Center) - Radius - p.Radius;
        }
    }

    /// <summary>
    /// проверка, лежит ли точка внутри шара
    /// </summary>
    public bool Contains(Point3D p)
    {
        return Radius > Center.DistanceTo(p);
    }

    /// <summary>
    /// проверка, пересекаются ли два шара
    /// </summary>
    public bool IntersectsWith(Sphere3D other)
    {
        return Radius <= Center.DistanceTo(other.Center) + other.Radius || Radius >= Center.DistanceTo(other.Center) - other.Radius;
    }

    /// <summary>
    /// проверка, лежит ли другой шар полностью внутри этого шара
    /// </summary>
    public bool Contains(Sphere3D other)
    {
        return Radius > (Center.DistanceTo(other.Center) + other.Radius);
    }
}