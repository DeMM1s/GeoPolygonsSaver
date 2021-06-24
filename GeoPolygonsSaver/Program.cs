using GeoPolygonsSaver.OpenStreetMap;
using System;
using System.IO;

namespace GeoPolygonsSaver
{
	class Program
	{
		static string address = string.Empty;
		static int frequency = 0;
		static string outputFileName = string.Empty;
		static void Main()
		{
			GetInfoForRequest();
			IGeoService geoService = new OSMGeoService(address, frequency);
			if (geoService.GetResponse())
			{
				string geoData = geoService.GetPolygons();
				if (geoData != null)
				{
					WritePoligonsToFile(geoData, outputFileName);
					Console.WriteLine($"Запись прошла успешно. Результаты сохранены в файл {outputFileName}.txt в директории программы.");
				}
				else
				{
					Console.WriteLine("Нет информации об указанном адресе.");
				}
			}
			Console.WriteLine("Нажмите любую клавишу для завершения...");
			Console.ReadLine();
		}

		static void GetInfoForRequest()
		{
			while (address == string.Empty)
			{
				Console.WriteLine("Введите адрес.");
				address = Console.ReadLine();
			}

			while (frequency <= 0)
			{
				Console.WriteLine("Введите частоту(число больше 0) сохранения точек полигона(1 если оптимизация не нужна).");
				string answer = Console.ReadLine();
				if (!Int32.TryParse(answer, out frequency))
				{
					Console.WriteLine("Неверный формат ввода.");
				}
			}
			while (outputFileName == string.Empty)
			{
				Console.WriteLine("Введите название файла для сохранения результатов.");
				outputFileName = Console.ReadLine();
			}
		}

		static void WritePoligonsToFile(string geoData, string fileName)
		{
			StreamWriter streamWriter = new StreamWriter($"{fileName}.txt", false);
			streamWriter.Write(geoData);
			streamWriter.Close();
		}
	}
}
