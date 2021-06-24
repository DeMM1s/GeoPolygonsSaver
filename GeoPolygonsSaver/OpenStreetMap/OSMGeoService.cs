using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GeoPolygonsSaver.OpenStreetMap
{
	public class OSMGeoService : IGeoService
	{
		private string _url = null;
		private HttpWebResponse _webResponse;
		private int _countOfPolygons = 0;
		private string _geoData = null;
		public OSMGeoService(string address)
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

		public string GetPolygons(int frequency)
		{
			string polygons = null;
			GetGeoData();
			if (_geoData != null)
			{
				if (_countOfPolygons == 1)
				{
					polygons = GetPolygon(frequency);
				}
				else
				{
					for (int i = 0; i < _countOfPolygons; i++)
					{
						polygons += GetPolygon(i, frequency);
					}
				}
			}
			return polygons;
		}

		private void GetGeoData()
		{
			using (var streamReader = new StreamReader(_webResponse.GetResponseStream()))
			{
				_geoData = streamReader.ReadToEnd();
				if (_geoData != null)
				{
					OSMJson deserializeJson = JsonConvert.DeserializeObject<OSMJson>(_geoData);
					_geoData = GetPolygonsFromJson(deserializeJson);
				}
			}
		}

		private string GetPolygonsFromJson(OSMJson deserializeJson)
		{
			string polygons = null;
			List<Feature> features = deserializeJson.Features.Where(f => f.Properties.Category == "place" || f.Properties.Category == "boundary").ToList();
			if (features.Count > 1)
			{
				Console.WriteLine("Указан неточный адрес, по введенному адресу нашлось несколько совпадений, сохранено будет только первое совпадение из полученного списка");
			}
			Feature feature = features[0];
			if (feature.Geometry.Type == "Polygon")
			{
				polygons += feature.Geometry.Coordinates[0].ToString();
				_countOfPolygons++;

			}
			if (feature.Geometry.Type == "MultiPolygon")
			{
				polygons += feature.Geometry.Coordinates[0].ToString().Remove(feature.Geometry.Coordinates[0].ToString().Length - 3, 3) + ",";
				_countOfPolygons++;
				for (int i = 1; i < feature.Geometry.Coordinates.Count; i++)
				{
					polygons += feature.Geometry.Coordinates[i].ToString().Remove(0, 2);
					if (i + 1 != feature.Geometry.Coordinates.Count)
					{
						polygons = polygons.Remove(polygons.Length - 2, 2) + ",";
					}
					_countOfPolygons++;
				}
			}
			return polygons;
		}

		private string GetPolygon(int frequency)
		{
			string polygon = null;
			int countOfPoint = 0;
			List<List<double>> deserializePolygon = JsonConvert.DeserializeObject<List<List<double>>>(_geoData);
			for (int i = 0; i < deserializePolygon.Count; i += frequency)
			{
				polygon += "[" + deserializePolygon[i][0].ToString() + ", " + deserializePolygon[i][1].ToString() + "]";
				if (i + frequency < deserializePolygon.Count)
				{
					polygon += ", ";
				}
				countOfPoint++;
			}
			Console.WriteLine($"Сохранено {countOfPoint} точек из {deserializePolygon.Count} точек полигона");
			return polygon;
		}

		private string GetPolygon(int currentPolygon, int frequency)
		{
			string polygon = null;
			int countOfPoint = 0;
			List<List<List<double>>> deserializePolygon = JsonConvert.DeserializeObject<List<List<List<double>>>>(_geoData);
			for (int i = 0; i < deserializePolygon[currentPolygon].Count; i += frequency)
			{
				polygon += "[" + deserializePolygon[currentPolygon][i][0].ToString() + ", " + deserializePolygon[currentPolygon][i][1].ToString() + "]";
				if (i + frequency < deserializePolygon[currentPolygon].Count)
				{
					polygon += ", ";
				}
				countOfPoint++;
			}
			Console.WriteLine($"Сохранено {countOfPoint} точек из {deserializePolygon[currentPolygon].Count} точек в полигоне {currentPolygon} из мультиполигона");
			return polygon;
		}
	}
}
