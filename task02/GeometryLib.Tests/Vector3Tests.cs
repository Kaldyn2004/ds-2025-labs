namespace GeometryLib.Tests;

public class Vector3Tests
{
    [Theory]
    [MemberData(nameof(LengthTestData))]
    public void Can_get_length(Vector3 vector, double expectedLength)
    {
        Assert.Equal(expectedLength, vector.Length, precision: Vector3.Precision);
    }

    public static TheoryData<Vector3, double> LengthTestData()
    {
        return new TheoryData<Vector3, double>
        {
            { new Vector3(0, 0, 0), 0 },
            { new Vector3(3, 0, 0), 3 },
            { new Vector3(-1, -2, -2), 3 },
            { new Vector3(1, 1, 1), Math.Sqrt(3) },
        };
    }

    [Theory]
    [MemberData(nameof(AddTestData))]
    public void Can_add_vectors(Vector3 a, Vector3 b, Vector3 expected)
    {
        // Сложение векторов — это коммутативная операция.
        Assert.Equal(expected, a.Add(b));
        Assert.Equal(expected, b.Add(a));
    }

    public static TheoryData<Vector3, Vector3, Vector3> AddTestData()
    {
        return new TheoryData<Vector3, Vector3, Vector3>
        {
            { new Vector3(1, 2, 3), new Vector3(4, 5, 6), new Vector3(5, 7, 9) },
            { new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Vector3(1, 1, 1) },
            { new Vector3(-1, -2, -3), new Vector3(1, 2, 3), new Vector3(0, 0, 0) },
        };
    }

    [Theory]
    [MemberData(nameof(DotProductTestData))]
    public void Can_calculate_dot_product(Vector3 a, Vector3 b, double expected)
    {
        // Скалярное произведение — это коммутативная операция.
        Assert.Equal(expected, a.DotProduct(b), precision: Vector3.Precision);
        Assert.Equal(expected, b.DotProduct(a), precision: Vector3.Precision);
    }

    public static TheoryData<Vector3, Vector3, double> DotProductTestData()
    {
        return new TheoryData<Vector3, Vector3, double>
        {
            { new Vector3(1, 0, 0), new Vector3(0, 1, 0), 0 },
            { new Vector3(2, 3, 4), new Vector3(5, 6, 7), 56 },
            { new Vector3(-1, -1, -1), new Vector3(1, 1, 1), -3 },
        };
    }

    [Theory]
    [MemberData(nameof(NormalizeVectorsTestData))]
    public void Can_normalize_vectors(Vector3 value, Vector3 expected)
    {
        Vector3 result = value.Normalize();
        Assert.Equal(expected, result);
    }

    public static TheoryData<Vector3, Vector3> NormalizeVectorsTestData()
    {
        return new TheoryData<Vector3, Vector3>
        {
            { new Vector3(0, 1, 0), new Vector3(0, 1, 0) }, // Вектор единичной длины
            { new Vector3(3, 4, 0), new Vector3(0.6, 0.8, 0) }, // Вектор не единичной длины
        };
    }

    [Fact]
    public void Cannot_normalize_zero_vector()
    {
        Vector3 v = new Vector3(0, 0, 0);
        Assert.Throws<InvalidOperationException>(() => v.Normalize());
    }

    [Theory]
    [MemberData(nameof(AreOrthogonalTestData))]
    public void Can_check_vectors_are_orthogonal(Vector3 a, Vector3 b, bool expected)
    {
        // Проверка ортогональности — это коммутативная операция
        Assert.Equal(Vector3.AreOrthogonal(a, b), expected);
        Assert.Equal(Vector3.AreOrthogonal(b, a), expected);
    }

    public static TheoryData<Vector3, Vector3, bool> AreOrthogonalTestData()
    {
        return new TheoryData<Vector3, Vector3, bool>()
        {
            // Все оси ортогональны друг другу
            { new Vector3(1, 0, 0), new Vector3(0, 1, 0), true },
            { new Vector3(1, 0, 0), new Vector3(0, 0, 1), true },
            { new Vector3(0, 1, 0), new Vector3(0, 0, 1), true },

            // Указанные ниже вектора не ортогональны
            { new Vector3(1, 1, 0), new Vector3(1, 0, 0), false },
            { new Vector3(3, 4, 0), new Vector3(0, 1, 1), false },

            // Нулевой вектор ортогонален любым векторам
            { new Vector3(0, 0, 0), new Vector3(1, 2, 3), true },
            { new Vector3(0, 0, 0), new Vector3(7, 12, 0), true },
        };
    }

    [Theory]
    [MemberData(nameof(ProjectTestData))]
    public void Can_project_vector_to_another_vector(Vector3 a, Vector3 b, Vector3 expected)
    {
        Vector3 result = a.Project(b);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Vector3, Vector3, Vector3> ProjectTestData()
    {
        return new TheoryData<Vector3, Vector3, Vector3>()
        {
            // Проекция на вектор нулевой длины → нулевой вектор
            { new Vector3(1, 2, 3), new Vector3(0, 0, 0), new Vector3(0, 0, 0) },

            // Проекция на ортогональный вектор → нулевой вектор
            { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0) },

            // Проекция вектора на самого себя → тот же вектор
            { new Vector3(2, 3, 4), new Vector3(2, 3, 4), new Vector3(2, 3, 4) },

            // Проекция вектора на ось → вектор на этой оси
            { new Vector3(2, 3, 4), new Vector3(1, 0, 0), new Vector3(2, 0, 0) },
            { new Vector3(17, 10.61, 12.68), new Vector3(0, 1, 0), new Vector3(0, 10.61, 0) },
            { new Vector3(17, 10.61, 12.68), new Vector3(0, 0, 1), new Vector3(0, 0, 12.68) },
        };
    }
}