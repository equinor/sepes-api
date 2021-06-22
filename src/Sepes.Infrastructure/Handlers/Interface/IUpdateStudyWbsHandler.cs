using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Handlers.Interface
{
    public interface IUpdateStudyWbsHandler
    {
        Task Handle(Study study, CancellationToken cancellationToken = default);
    }
}
