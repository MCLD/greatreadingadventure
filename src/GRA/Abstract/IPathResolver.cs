
namespace GRA.Abstract
{
    public interface IPathResolver
    {
        string ResolveContentPath(string filePath = default(string));
        string ResolveContentFilePath(string filePath = default(string));
    }
}
