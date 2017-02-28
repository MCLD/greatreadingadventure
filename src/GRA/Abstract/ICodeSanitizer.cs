
namespace GRA.Abstract
{
    public interface ICodeSanitizer
    {
        string Sanitize(string code, int length = 50);
    }
}
