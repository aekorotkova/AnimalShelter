using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AnimalShelter
{
    public static class ExcelExporter
    {
        /// <summary>
        /// Экспортирует список объектов в CSV-файл на рабочий стол.
        /// </summary>
        /// <typeparam name="T">Тип объектов</typeparam>
        /// <param name="data">Список данных</param>
        /// <param name="fileNamePrefix">Префикс имени файла</param>
        /// <returns>Полный путь к созданному файлу</returns>
        public static string ExportToCsv<T>(List<T> data, string fileNamePrefix)
        {
            if (data == null || data.Count == 0)
                throw new InvalidOperationException("Нет данных для экспорта.");

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"{fileNamePrefix}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
            string fullPath = Path.Combine(desktopPath, fileName);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var sb = new StringBuilder();

            for (int i = 0; i < properties.Length; i++)
            {
                if (i > 0) sb.Append(";");
                sb.Append(EscapeCsv(properties[i].Name));
            }
            sb.AppendLine();

            foreach (var item in data)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i > 0) sb.Append(";");

                    var value = properties[i].GetValue(item);
                    string text = FormatValue(value);
                    sb.Append(EscapeCsv(text));
                }
                sb.AppendLine();
            }

            File.WriteAllText(fullPath, sb.ToString(), new UTF8Encoding(true));

            return fullPath;
        }

        private static string FormatValue(object value)
        {
            if (value == null) return "";

            if (value is DateTime dt)
                return dt.ToString("dd.MM.yyyy");

            if (value is DateTime?)
            {
                var nullableDt = (DateTime?)value;
                return nullableDt.HasValue ? nullableDt.Value.ToString("dd.MM.yyyy") : "";
            }

            return value.ToString();
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            if (value.Contains(";") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}