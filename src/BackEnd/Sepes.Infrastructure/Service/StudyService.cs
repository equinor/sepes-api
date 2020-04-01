using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Service
{
    public class StudyService
    {
        readonly SepesDbContext _db;

        public StudyService(SepesDbContext db)
        {
            _db = db;
        }



               
    }
}
