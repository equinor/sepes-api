using AutoMapper;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceOperationService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public SandboxResourceOperationService(SepesDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


    }
}
