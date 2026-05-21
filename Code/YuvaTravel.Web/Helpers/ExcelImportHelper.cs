using ClosedXML.Excel;

namespace YuvaTravel.Web.Helpers
{
    public static class ExcelImportHelper
    {
        /// <summary>
        /// Read Excel and map rows to DTO by column header name
        /// </summary>
        public static List<T> ReadExcel<T>(string filePath) where T : new()
        {
            var result = new List<T>();
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();

            var headerRow = worksheet.Row(1);
            var headers = new Dictionary<string, int>();
            foreach (var cell in headerRow.CellsUsed())
            {
                headers[cell.Value.ToString().Trim()] = cell.Address.ColumnNumber;
            }

            var properties = typeof(T).GetProperties();
            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int rowNum = 2; rowNum <= lastRow; rowNum++)
            {
                var row = worksheet.Row(rowNum);
                var item = new T();
                foreach (var prop in properties)
                {
                    if (headers.TryGetValue(GetColumnName<T>(prop.Name), out int colNum))
                    {
                        var cell = row.Cell(colNum);
                        try
                        {
                            if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                            {
                                var val = cell.Value.ToString().Trim();
                                prop.SetValue(item, val == "1" || val.ToLower() == "true" || val == "是" || val == "Y");
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(item, cell.Value.ToString().Trim());
                            }
                            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                            {
                                if (int.TryParse(cell.Value.ToString(), out int intVal))
                                    prop.SetValue(item, intVal);
                            }
                            else
                            {
                                var converted = Convert.ChangeType(cell.Value.ToString(), prop.PropertyType);
                                prop.SetValue(item, converted);
                            }
                        }
                        catch { }
                    }
                }
                result.Add(item);
            }

            return result;
        }

        private static readonly Dictionary<string, Dictionary<string, string>> _columnMappings = new();

        public static void AddMapping<T>(string propertyName, string columnHeader)
        {
            var typeName = typeof(T).FullName!;
            if (!_columnMappings.ContainsKey(typeName))
                _columnMappings[typeName] = new Dictionary<string, string>();
            _columnMappings[typeName][propertyName] = columnHeader;
        }

        private static string GetColumnName<T>(string propertyName)
        {
            var typeName = typeof(T).FullName!;
            if (_columnMappings.TryGetValue(typeName, out var mappings))
            {
                if (mappings.TryGetValue(propertyName, out var colName))
                    return colName;
            }
            return propertyName;
        }

        public static List<T> ReadExcelWithMappings<T>(string filePath, Dictionary<string, string> propertyToHeader) where T : new()
        {
            var result = new List<T>();
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();

            var headerRow = worksheet.Row(1);
            var headers = new Dictionary<string, int>();
            foreach (var cell in headerRow.CellsUsed())
            {
                headers[cell.Value.ToString().Trim()] = cell.Address.ColumnNumber;
            }

            var properties = typeof(T).GetProperties();
            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int rowNum = 2; rowNum <= lastRow; rowNum++)
            {
                var row = worksheet.Row(rowNum);
                var item = new T();
                foreach (var prop in properties)
                {
                    if (!propertyToHeader.TryGetValue(prop.Name, out var headerName)) continue;
                    if (!headers.TryGetValue(headerName, out int colNum)) continue;

                    var cell = row.Cell(colNum);
                    try
                    {
                        if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            var val = cell.Value.ToString().Trim();
                            prop.SetValue(item, val == "1" || val.ToLower() == "true" || val == "是" || val == "Y");
                        }
                        else if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(item, cell.Value.ToString().Trim());
                        }
                        else
                        {
                            var converted = Convert.ChangeType(cell.Value.ToString(), prop.PropertyType);
                            prop.SetValue(item, converted);
                        }
                    }
                    catch { }
                }
                result.Add(item);
            }

            return result;
        }
    }
}
