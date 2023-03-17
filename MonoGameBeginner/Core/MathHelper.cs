using System;
using System.Security.Cryptography;

namespace Core;

public static class MathHelper
{
    public const float FeetInMiles = 5280f;
    
    public static float MilesToFeet(float miles)
    {
        return miles / FeetInMiles;
    }

    public static float TotalSeconds(int hours, int minutes, int seconds)
    {
        return ((hours * 60) + minutes) * 60 + seconds;
    }

    public static float RectanglePerimeter(float width, float height)
    {
        return (width * 2) + (height * 2);
    }

    public static float CircleCircumference(float radius)
    {
        return 2 * MathF.PI * radius;
    }

    public static float CircleArea(float radius)
    {
        return (MathF.Pow(radius, 2) * MathF.PI);
    }

    public static float PointDistance(float x0, float x1, float y0, float y1)
    {
        return MathF.Sqrt(MathF.Pow(x1 - x0, 2) + MathF.Pow(y1 - y0, 2));
    }

    public static float TriangleArea(float x0, float y0, float x1, float y1, float x2, float y2)
    {
        // Heron's Formula
        var sideA = PointDistance(x0, x1, y0, y1);
        var sideB = PointDistance(x0, x2, y0, y2);
        var sideC = PointDistance(x1, x2, y1, y2);

        var semiperimeter = (sideA + sideB + sideC) / 2;

        return MathF.Sqrt(semiperimeter * (semiperimeter - sideA) * (semiperimeter - sideB) * (semiperimeter - sideC));
    }
    
}