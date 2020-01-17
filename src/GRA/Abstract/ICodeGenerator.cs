namespace GRA.Abstract
{
    public interface ICodeGenerator
    {
        void SetAllowedCharacters(string allowedCharacters);
        string Generate(int codeLength, bool formatCode = true);
        string FormatCode(string code);
    }
}
