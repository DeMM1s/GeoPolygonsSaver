using System.Collections.Generic;

namespace GeoPolygonsSaver.OpenStreetMap
{
	public class OSMJson
	{
		public string Type { get; set; }
		public string Licence { get; set; }
		public List<Feature> Features { get; set; }
	}

	public class Properties
	{
		public int Place_id { get; set; }
		public string Osm_type { get; set; }
		public long Osm_id { get; set; }
		public string Display_name { get; set; }
		public int Place_rank { get; set; }
		public string Category { get; set; }
		public string Type { get; set; }
		public double Importance { get; set; }
		public string Icon { get; set; }
	}

	public class Geometry
	{
		public string Type { get; set; }
		public List<object> Coordinates { get; set; }
	}

	public class Feature
	{
		public string Type { get; set; }
		public Properties Properties { get; set; }
		public List<double> Bbox { get; set; }
		public Geometry Geometry { get; set; }
	}
}
