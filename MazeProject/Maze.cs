using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MazeProject
{
	public class Maze
	{
		#region Fields

		private MazeCellArray m_Cells;
		private bool m_MazeCreated = false;
		ManualResetEvent m_Done = new ManualResetEvent(false);

		#endregion Fields

		#region Properties
		/// <summary>
		/// Indexer provides indirect access to individual maze cells
		/// </summary>
		/// <param name="x">X ordinate</param>
		/// <param name="y">Y ordinate</param>
		/// <returns>Maze cell at given coordinates</returns>
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

		/// <summary>
		/// Gets the cell at the given position
		/// </summary>
		/// <param name="position">Position of cell to get</param>
		/// <returns>Cell at given position</returns>
		public MazeCell this[Position position]
		{
			get
			{
				return m_Cells[position.X, position.Y];
			}
			set
			{
				m_Cells[position.X, position.Y] = value;
			}
		}

		/// <summary>
		/// Solution
		/// </summary>
		public List<DirectionEnum> Solution { get; set; }

		/// <summary>
		/// Gets the width of the maze
		/// </summary>
		public int Width
		{
			get
			{
				return m_Cells.Width;
			}
		}

		/// <summary>
		/// Gets the height of the maze
		/// </summary>
		public int Height
		{
			get
			{
				return m_Cells.Height;
			}
		}

		/// <summary>
		/// Starting point in the maze
		/// </summary>
		public Position StartPoint { get; set; }

		/// <summary>
		/// Ending point in the maze
		/// </summary>
		public Position EndPoint { get; set; }

		/// <summary>
		/// Complexity of the maze from 0 to 1.  0 = easy, 1 = hard.
		/// </summary>
		public double Complexity { get; set; }
		#endregion Properties

		#region Constructors
		/// <summary>
		/// Creates a maze with default size
		/// </summary>
		public Maze() : this(Constants.MIN_WIDTH, Constants.MIN_HEIGHT)
		{
		}

		/// <summary>
		/// Creates a maze with the given size
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Maze(int width, int height)
		{
			Resize(width, height);
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Resizes the array to the given dimensions and initializes all the cells
		/// </summary>
		/// <param name="width">New width</param>
		/// <param name="height">New height</param>
		public void Resize(int width, int height)
		{
			// Validate Width and Height

            if (width <= Constants.MAX_WIDTH && width >= Constants.MIN_WIDTH
               && height <= Constants.MAX_HEIGHT && height >= Constants.MIN_HEIGHT)
            {
                m_Cells = new MazeCellArray(width, height);
                m_MazeCreated = false;
            }
		}
				
		/// <summary>
		/// Creates a new maze using the existing Maze size
		/// </summary>
		/// <param name="complexity">Complexity of the maze from 0 to 1.  0 = easy, 1 = hard.</param>
		public void CreateMaze(double complexity)
		{
			try
			{
				Complexity = complexity;
				DateTime startTime = DateTime.Now;
				// Reset the maze cells to their initial states
				m_Cells.Reset();
				
				// Create starting and ending point (must be distinct)
				StartPoint = Position.GetRandomPosition(m_Cells.XRange, m_Cells.YRange);
				do
				{
					EndPoint = Position.GetRandomPosition(m_Cells.XRange, m_Cells.YRange);
				} while (EndPoint == StartPoint);

				ProcessCell(m_Cells[StartPoint], DirectionEnum.None);
				DateTime endTime = DateTime.Now;
				Console.WriteLine("Maze creation time: {0} ms", endTime.Subtract(startTime).TotalMilliseconds);
				m_MazeCreated = true;
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		/// <summary>
		/// Process the given cell
		/// </summary>
		/// <param name="cell">Cell to process</param>
		/// <param name="heading">Direction taken to arrive at current cell</param>
		private void ProcessCell(MazeCell cell, DirectionEnum heading)
		{
            // Get a list of neighbor (N, S, E, W) cells to the one passed in that have all walls intact
            var neighbors = m_Cells.GetNeighborCellsWithAllWalls(cell.Position);
            if (neighbors.Count == 0)
            {
                // No neighbors, this cell is processed
                return;
            }
            // Check for propensity to stay in same direction
            DirectionEnum newHeading = DirectionEnum.None;
            double chance = Utils.Rnd.NextDouble();
            if ((chance > Complexity) && (neighbors.ContainsKey(heading)))
            {
                newHeading = heading;
            }
            // Process the remaining cells in a random order
            while (neighbors.Count > 0)
            {
                if (newHeading == DirectionEnum.None)
                {
                    // Get a random neighbor
                    newHeading = neighbors.Keys.GetRandomValue();
                }
                MazeCell neighbor = neighbors[newHeading];
                if (neighbor.Walls == DirectionEnum.All)
                {
                    m_Cells.RemoveCellWall(cell.Position, newHeading);
                    ProcessCell(neighbor, newHeading);
                }
                neighbors.Remove(newHeading);
                newHeading = DirectionEnum.None;
            }
		}

		/// <summary>
		/// Solves the maze
		/// </summary>
		/// <returns></returns>
		public bool SolveMaze()
		{
			if (!m_MazeCreated) throw new Exception("Maze not created!");

			DateTime startTime = DateTime.Now;
			MazeCell start = m_Cells[StartPoint];
			m_Done.Reset();
			SolveMazePath(start, DirectionEnum.None, new List<DirectionEnum>());

			if (!m_Done.WaitOne(30000))
			{
				// Taking too long - something bad happened
				m_Done.Set();
				throw new TimeoutException("Maze solving taking too long.");
			}
			DateTime endTime = DateTime.Now;
			Console.WriteLine("Maze solve time: {0} ms", endTime.Subtract(startTime).TotalMilliseconds);
			return true;
			
		}

		/// <summary>
		/// Processes the maze solution down a given path
		/// </summary>
		/// <param name="cell">Cell to process on current path</param>
		/// <param name="heading">Heading taken to reach current cell</param>
		/// <param name="directions">List of directions tracing the path taken so far</param>
		private void SolveMazePath(MazeCell cell, DirectionEnum heading, List<DirectionEnum> directions)
		{
            if (m_Done.WaitOne(1000))
            {
                return;
            }

            if (directions.Count > (Width * Height))
            {
                m_Done.Set();
                throw new Exception("directions' length is greater than the number of moves possible to solve the maze!");

            }

            if (heading != DirectionEnum.None)
                directions.Add(heading);

           // Check if cell's Position property equals EndPoint. If it does,
           // we have found the solution. Set Solution equal to directions, 
           // call Set() on m_Done and return from the method

            if (cell.Position == EndPoint)
            {
                Solution = directions;
                m_Done.Set();
                return;
            }

            var neighbors = m_Cells.GetAccessibleNeighborCells(cell.Position, EnumMethods.GetOpposite(heading));

            if (neighbors == null || neighbors.Count == 0)
                return;

            if (neighbors.Count > 1)
            {
                foreach (var direction in neighbors.Keys)
                {
                    Task.Run(() => { SolveMazePath(neighbors[direction], direction, new List<DirectionEnum>(directions)); });
                }
            }
            else
            {
                SolveMazePath(neighbors.Values.First(), neighbors.Keys.First(), directions);
            }
           
		}
		#endregion Methods
	}
}
