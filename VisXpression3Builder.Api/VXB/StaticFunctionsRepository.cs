using VisXpression3Builder.Lib.Attributes;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Api.VXB
{
    /// <summary>
    /// This class is for defining static methods as static functions that can be referenced and used in user defined functions
    /// </summary>
    public class StaticFunctionsRepository : AStaticFunctionsRepository
    {
        [StaticFunction(Title = "Xor")]
        public static double? Xor(double? a, double? b)
        {
            return 1.0 * ((int)a ^ (int)b);
        }

        [StaticFunction(Title = "Xnor")]
        public static double? Xnor(double? a, double? b)
        {
            return 1.0 * ~((int)a ^ (int)b);
        }

        [StaticFunction(Title = "Not")]
        public static bool? Not(bool? a)
        {
            return !a;
        }
    }
}