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

		public void Start(int frequency, string outputFileName)
		{
			if (_geoService.GetResponse())
			{
				string geoData = _geoService.GetPolygons(frequency);
				if (geoData != null)
				{
					WritePoligonsToFile(geoData, outputFileName);
					Console.WriteLine($"Запись прошла успешно. Результаты сохранены в файл {outputFileName}.txt в директории программы.");
				}
				else
				{
					Console.WriteLine("Нет информации об адресе.");
				}
			}
			else
			{
				Console.WriteLine("Не удалось получить ответ от сервера...");
			}
			Console.WriteLine("Нажмите любую клавишу для завершения...");
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
