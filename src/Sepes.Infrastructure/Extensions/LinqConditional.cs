using System.Linq;

namespace Sepes.Infrastructure
{
    public static class LinqConditional
    {
        public static IQueryable<T> If<T>(
           this IQueryable<T> source,
           bool condition,
           System.Func<IQueryable<T>, IQueryable<T>> transform)
        {
            return condition ? transform(source) : source;
        }
    }
}
