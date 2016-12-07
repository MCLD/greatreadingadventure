using GRA.Abstract;
using System;
using System.Security.Cryptography;

namespace GRA
{
    public class TokenGenerator : ITokenGenerator
    {
        public string Generate(int desiredLength = 10)
        {
            // create more randomness than we need since we'll only keep letters and digits
            byte[] bytes = new byte[desiredLength * 2];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
            }

            // see https://msdn.microsoft.com/en-us/library/3d0e5t57.aspx
            // Convert the binary input into Base64 UUEncoded output. 
            // Each 3 byte sequence in the source data becomes a 4 byte 
            // sequence in the character array.  
            long arrayLength = (long)((4.0d / 3.0d) * bytes.Length);

            // If array length is not divisible by 4, go up to the next 
            // multiple of 4. 
            if (arrayLength % 4 != 0)
            {
                arrayLength += 4 - arrayLength % 4;
            }

            char[] stringArray = new char[arrayLength];

            // convert random bytes to a character array
            Convert.ToBase64CharArray(bytes, 0, bytes.Length, stringArray, 0);

            // only keep letters and numbers
            stringArray = Array.FindAll<char>(stringArray, (c => (char.IsLetterOrDigit(c))));

            string result = new string(stringArray);

            if (result.Length > desiredLength)
            {
                return result.Substring(0, desiredLength);
            }
            else
            {
                return result;
            }
        }
    }
}
