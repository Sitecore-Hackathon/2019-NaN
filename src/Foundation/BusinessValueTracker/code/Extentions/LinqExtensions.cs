using System;
using System.Collections.Generic;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Extentions
{
    public static class LinqExtensions
    {
        public static List<T>[] Partition<T>(this List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            var partitions = new List<T>[totalPartitions];

            var maxSize = (int) Math.Ceiling(list.Count / (double) totalPartitions);
            var k = 0;

            for (var i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (var j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }

                k += maxSize;
            }

            return partitions;
        }
    }
}