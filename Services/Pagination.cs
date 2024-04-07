using System.Linq;
namespace VFM.Services
{
    public static class Pagination
    {
        public static int GetNumberPages<T>(this IEnumerable<T> array, int maxElementInPage)
        {
            int pages = array.Count() / maxElementInPage;
            if (array.Count() % maxElementInPage > 0) pages++;

            return pages;
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> array, int maxElementInPage, int currentPage)
        {
            int startPosition = maxElementInPage * currentPage - 20;
            return array.Skip(startPosition).Take(maxElementInPage);
        }
    }
}
