using System;
using System.Text.RegularExpressions;

namespace Sepes.Common.Util
{
    public class GenericNameValidation
    {
        public static void ValidateName (string name, int minimumLength = 3)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name can not be empty");
            }

            var onlyLettersAndNumbers = new Regex(@"^[a-zA-Z0-9]+$");
            var nameWithoutSpaces = name.Replace(" ", "").Trim();
            if(!onlyLettersAndNumbers.IsMatch(nameWithoutSpaces) || nameWithoutSpaces.Length < minimumLength)
            {
                throw new ArgumentException($"Name should should only contain letters or/and numbers and be minimum {minimumLength} characters long");
            }
        }
    }
}
