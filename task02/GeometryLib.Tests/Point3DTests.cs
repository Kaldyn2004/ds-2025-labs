namespace GeometryLib.Tests;

public class Point3DTests
{
    [Theory]
    [MemberData(nameof(ConstructorTestData))]
    public void Can_create_point(double x, double y, double z)
    {
        // Создание точки.
        Point3D point = new Point3D(x, y, z);

        // Проверка, что данные переданные в конструктор равны
        Assert.Equal(x, point.X);
        Assert.Equal(y, point.Y);
        Assert.Equal(z, point.Z);
    }

    public static TheoryData<double, double, double> ConstructorTestData()
    {
        return new TheoryData<double, double, double>
        {
            { 0, 0, 0 },
            { 30.15, 15.20, 20.7 },
            { double.MaxValue, double.MinValue, 0 },
        };
    }

    [Theory]
    [MemberData(nameof(DistanceTestData))]
    public void Can_get_distance_to_point(Point3D a, Point3D b, double expectedDistance)
    {
        double distanceAB = a.DistanceTo(b);
        double distanceBA = b.DistanceTo(a);

        Assert.Equal(expectedDistance, distanceAB, precision: Point3D.Precision);
        Assert.Equal(distanceAB, distanceBA, precision: Point3D.Precision);
    }

    public static TheoryData<Point3D, Point3D, double> DistanceTestData()
    {
        return new TheoryData<Point3D, Point3D, double>
        {
            // Растояние точек с одинаковыми координатами равны нулю
            { new Point3D(0, 0, 0), new Point3D(0, 0, 0), 0 },
            { new Point3D(-5, 10, 13), new Point3D(-5, 10, 13), 0 },

            // Расстояние по одной оси равны модулю разницы этих растояний
            { new Point3D(0, 0, 0), new Point3D(15, 0, 0), 15 },
            { new Point3D(6, 8, -15), new Point3D(6, 8, -20), 5 },
            { new Point3D(3, 14, 5), new Point3D(3, 4, 5), 10 },

            // Расстояние между точками
            { new Point3D(0, 0, 0), new Point3D(3, 8, 13),  Math.Sqrt(242) },
            { new Point3D(0, -8, 7), new Point3D(0, -2, -1), 10 },
            { new Point3D(0, 0, 0), new Point3D(1, 1, 1), Math.Sqrt(3) },
            { new Point3D(1, 2, 3), new Point3D(4, 6, 8), Math.Sqrt(50) },
            { new Point3D(-10, -20, -3), new Point3D(4, 6, 8), Math.Sqrt(993) },
        };
    }
}