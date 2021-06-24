using System.Collections.Generic;

namespace GeoPolygonsSaver
{
	public class Polygon
	{
		public List<Point> Points { get; set; }
	}

	public class Point
	{
		public double Xcoordinate { get; set; }
		public double Ycoordinate { get; set; }
	}
}
