using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeProject
{
	public static class Utils
	{
		public static Random Rnd = new Random();

		/// <summary>
		/// Gets a random value from the given IEnumerable collection
		/// </summary>
		/// <typeparam name="T">Data type of collection</typeparam>
		/// <param name="items">Collection</param>
		/// <returns>Random item from collection</returns>
		public static T GetRandomValue<T>(this IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (items.Count() == 0)
			{
				throw new ArgumentException("items", "Items collection cannot be empty");
			}
			return items.ElementAt(Rnd.Next(0, items.Count()));
		}
	}
}
