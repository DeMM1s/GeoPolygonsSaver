namespace GeoPolygonsSaver
{
	public interface IGeoService
	{
		bool GetResponse();
		string GetPolygons(int frequency);
	}
}
