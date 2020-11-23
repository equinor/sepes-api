using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class OperationsForAnyUser
    {
        static HashSet<UserOperations> _operations;

        public static HashSet<UserOperations> GetOperations()
        {
            EnsureOperationExsits();

            return _operations;
        }

        public static void EnsureOperationExsits()
        {
            if (_operations == null)
            {
                _operations = new HashSet<UserOperations>();
                _operations.Add(UserOperations.StudyRead);
            }
        }

    }
}
