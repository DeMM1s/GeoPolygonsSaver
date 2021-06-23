using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GeoPolygonsSaver.OpenStreetMap
{
	public class OSMGeoService : IGeoService
	{
		private string _url;
		private HttpWebResponse _webResponse;
		public void SetUrl(string address)
		{
			Console.WriteLine("Для доступа к API OSM необходима учетная запись, пожалуйста, введите Email вашего аккаунта OSM.");
			string email = Console.ReadLine();
			_url = "https://nominatim.openstreetmap.org/search?q=" + address.Replace(' ', '+') + "&format=geojson&polygon_geojson=1" + "&email=" + email;
		}

		public bool GetResponse()
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
			httpWebRequest.ContentType = "text/json";
			httpWebRequest.Method = "GET";
			_webResponse = (HttpWebResponse)httpWebRequest.GetResponse();

			return _webResponse.StatusCode == HttpStatusCode.OK;
		}

		public string GetGeoData()
		{
			using (var streamReader = new StreamReader(_webResponse.GetResponseStream()))
			{
				var result = streamReader.ReadToEnd();
				if (result != null)
				{
					OSMJson deserializeJson = JsonConvert.DeserializeObject<OSMJson>(result);
					return GetPolygonsFromJson(deserializeJson);
				}
				return null;
			}
		}

		private string GetPolygonsFromJson(OSMJson deserializeJson)
		{
			string polygons = null;
			foreach (Feature item in deserializeJson.Features)
			{
				if (item.Geometry.Type == "Polygon")
				{
					for (int i = 0; i < item.Geometry.Coordinates.Count; i++)
					{
						polygons += item.Geometry.Coordinates[i].ToString();
					}
				}
			}
			return polygons;
		}
	}
}
