using System;
using System.Collections.Generic;

// 抽象基类：形状
public abstract class Shape
{
    // 抽象方法：计算面积（子类必须实现）
    public abstract double CalculateArea();

    // 抽象方法：判断形状是否合法（子类必须实现）
    public abstract bool IsValid();
}

// 长方形类
public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double CalculateArea()
    {
        if (!IsValid())
            return 0; // 不合法的长方形面积视为0
        return Width * Height;
    }

    public override bool IsValid()
    {
        // 长和宽都必须大于0
        return Width > 0 && Height > 0;
    }

    public override string ToString()
    {
        return $"长方形(宽={Width:F2}, 高={Height:F2})";
    }
}

// 正方形类（特殊的长方形）
public class Square : Shape
{
    public double Side { get; set; }

    public Square(double side)
    {
        Side = side;
    }

    public override double CalculateArea()
    {
        if (!IsValid())
            return 0;
        return Side * Side;
    }

    public override bool IsValid()
    {
        // 边长必须大于0
        return Side > 0;
    }

    public override string ToString()
    {
        return $"正方形(边长={Side:F2})";
    }
}

// 圆形类
public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double CalculateArea()
    {
        if (!IsValid())
            return 0;
        return Math.PI * Radius * Radius;
    }

    public override bool IsValid()
    {
        // 半径必须大于0
        return Radius > 0;
    }

    public override string ToString()
    {
        return $"圆形(半径={Radius:F2})";
    }
}

// 主程序
class Program
{
    static void Main(string[] args)
    {
        Random random = new Random();
        List<Shape> shapes = new List<Shape>();

        // 随机创建10个形状对象
        for (int i = 0; i < 10; i++)
        {
            int type = random.Next(3); // 0:长方形, 1:正方形, 2:圆形
            Shape shape;

            switch (type)
            {
                case 0:
                    // 随机生成宽和高（范围：-5 到 10，包含负数用于测试合法性）
                    double width = random.NextDouble() * 10 - 5;
                    double height = random.NextDouble() * 10 - 5;
                    shape = new Rectangle(width, height);
                    break;
                case 1:
                    // 随机生成边长（范围：-5 到 10）
                    double side = random.NextDouble() * 10 - 5;
                    shape = new Square(side);
                    break;
                case 2:
                    // 随机生成半径（范围：-5 到 10）
                    double radius = random.NextDouble() * 10 - 5;
                    shape = new Circle(radius);
                    break;
                default:
                    shape = null;
                    break;
            }

            if (shape != null)
            {
                shapes.Add(shape);
            }
        }

        // 计算所有对象的面积之和
        double totalArea = 0;
        Console.WriteLine("生成的10个形状信息：");
        foreach (var shape in shapes)
        {
            double area = shape.CalculateArea();
            totalArea += area;
            Console.WriteLine($"{shape} - 合法：{shape.IsValid()} - 面积：{area:F2}");
        }

        Console.WriteLine($"\n所有形状的面积之和为：{totalArea:F2}");
    }
}                                                                                                                                                                                                                                                                                                      