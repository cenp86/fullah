using System;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UalaAccounting.api.Models;

namespace UalaAccounting.test
{
	public class FilesProcess
	{
        List<ConfigurationSheetModel> listConfigurationSheet;
        public FilesProcess()
		{
            listConfigurationSheet = LoandConfigurationSheetModel();

        }

		public void ProcessFile()
		{
			string path = "/Users/alexis.salinas@mambu.com/Documents/customers/uala/archivoej/configurationuala.xlsx";
			using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
                IWorkbook workbook = new XSSFWorkbook(file);
				var intNumberOfSheet = workbook.NumberOfSheets;
                var NumberOfSheetConfiguration = listConfigurationSheet.Count();

                if (intNumberOfSheet == NumberOfSheetConfiguration)
				{
                    // Recorrer todas las hojas
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        ISheet sheet = workbook.GetSheetAt(i);
                        string sheetName = sheet.SheetName;
                        var configurationSheet = listConfigurationSheet.FirstOrDefault(x => x.Name == sheetName);
                        // TODO: DEFINIR DONDE PONEMOS LOS NOMBRE DE LOS SHEETS
                        if (configurationSheet == null)
                        {
                            throw new Exception("Sheets are wrong, you have different names.");
                        }

                        if (configurationSheet.RequiredId == true)
                        {
                            IRow headerRow = sheet.GetRow(configurationSheet.StartPositionID);
                            ICell cell = headerRow.GetCell(1);

                            var id = cell.ToString();

                            if (string.IsNullOrEmpty(id))
                            {
                                throw new Exception("You Have to Have id for this sheet.");
                            }
                        }

                        if (!HasExpectedColumns(sheet, configurationSheet.NumberColumn, configurationSheet.StartPositionColumName))
                        {
                            throw new Exception("You have differents numbers of columns.");
                        }
                    }
                }
				else
				{
					throw new Exception("The amount of sheet is wrong, you have to have only 5 sheets.");
				}
            }
        }

        private List<ConfigurationSheetModel> LoandConfigurationSheetModel()
        {
            List<ConfigurationSheetModel> list = new List<ConfigurationSheetModel>();
            ConfigurationSheetModel item = new ConfigurationSheetModel();
            item.Name = "ACCOUNTCHART";
            item.NumberColumn = 5;
            item.StartPositicion = 2;
            item.StartPositionColumName = 1;
            item.RequiredId = true;
            item.StartPositionID = 0;

            list.Add(item);
            ConfigurationSheetModel item1 = new ConfigurationSheetModel();
            item1.Name = "PRODUCT";
            item1.NumberColumn = 3;
            item1.StartPositicion = 0;
            item1.StartPositionColumName = 0;
            item1.StartPositionID = 0;

            list.Add(item1);

            ConfigurationSheetModel item2 = new ConfigurationSheetModel();
            item2.Name = "STAGE1";
            item2.NumberColumn = 12;
            item2.StartPositicion = 0;
            item2.StartPositionColumName = 0;
            item2.StartPositionID = 0;

            list.Add(item2);

            ConfigurationSheetModel item3 = new ConfigurationSheetModel();
            item3.Name = "STAGE2";
            item3.NumberColumn = 12;
            item3.StartPositicion = 0;
            item3.StartPositionColumName = 0;
            item3.StartPositionID = 0;

            list.Add(item3);

            ConfigurationSheetModel item4 = new ConfigurationSheetModel();
            item4.Name = "STAGE3";
            item4.NumberColumn = 12;
            item4.StartPositicion = 0;
            item4.StartPositionColumName = 0;
            item4.StartPositionID = 0;

            list.Add(item4);

            return list;


        }




        static bool HasExpectedColumns(ISheet sheet, int expectedColumns, int startPosition)
        {
            // Obtener la primera fila para determinar el número de columnas
            IRow headerRow = sheet.GetRow(startPosition);
            return headerRow != null && headerRow.LastCellNum == expectedColumns;
        }

        static bool ValidateColumnFormats(ISheet sheet)
        {
            bool allFormatsCorrect = true;
            IRow headerRow = sheet.GetRow(0);
            if (headerRow != null)
            {
                for (int j = 0; j < headerRow.LastCellNum; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    if (cell != null)
                    {
                        // Aquí definirías la lógica para validar el formato específico
                        // Por ejemplo, esperar que la primera columna sea de tipo texto
                        if (j == 0 && cell.CellType != CellType.String)
                        {
                            allFormatsCorrect = false;
                        }
                        // Agregar más condiciones según sea necesario para cada columna
                    }
                }
            }
            return allFormatsCorrect;
        }
    }
}

