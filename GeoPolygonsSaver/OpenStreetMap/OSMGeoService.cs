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
		private string _url;
		private HttpWebResponse _webResponse;
		private int _countOfPolygons = 0;
		private string _geoData;
		private string _polygons;
		private int _frequency;
		public OSMGeoService(string address, int frequency)
		{
			_frequency = frequency;
			string email = string.Empty;
			while (email == string.Empty)
			{
				Console.WriteLine("Для доступа к API OSM необходима активированная учетная запись, пожалуйста, введите Email вашего аккаунта OSM.");
				email = Console.ReadLine();
			}
			_url = "https://nominatim.openstreetmap.org/search?q=" + address.Replace(' ', '+') + "&format=geojson&polygon_geojson=1" + "&email=" + email;
		}

		public bool GetResponse()
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
			httpWebRequest.ContentType = "text/json";
			httpWebRequest.Method = "GET";
			try
			{
				_webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			}
			catch (WebException e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
			return _webResponse.StatusCode == HttpStatusCode.OK;
		}

		public string GetPolygons()
		{
			GetGeoData();
			if (_geoData != null)
			{
				if (_countOfPolygons == 1)
				{
					List<List<double>> deserializePolygon = JsonConvert.DeserializeObject<List<List<double>>>(_geoData);
					_polygons = CreatePolygonWithFrequencyPoints(deserializePolygon);
				}
				else
				{
					List<List<List<double>>> deserializeMultiPolygon = JsonConvert.DeserializeObject<List<List<List<double>>>>(_geoData);
					_polygons = CreateMultiPolygonWithFrequencyPoints(deserializeMultiPolygon); 
				}
			}
			return _polygons;
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

		private string CreatePolygonWithFrequencyPoints(List<List<double>> deserializePolygon)
		{
			string polygon = "[";
			int countOfPoint = 0;
			for (int i = 0; i < deserializePolygon.Count; i += _frequency)
			{
				polygon += "[" + deserializePolygon[i][0].ToString().Replace(',', '.') + ", " + deserializePolygon[i][1].ToString().Replace(',', '.') + "]";
				if (i + _frequency < deserializePolygon.Count)
				{
					polygon += ",";
				}
				countOfPoint++;
			}
			Console.WriteLine($"Сохранено {countOfPoint} точек из {deserializePolygon.Count} точек полигона");
			return polygon + "]";
		}

		private string CreateMultiPolygonWithFrequencyPoints(List<List<List<double>>> deserializeMultiPolygon)
		{
			string polygons = "[";
			Console.WriteLine($"Геообъект является мультиполигоном, состоящим из {deserializeMultiPolygon.Count} полигонов");
			for (int i = 0; i < deserializeMultiPolygon.Count; i++)
			{
				polygons += CreatePolygonWithFrequencyPoints(deserializeMultiPolygon[i]);
				if (i + 1 != deserializeMultiPolygon.Count)
				{
					_polygons += ",";
				}
			}
			polygons += "]";
			return polygons;
		}
	}
}
