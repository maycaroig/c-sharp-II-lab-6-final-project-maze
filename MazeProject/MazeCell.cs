using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeProject
{
	public class MazeCell
	{
		#region Properties
		/// <summary>
		/// Represents the walls that are "up" in a specific maze cell
		/// </summary>
		public DirectionEnum Walls { get; set; }

		/// <summary>
		/// Position in the maze
		/// </summary>
		public Position Position { get; set; }
		#endregion Properties

		#region Constructors

        public MazeCell()
        {
            Position = new Position(0, 0);
            Walls = DirectionEnum.All;
        }

        public MazeCell(int x, int y, DirectionEnum walls)
        {
            Position = new Position(x, y);
            Walls = walls;
        }

		#endregion Constructors

		#region Methods
		/// <summary>
		/// Removes the wall(s) in the given direction(s)
		/// </summary>
		/// <param name="wallToRemove">Direction of wall to remove</param>
		public void RemoveWall(DirectionEnum wallToRemove)
		{
            Walls &= ~wallToRemove;
		}

		public override string ToString()
		{
			return string.Format("{0} - Walls: {1}", Position, Walls);
		}
		#endregion Methods
	}
}
