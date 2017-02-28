
namespace GRA.Abstract
{
    public interface ITokenGenerator
    {
        string Generate(int desiredLength = 10);
    }
}
