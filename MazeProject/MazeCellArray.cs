using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeProject
{
	public class MazeCellArray 
	{
		#region Fields
		private MazeCell[,] m_Cells = null;
		private Range<int>? m_XRange = null;
		private Range<int>? m_YRange = null;
		#endregion Fields

		#region Properties

        public MazeCell this[int x, int y]
        { 
            get 
            { 
                return m_Cells[x, y]; 
            } 
            set 
            { 
                m_Cells[x, y] = value;
            } 
        }

        public MazeCell this[Position pos]
        {
            get
            { 
                return m_Cells[pos.X, pos.Y];
            }
            set
            { 
                m_Cells[pos.X, pos.Y] = value;
            }
        }

        public int Width 
        {
            get
            {
                if (m_Cells != null)
                   return m_Cells.GetLength(0);
                return 0;
            }
        }

        public int Height
        {
            get
            {
                if (m_Cells != null)
                    return m_Cells.GetLength(1);
                return 0;
            }
        }

        public Range<int> XRange
        {
            get 
            {
                if (m_XRange == null)
                {
                    m_XRange = new Range<int>(0, Width - 1);
                } 
                return m_XRange.Value; 
            }
        }

        public Range<int> YRange 
        {
            get 
            {
                if (m_YRange == null)
                { 
                    m_YRange = new Range<int>(0, Height - 1);
                }
                return m_XRange.Value; 
            } 
        }


		#endregion Properties

		#region Constructors
		/// <summary>
		/// Creates a new maze cell array with the given dimensions
		/// </summary>
		/// <param name="width">Width</param>
		/// <param name="height">Height</param>
		public MazeCellArray(int width, int height)
		{
			Resize(width, height);
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Resets the cells in the array
		/// </summary>
		public void Reset()
		{
            for(int i = 0; i <= Width - 1; i++)
            {
                for (int j = 0; j <= Height - 1; j++ )
                {
                    m_Cells[i, j] = new MazeCell(i, j, DirectionEnum.All);
                }
            }
		}

        /// <summary>
        /// Resizes the array to the given dimensions and initializes all the cells
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        public void Resize(int width, int height)
		{
           // Verify that the width and height are legal

            if (width <= Constants.MAX_WIDTH && width >= Constants.MIN_WIDTH
                && height <= Constants.MAX_HEIGHT && height >= Constants.MIN_HEIGHT)
            {
                m_XRange = null;
                m_YRange = null;

                m_Cells = new MazeCell[width, height];
                Reset();
            }
		}

		/// <summary>
		/// Removes the wall from the given cell and the adjacent wall's opposing wall
		/// </summary>
		/// <param name="position">Position of cell to remove wall from</param>
		/// <param name="wallToRemove">Direction of wall to remove</param>
		public void RemoveCellWall(Position position, DirectionEnum wallToRemove)
		{
            // Find adjacent cell in the direction of the wall to remove
            MazeCell cell = this[position];
            Position neighborPosition = cell.Position.MovePosition(wallToRemove);

            // Only remove walls if neighborPosition is within bounds,
            // otherwise the given cell is on the border and walls cannot come down. 
            if (XRange.Contains(neighborPosition.X) && YRange.Contains(neighborPosition.Y)) 
            {
                cell.RemoveWall(wallToRemove);
                MazeCell neighbor = this[neighborPosition];
                neighbor.RemoveWall(wallToRemove.GetOpposite());
            }
		}

		/// <summary>
		/// Gets the neighbor cells of the given location in the cardinal directions
		/// </summary>
		/// <param name="position">Position of cell to get neighbors for</param>
		/// <returns>Dictionary of all found neighbor cells</returns>
		public Dictionary<DirectionEnum, MazeCell> GetNeighborCellsWithAllWalls(Position position)
		{
            Dictionary<DirectionEnum, MazeCell> neighbors = new Dictionary<DirectionEnum, MazeCell>();
            foreach (var directionEnum in Constants.Cardinals)
            {
                var newPosition = (new Position(position.X, position.Y)).MovePosition(directionEnum);            
                
                //Ensuring that the X and Y are within the Ranges
                if (XRange.Contains(newPosition.X) && YRange.Contains(newPosition.Y))
                {
                    var mazeCell = this[newPosition];
                    if (mazeCell.Walls == DirectionEnum.All)
                    {
                        neighbors.Add(directionEnum,mazeCell);
                    }
                }
            }

            return neighbors;
		}

		/// <summary>
		/// Gets the neighbor cells of the given location in the cardinal directions
		/// </summary>
		/// <param name="position">Position of cell to get neighbors for</param>
		/// <returns>Dictionary of all found neighbor cells</returns>
		public Dictionary<DirectionEnum, MazeCell> GetAccessibleNeighborCells(Position position,
            DirectionEnum exclude = DirectionEnum.None)
		{
            Dictionary<DirectionEnum, MazeCell> neighbors = new Dictionary<DirectionEnum, MazeCell>();
            foreach (var dir in Constants.Cardinals)
            {
                var neighbor = (new Position(position.X, position.Y)).MovePosition(dir);

                //Ensuring that the X and Y are within the Ranges
                if (dir != exclude)
                {
                    if (!this[position].Walls.HasFlag(dir))
                    {
                        var mazeCell = this[neighbor];
                        neighbors.Add(dir, mazeCell);
                    }
                }
            }

            return neighbors;
		}
		#endregion Methods
	}
}
