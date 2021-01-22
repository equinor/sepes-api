using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepes.Infrastructure.Util
{
    public class GenericNameValidation
    {
        public static void ValidateName (string name, int minimumLength = 3)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Name can not be empty");
            }

            var onlyLettersAndNumbers = new Regex(@"^[a-zA-Z0-9]+$");

            if(!onlyLettersAndNumbers.IsMatch(name) || name.Length < minimumLength)
            {
                throw new Exception("Name should should only contain letters or/and numbers and be minimum 3 characters long");
            }
        }
    }
}
