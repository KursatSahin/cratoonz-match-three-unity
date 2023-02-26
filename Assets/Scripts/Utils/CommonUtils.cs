using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Utils
{
    public static class CommonUtils
    {
        public static bool IsNullOrEmpty<T>(this T[] list)
        {
            return list == null || list.Length == 0;
        }
        
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any(); 
        }
        
        /// <summary>
        /// Shuffle generic type array
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] list)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            
            for (int i = list.Length - 1; i > 0; i--)
            {
                int j = r.Next(0, i - 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }
    
        public static void Shuffle<T> (this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1) 
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        
        public static void Fade(this Image img , float value)
        {
            Color col = img.color;
            col.a = value;
            img.color = col;
        }
    }
}