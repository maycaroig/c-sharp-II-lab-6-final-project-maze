using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeProject
{
	public struct Position
	{
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public static bool operator ==(Position p1, Position p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y); 
        }

        public static bool operator !=(Position p1, Position p2) 
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj) 
        {
            if (obj is Position)
            { 
                return (this == (Position)obj);
            } 
            return false;
        }

        public override int GetHashCode()
        {
            return X ^ Y; 
        }

        public static Position GetRandomPosition(Range<int> horizontal, Range<int> vertical)
        {
            int x = Utils.Rnd.Next(horizontal.Min, horizontal.Max + 1);
            int y = Utils.Rnd.Next(vertical.Min, vertical.Max + 1);
            return new Position(x, y);
        }

        public Position MovePosition(DirectionEnum direction)
        {
            int x = X;
            int y = Y;

            if (direction.HasFlag(DirectionEnum.North))
            {
                y--; 
            } 
            if (direction.HasFlag(DirectionEnum.South))
            {
                y++;
            } 
            if (direction.HasFlag(DirectionEnum.East)) 
            {
                x++;
            }
            if (direction.HasFlag(DirectionEnum.West))
            {
                x--;
            }
            return new Position(x, y);
        }
	}
}
