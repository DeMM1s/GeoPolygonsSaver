namespace GeoPolygonsSaver
{
	public interface IGeoService
	{
		void SetUrl(string address);
		bool GetResponse();
		string GetGeoData();
	}
}
