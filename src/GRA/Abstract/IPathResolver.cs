
namespace GRA.Abstract
{
    public interface IPathResolver
    {
        string ResolveContentPath(string filePath = default);
        string ResolveContentFilePath(string filePath = default);
        string ResolvePrivatePath(string filePath = default);
        string ResolvePrivateFilePath(string filePath = default);
        string ResolvePrivateTempFilePath(string filePath = default);
    }
}
