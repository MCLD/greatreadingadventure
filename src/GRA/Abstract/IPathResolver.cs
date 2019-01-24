
namespace GRA.Abstract
{
    public interface IPathResolver
    {
        string ResolveContentPath(string filePath = default(string));
        string ResolveContentFilePath(string filePath = default(string));
        string ResolvePrivatePath(string filePath = default(string));
        string ResolvePrivateFilePath(string filePath = default(string));
    }
}
