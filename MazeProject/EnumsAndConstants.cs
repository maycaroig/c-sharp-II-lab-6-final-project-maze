using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeProject
{
	[Flags]
	public enum DirectionEnum
	{
		/// <summary>Undefined direction</summary>
		None = 0,
		/// <summary>North</summary>
		North = 1,
		/// <summary>South</summary>
		South = 2,
		/// <summary>East</summary>
		East = 4,
		/// <summary>West</summary>
		West = 8,
		/// <summary>Combination of all four cardinal directions</summary>
		All = North | East | South | West
	}

	public static class Constants
	{
		public const int MIN_WIDTH = 5;
		public const int MAX_WIDTH = 50;
		public const int MIN_HEIGHT = 5;
		public const int MAX_HEIGHT = 50;

		public static readonly DirectionEnum[] Cardinals = { DirectionEnum.North, DirectionEnum.South, DirectionEnum.East, DirectionEnum.West };

		public static readonly Range<int> WidthRange = new Range<int>(MIN_WIDTH, MAX_WIDTH);
		public static readonly Range<int> HeightRange = new Range<int>(MIN_HEIGHT, MAX_HEIGHT);
	}


	public static class EnumMethods
	{
		/// <summary>
		/// Gets the opposite direction from the one given
		/// </summary>
		/// <param name="direction">Original direction</param>
		/// <returns>Opposite direction</returns>
		/// <remarks>Input of East returns West.  Input of North, East return South, West</remarks>
		public static DirectionEnum GetOpposite(this DirectionEnum direction)
		{
			DirectionEnum result = DirectionEnum.None;
			if (direction.HasFlag(DirectionEnum.East))
			{
				result |= DirectionEnum.West;
			}
			if (direction.HasFlag(DirectionEnum.West))
			{
				result |= DirectionEnum.East;
			}
			if (direction.HasFlag(DirectionEnum.North))
			{
				result |= DirectionEnum.South;
			}
			if (direction.HasFlag(DirectionEnum.South))
			{
				result |= DirectionEnum.North;
			}
			return result;
		}
	}
}
