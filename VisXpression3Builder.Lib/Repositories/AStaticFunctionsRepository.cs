using VisXpression3Builder.Lib.Attributes;

namespace VisXpression3Builder.Lib.Repositories
{
    /// <summary>
    /// This class is for defining static methods as static functions that can be referenced and used in user defined functions.
    /// Have to be implemented by library-user.
    /// </summary>
    public abstract class AStaticFunctionsRepository : ABuiltInFunctionRepository<StaticFunctionAttribute>
    {
    }
}
