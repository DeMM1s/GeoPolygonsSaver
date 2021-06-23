using System;
using System.IO;

namespace GeoPolygonsSaver
{
	public class PolygonWriter
	{
		private IGeoService _geoService;
		public PolygonWriter(IGeoService geoService)
		{
			_geoService = geoService;
		}

		public void Start()
		{
			string address = string.Empty;
			while (address == string.Empty)
			{
				Console.WriteLine("Введите адрес.");
				address = Console.ReadLine();
			}
			_geoService.SetUrl(address);
			if (_geoService.GetResponse())
			{
				string geoData = _geoService.GetGeoData();
				WritePoligonsToFile(geoData, address);
				Console.WriteLine($"Запись прошла успешно. Результаты сохранены в файл {address}.txt в директории программы.");
				Console.WriteLine("Нажмите любую клавишу для завершения...");
			}
			else
			{
				Console.WriteLine("Что то пошло не так...");
			}
			Console.ReadLine();
		}

		private void WritePoligonsToFile(string geoData, string fileName)
		{
			FileStream file = new FileStream($"{fileName}.txt", FileMode.OpenOrCreate);
			StreamWriter streamWriter = new StreamWriter(file);
			streamWriter.Write(geoData);
			streamWriter.Close();
			file.Close();
		}
	}
}
