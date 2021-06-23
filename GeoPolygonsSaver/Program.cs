using GeoPolygonsSaver.OpenStreetMap;

namespace GeoPolygonsSaver
{
	class Program
	{
		static void Main()
		{
			PolygonWriter polygonWriter = new PolygonWriter(new OSMGeoService());
			polygonWriter.Start();
		}
	}
}
