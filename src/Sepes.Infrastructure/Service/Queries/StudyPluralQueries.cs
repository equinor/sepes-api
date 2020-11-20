using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Linq;

namespace Sepes.Infrastructure.Service.Queries
{
    public static class StudyPluralQueries
    {
        #region Public Methods
        public static IQueryable<Study> ActiveStudiesIncludingHiddenQueryable(SepesDbContext db, int userId)
        {
            //TOD: Review access check. As of now, if the user has ANY role associated with a study, he can view it
            return StudyBaseQueries.ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp => sp.User)
                .Where(s =>
                (s.Restricted == false || s.StudyParticipants.Where(sp => sp.UserId == userId).Any()));
        }

        #endregion
    }
}
