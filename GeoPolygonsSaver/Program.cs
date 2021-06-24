using GeoPolygonsSaver.OpenStreetMap;
using System;

namespace GeoPolygonsSaver
{
	class Program
	{
		static void Main()
		{
			string address = string.Empty;
			int frequency = default;
			string outputFileName = string.Empty;
			while (address == string.Empty)
			{
				Console.WriteLine("Введите адрес.");
				address = Console.ReadLine();
			}

			while (frequency <= 0)
			{
				Console.WriteLine("Введите частоту сохранения точек полигона(1 если оптимизация не нужна).");
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
			PolygonWriter polygonWriter = new PolygonWriter(new OSMGeoService(address));
			polygonWriter.Start(frequency, outputFileName);
		}
	}
}
