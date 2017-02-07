using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Abstract
{
    public interface ICodeGenerator
    {
        void SetAllowedCharacters(string allowedCharacters);
        string Generate(int codeLength, bool formatCode = true);
        string FormatCode(string code);
    }
}
