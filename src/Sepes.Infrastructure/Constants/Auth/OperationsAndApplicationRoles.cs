using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class OperationsAndApplicationRoles
    {
        static Dictionary<Tuple<UserOperations, bool>, HashSet<string>> _operations;

        public static Dictionary<Tuple<UserOperations, bool>, HashSet<string>> GetOperations()
        {
            EnsureOperationExsits();

            return _operations;
        }

        public static void EnsureOperationExsits()
        {
            if (_operations == null)
            {
                _operations = new Dictionary<Tuple<UserOperations, bool>, HashSet<string>>();
                AddRole(UserOperations.StudyRead, true, AppRoles.Admin, AppRoles.Sponsor);
                AddRole(UserOperations.StudyCreate, false, AppRoles.Admin, AppRoles.Sponsor);
                AddRole(UserOperations.StudyDelete, true, AppRoles.Admin, AppRoles.Sponsor);
            }
        }

        static void AddRole(UserOperations operation, bool onlyForNonHidden, params string[] allowedForRoles)
        {
            _operations.Add(new Tuple<UserOperations, bool>(operation, onlyForNonHidden), new HashSet<string>(allowedForRoles));
        }

     
    }
}
