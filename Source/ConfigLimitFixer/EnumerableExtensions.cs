using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable IDE0046 // Convert to conditional expression
public static class EnumerableExtensions
{
    public static IEnumerable<T[]> Chunk<T>(
        this IEnumerable<T> source,
        int chunkSize)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(chunkSize),
                chunkSize,
                "The chunk size must be greater than 0.");
        }

        return enumerateChunks();

        IEnumerable<T[]> enumerateChunks()
        {
            var taken = 0;
            while (true)
            {
                var chunk = source
                    .Skip(taken)
                    .Take(chunkSize)
                    .ToArray();

                if (chunk.Length == 0)
                {
                    break;
                }

                taken += chunk.Length;
                yield return chunk;
            }
        }
    }
}

#pragma warning restore IDE0046 // Convert to conditional expression
