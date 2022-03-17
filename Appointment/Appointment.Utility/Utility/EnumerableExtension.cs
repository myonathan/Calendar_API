using System;
using System.Collections.Generic;
using System.Linq;

namespace Appointment.Utilities
{
    public static class EnumerableExtension
    {
        private static Random random = new Random();

        public static void Each<TSource>(this IEnumerable<TSource> source, Action<TSource> execution) //where TSource : new()
        {
            foreach (TSource item in source)
            {
                execution?.Invoke(item);
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {


            return source.OrderBy(model => keySelector.Invoke(model)).FirstOrDefault();

        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {

            return source.OrderByDescending(model => keySelector.Invoke(model)).FirstOrDefault();
        }


        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
        public static IList<TSource> Shuffle<TSource>(this IList<TSource> source)
        {
            var n = source.Count();
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                var value = source[k];
                source[k] = source[n];
                source[n] = value;
            }

            return source;
        }

        public static IList<TSource> Shift<TSource>(this IList<TSource> source, int from, int to) where TSource : class, ISortable, new()
        {
            var start = from > to ? to : from + 1;
            var end = from > to ? from - 1 : to;
            var increase = from > to ? 1 : -1;

            var model = source.First(i => i.Position.Equals(from));
            var changes = source.Where(i => i.Position >= start && i.Position <= end);
            foreach (var change in changes)
            {
                change.Position += increase;
            }

            model.Position = to;

            return source;
        }

        public static TSource SafeFirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source?.Any() == true)
                return source.FirstOrDefault();
            else
                return default;
        }

        public static IEnumerable<TResult> Distinct<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(model => selector.Invoke(model)).Distinct();
        }

        public static IList<TSource> DistinctList<TSource>(this IEnumerable<TSource> source)
        {
            return source.Distinct().ToList();
        }

        public static IList<TResult> DistinctList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(model => selector.Invoke(model)).Distinct().ToList();
        }

        public static IList<TSource> WhereList<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Where(model => predicate.Invoke(model)).ToList();
        }

        public static IList<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(model => selector.Invoke(model)).ToList();
        }

        public static IList<TSource> OrderByList<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.OrderBy(model => keySelector.Invoke(model)).ToList();
        }

        public static IList<TSource> OrderByDescendingList<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.OrderByDescending(model => keySelector.Invoke(model)).ToList();
        }

        public static IList<TSource> SortByPositionList<TSource>(this IEnumerable<TSource> source) where TSource : ISortable
        {
            return source.SortByPosition().ToList();
        }

        public static IEnumerable<TSource> SortByPosition<TSource>(this IEnumerable<TSource> source) where TSource : ISortable
        {
            var position = 1;

            foreach (TSource model in source)
            {
                model.Position = position;
                position++;
            }

            return source;
        }
    }
    public interface ISortable
    {
        int Position { get; set; }
    }
}
