using System.Collections.Generic;
using System.Linq;

namespace MarnoldSacn
{
    //public class MarnoldSacnConstants
    public static class MarnoldSacnConstants
    {
        public const int Port = 5568;
        public const int MaxPacketLength = 1444;
        public const int Universe_MinValue = 1;
        public const int Universe_MaxValue = 63999;
        public const int Priority_MaxValue = 200;
        public const byte Priority_Default = 100;
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> collection, int chunkSize)
        {
            int count = collection.Count();
            int chunkCount = count / chunkSize;
            if (count % chunkSize > 0)
            {
                chunkCount++;
            }

            for (int i = 0; i < chunkCount; i++)
            {
                yield return collection.Skip(chunkSize * i).Take(chunkSize);
            }
        }
    }
}