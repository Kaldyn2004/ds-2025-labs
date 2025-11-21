using System;
using System.Text.RegularExpressions;
namespace GeometryLib.Tests;

public class Sphere3DTests
{
    [Theory]
    [MemberData(nameof(ConstructorTestData))]
    public void Can_create_sphere(Point3D center, double radius)
    {
        // Создание сферы.
        Sphere3D sphere = new Sphere3D(center, radius);

        // Проверка, что данные переданные в конструктор равны
        Assert.Equal(center, sphere.Center);
        Assert.Equal(radius, sphere.Radius);
    }

    public static TheoryData<Point3D, double> ConstructorTestData()
    {
        return new TheoryData<Point3D, double>
        {
            { new Point3D(10, 15, 17), 10 },
            { new Point3D(-16, 15, -12), 4 },
            { new Point3D(42, -8, 4), 24 },
            { new Point3D(0, 0, 0), 0.5 },
            { new Point3D(1, 2, 3), 1.5 },
        };
    }

    [Theory]
    [MemberData(nameof(ConstructorWithExceptionTestData))]
    public void Can_not_create_sphere(Point3D center, double radius, string errorText)
    {
        // Создание сферы.
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Sphere3D(center, radius));

        // Проверка, из-за чего возникло исключение
        Assert.Equal(errorText, exception.Message);
    }

    public static TheoryData<Point3D, double, string> ConstructorWithExceptionTestData()
    {
        return new TheoryData<Point3D, double, string>
        {
            // Проверка ошибки при отрицательном радиусе
            { new Point3D(10, 15, 17), -1, "The sphere's radius must be positive." },
            { new Point3D(-16, 15, -12), -321, "The sphere's radius must be positive." },

            // Проверка ошибки при нулевом радиусе
            { new Point3D(10, 15, 17), 0, "The sphere's radius cannot be zero." },
        };
    }

    [Theory]
    [MemberData(nameof(DiameterTestData))]
    public void Can_get_diameter(Point3D center, double radius, double diameter)
    {
        // Создание сферы.
        Sphere3D sphere = new Sphere3D(center, radius);

        // Проверка, диаметра
        Assert.Equal(diameter, sphere.Diameter);
    }

    public static TheoryData<Point3D, double, double> DiameterTestData()
    {
        return new TheoryData<Point3D, double, double>
        {
            // Диаметр в 2 раза больше радиуса
            { new Point3D(10, 15, 17), 10, 20 },
            { new Point3D(-16, 15, -12), 4, 8 },
            { new Point3D(42, -8, 4), 24, 48 },
            { new Point3D(0, 0, 0), 0.5, 1.0 },
            { new Point3D(1, 2, 3), 1.5, 3.0 },
        };
    }

    [Theory]
    [MemberData(nameof(AreaTestData))]
    public void Can_get_area(Point3D center, double radius, double area)
    {
        // Создание сферы.
        Sphere3D sphere = new Sphere3D(center, radius);

        // Проверка, площади сферы
        Assert.Equal(area, sphere.Area, Point3D.Tolerance);
    }

    public static TheoryData<Point3D, double, double> AreaTestData()
    {
        return new TheoryData<Point3D, double, double>
        {
            { new Point3D(10, 15, 17), 10, 400 * Math.PI },
            { new Point3D(-16, 15, -12), 4, 64 * Math.PI },
            { new Point3D(42, -8, 4), 1.5, 9 * Math.PI },
            { new Point3D(0, 0, 0), 101.25, 41006.25 * Math.PI },
        };
    }

    [Theory]
    [MemberData(nameof(VolumeTestData))]
    public void Can_get_volume(Point3D center, double radius, double volume)
    {
        // Создание сферы.
        Sphere3D sphere = new Sphere3D(center, radius);

        // Проверка, объема
        Assert.Equal(volume, sphere.Volume, Point3D.Tolerance);
    }

    public static TheoryData<Point3D, double, double> VolumeTestData()
    {
        return new TheoryData<Point3D, double, double>
        {
            { new Point3D(10, 15, 17), 10, (4.0 / 3.0) * Math.PI * 1000 },
            { new Point3D(-16, 15, -12), 3, (4.0 / 3.0) * Math.PI * 27 },
            { new Point3D(42, -8, 4), 0.5, (4.0 / 3.0) * Math.PI * 0.125 },
            { new Point3D(0, 0, 0), 2.5, (4.0 / 3.0) * Math.PI * 15.625 },
        };
    }


    [Theory]
    [MemberData(nameof(SphereContainPointTestData))]
    public void Can_check_sphere_contain_point(Point3D center, double radius, Point3D a, bool expected)
    {
        // Создание сферы.
        Sphere3D sphere = new Sphere3D(center, radius);

        // Проверка, нахождения точки внутри сферы
        Assert.Equal(sphere.Contains(a), expected);
    }

    public static TheoryData<Point3D, double, Point3D, bool> SphereContainPointTestData()
    {
        return new TheoryData<Point3D, double, Point3D, bool>
        {
            // Точка находится внутри сферы
            { new Point3D(10, 15, 17), 10, new Point3D(10, 15, 17), true },
            { new Point3D(10, 15, 17), 10, new Point3D(12, 13, 14), true },
            { new Point3D(-16, 15, -12), 4, new Point3D(-15, 14, -13), true },

            // Точка находится на поверхности
            { new Point3D(42, -8, 4), 24, new Point3D(66, -8, 4), false },
            { new Point3D(42, -8, 4), 24, new Point3D(42, -32, 4), false },
            { new Point3D(42, -8, 4), 24, new Point3D(42, -8, 28), false },

             // Точка находится за пределами сферы
            { new Point3D(42, -8, 4), 24, new Point3D(-42, 8, -4), false },
            { new Point3D(0, 0, 0), 0.5, new Point3D(1, 1, 1), false },
            { new Point3D(1, 2, 3), 1.5, new Point3D(3, 4, 5), false },
        };
    }

    [Theory]
    [MemberData(nameof(IntersectionOfSpheresTestData))]
    public void Can_check_sphere_intersects_with_other_sphere(Sphere3D a, Sphere3D b, bool expected)
    {
        Assert.Equal(a.IntersectsWith(b), expected);
        Assert.Equal(b.IntersectsWith(a), expected);
    }

    public static TheoryData<Sphere3D, Sphere3D, bool> IntersectionOfSpheresTestData()
    {
        return new TheoryData<Sphere3D, Sphere3D, bool>
        {
            // Шары с одинаковым центрои и радиусом пересекаются
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(0, 0, 0), 10), true },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 19), 3), true },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 4), 1.5), true },

            // Шары с общим центром и разным радиусом пересекаются
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(0, 0, 0), 11), true },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 19), 3.1), true },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 4), 1.505), true },

            // Если 1 шар полностью внутри 2 шара, то они пересекаются
            { new Sphere3D(new Point3D(0, 0, 0), 20), new Sphere3D(new Point3D(3, 2, 1), 10), true },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 19), 3.1), true },

             // Пересекающиеся шары
            { new Sphere3D(new Point3D(0, 0, 0), 21), new Sphere3D(new Point3D(40, 0, 0), 20), true },
            { new Sphere3D(new Point3D(-8, 0, 0), 15), new Sphere3D(new Point3D(15, 0, 0), 9), true },
            { new Sphere3D(new Point3D(13, 12, 1), 4), new Sphere3D(new Point3D(15, 12, 2), 5), true },

            // Не пересекающиеся шары
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(21, 0, 0), 10), false },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 31), 3), false },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 12), 1.5), false },
        };
    }

    [Theory]
    [MemberData(nameof(SphereСontainsSphereTestData))]
    public void Can_check_sphere_contains_other_sphere(Sphere3D a, Sphere3D b, bool expected)
    {
        Assert.Equal(a.Contains(b), expected);
    }

    public static TheoryData<Sphere3D, Sphere3D, bool> SphereСontainsSphereTestData()
    {
        return new TheoryData<Sphere3D, Sphere3D, bool>
        {
            // Одинаковые шары друг друга не содержат в себе
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(0, 0, 0), 10), false },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 19), 3), false },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 4), 1.5), false },

            // Пересекающиеся шары не содержат друг друга
            { new Sphere3D(new Point3D(0, 0, 0), 21), new Sphere3D(new Point3D(40, 0, 0), 20), false },
            { new Sphere3D(new Point3D(-8, 0, 0), 15), new Sphere3D(new Point3D(15, 0, 0), 9), false },
            { new Sphere3D(new Point3D(13, 12, 1), 4), new Sphere3D(new Point3D(15, 12, 2), 5), false },

            // Шар с общим центром, и большим радиусом содержит меньший шар, но не наоборот
            { new Sphere3D(new Point3D(0, 0, 0), 12), new Sphere3D(new Point3D(0, 0, 0), 11), true },
            { new Sphere3D(new Point3D(15, -17, 19), 3.5), new Sphere3D(new Point3D(15, -17, 19), 3.1), true },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.6), new Sphere3D(new Point3D(-8, -7, 4), 1.505), true },
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(0, 0, 0), 11), false },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 19), 3.1), false },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 4), 1.505), false },

            // Не содержат друг друга
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(21, 0, 0), 10), false },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Sphere3D(new Point3D(15, -17, 31), 3), false },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 12), 1.5), false },

            // Если 1 шар полностью внутри 2 шара, то 2 содержит шар, но не наоборот
            { new Sphere3D(new Point3D(0, 0, 0), 20), new Sphere3D(new Point3D(3, 2, 1), 10), true },
            { new Sphere3D(new Point3D(3, 2, 1), 10), new Sphere3D(new Point3D(0, 0, 0), 20), false },
        };
    }

    [Theory]
    [MemberData(nameof(DistanceToPointTestData))]
    public void Can_get_distance_to_point(Sphere3D s, Point3D p, double expectedDistance)
    {
        Assert.Equal(s.DistanceTo(p), expectedDistance);
    }

    public static TheoryData<Sphere3D, Point3D, double> DistanceToPointTestData()
    {
        return new TheoryData<Sphere3D, Point3D, double>
        {
            // Растояние до центра равно радиусу
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Point3D(0, 0, 0), 10 },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Point3D(15, -17, 19), 3 },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Point3D(-8, -7, 4), 1.5 },

            // Растояние до точки находящейся внутри сферы
            { new Sphere3D(new Point3D(0, 0, 0), 21), new Point3D(20, 0, 0), 1 },
            { new Sphere3D(new Point3D(-8, 0, 0), 15), new Point3D(-5, 0, 0), 12 },
            { new Sphere3D(new Point3D(13, 12, 1), 4), new Point3D(15, 11, 2), 4 - Math.Sqrt(6) },

            // Ратсояние до точки на поверхности сферы равно нулю
            { new Sphere3D(new Point3D(0, 0, 0), 12), new Point3D(0, 12, 0), 0 },
            { new Sphere3D(new Point3D(15, -17, 19), 5), new Point3D(12, -17, 15), 0 },

            // Растояние до точки за пределами сферы
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Point3D(21, 0, 0), 11 },
            { new Sphere3D(new Point3D(15, -17, 19), 3), new Point3D(15, -17, 31), 9 },
            { new Sphere3D(new Point3D(-8, -7, 4), 21), new Point3D(1, -7, 44), 20 },
        };
    }

    [Theory]
    [MemberData(nameof(DistanceToSphereTestData))]
    public void Can_get_distance_to_sphere(Sphere3D a, Sphere3D b, double expected)
    {
        Assert.Equal(a.DistanceTo(b), expected, Point3D.Tolerance);
    }

    public static TheoryData<Sphere3D, Sphere3D, double> DistanceToSphereTestData()
    {
        return new TheoryData<Sphere3D, Sphere3D, double>
        {
            // Растояние между пересекающимися сферами равно нулю
            { new Sphere3D(new Point3D(0, 0, 0), 10), new Sphere3D(new Point3D(0, 0, 0), 10), 0 },
            { new Sphere3D(new Point3D(15, -17, 19), 30), new Sphere3D(new Point3D(25, -12, 7), 14), 0 },
            { new Sphere3D(new Point3D(-8, -7, 4), 1.5), new Sphere3D(new Point3D(-8, -7, 7), 1.5), 0 },

            // 1 Сфера находится внутри другой
            { new Sphere3D(new Point3D(0, 0, 0), 20), new Sphere3D(new Point3D(3, 2, 1), 10), 10 - Math.Sqrt(14) },
            { new Sphere3D(new Point3D(16, 15, 14), 10), new Sphere3D(new Point3D(0, 0, 0), 38.8), 28.8 - Math.Sqrt(677) },

            // Сферы находятся за пределом друг друга
            { new Sphere3D(new Point3D(1,8, -15), 12), new Sphere3D(new Point3D(3, 4, 16), 11), Math.Sqrt(981) - 23 },
            { new Sphere3D(new Point3D(5, -7, 9), 3.5), new Sphere3D(new Point3D(-5, 7, -9), 3.5), Math.Sqrt(620) - 7 },
        };
    }
}