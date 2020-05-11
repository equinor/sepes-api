using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Service
{
    public class StudyService2
    {
        readonly SepesDbContext _db;

        public StudyService2(SepesDbContext db)
        {            
            _db = db;          
        }



               
    }
}
