using System;
using VisXpression3Builder.Lib.Attributes;

namespace VisXpression3Builder.Lib.Repositories
{
    internal class BasicFunctionsRepository : ABuiltInFunctionRepository<BasicFunctionAttribute>
    {
        [BasicFunction(Title = "Число")]
        public static double? Number(double? x)
        {
            return x;
        }

        [BasicFunction(Title = "Булево")]
        public static bool? Boolean(bool? x)
        {
            return x;
        }

        [BasicFunction(Title = "Массив чисел")]
        public static double?[] NumberArray(double?[] arr)
        {
            return arr;
        }

        [BasicFunction(Title = "Сложение")]
        public static double? Add(double? left, double? right)
        {
            return left + right;
        }

        [BasicFunction(Title = "Вычитание")]
        public static double? Subtract(double? left, double? right)
        {
            return left - right;
        }

        [BasicFunction(Title = "Умножение")]
        public static double? Multiply(double? left, double? right)
        {
            return left * right;
        }

        [BasicFunction(Title = "Деление")]
        public static double? Divide(double? left, double? right)
        {
            return left / right;
        }

        [BasicFunction(Title = "Степень")]
        public static double? Power(double? left, double? right)
        {
            return Math.Pow(left.Value, right.Value);
        }

        [BasicFunction(Title = "И")]
        public static bool? And(bool? left, bool? right)
        {
            return left.Value && right.Value;
        }

        [BasicFunction(Title = "Или")]
        public static bool? Or(bool? left, bool? right)
        {
            return left.Value || right.Value;
        }


        [BasicFunction(Title = "Меньше")]
        public static bool? Less(double? left, double? right)
        {
            return left < right;
        }

        [BasicFunction(Title = "Менье или равно")]
        public static bool? LessOrEqual(double? left, double? right)
        {
            return left <= right;
        }

        [BasicFunction(Title = "Равно")]
        public static bool? Equal(double? left, double? right)
        {
            return left == right;
        }

        [BasicFunction(Title = "Больше или равно")]
        public static bool? GreaterOrEqual(double? left, double? right)
        {
            return left >= right;
        }

        [BasicFunction(Title = "Больше")]
        public static bool? Greater(double? left, double? right)
        {
            return left > right;
        }

        [BasicFunction(Title = "Округление")]
        public static double? Round(double? x)
        {
            return Math.Round(x.Value);
        }

        [BasicFunction(Title = "Округление вниз")]
        public static double? Floor(double? x)
        {
            return Math.Floor(x.Value);
        }

        [BasicFunction(Title = "Округление вверх")]
        public static double? Ceil(double? x)
        {
            return Math.Ceiling(x.Value);
        }

        [BasicFunction(Title = "Перенаправить")]
        public static double? Reroute(double? x)
        {
            return x;
        }

        [BasicFunction(Title = "Перенаправить")]
        public static bool? Reroute(bool? x)
        {
            return x;
        }

        [BasicFunction(Title = "Перенаправить")]
        public static double?[] Reroute(double?[] x)
        {
            return x;
        }
    }
}