using System;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SearchAndOutputOfInformationInAFolder
{

    public partial class Form1 : Form
    {
        public static string MainDirectory { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private Excel.Application ExcelApplication;
        private Excel.Workbook ExcelWorkbook;
        private Excel.Worksheet ExcelWorksheet;
        private void DefiningADirectory_Click(object sender, EventArgs e)
        {
            SearchDirectory();
        }


        // Метод поиска директории для получения информации о файлах; 
        public void SearchDirectory()
        {
            if (SearchDirectoryFiles.ShowDialog() == DialogResult.OK) MainDirectory = SearchDirectoryFiles.SelectedPath + @"\";
            else MainDirectory = null;
            if (MainDirectory != null)
            {
                if (MessageBox.Show(text: "Вы уверены в правильности выбраной дириктории  для перемещение файлов.", caption: "Подтверждения действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    MessageBox.Show(text: "Выбирите нужную директорию для записи.", caption: "Отмена пользователем", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MainDirectory = null;
                }
            }
            if (MainDirectory != null) CreateExcelPage();
        }
        public void CreateExcelPage()
        {
            try
            {
                ExcelApplication = new Excel.Application();
                ExcelWorkbook = ExcelApplication.Workbooks.Add(1);
                ExcelWorksheet = (Excel.Worksheet)ExcelWorkbook.Sheets[1];
                // Создание оглавлений;
                ExcelWorksheet.Cells[1, "A"] = "NameFiles";
                ExcelWorksheet.Cells[1, "B"] = "FormatFiles";
                ExcelWorksheet.Cells[1, "C"] = "Size (MB)";
                // Запись информации о файлах; 
                long MediumSize = 0;
                int Count = 2;
                foreach (var Files in new DirectoryInfo(MainDirectory).GetFiles())
                {
                    ExcelWorksheet.Cells[Count, "A"] = Files.Name;
                    ExcelWorksheet.Cells[Count, "B"] = Files.Extension;
                    ExcelWorksheet.Cells[Count, "C"] = Files.Length / 1048576;
                    MediumSize += Files.Length / 1048576;
                    Count++;
                }
                // Средний размер файла;
                ExcelWorksheet.Cells[Count, "D"] = "MediumSize";
                ExcelWorksheet.Cells[Count, "E"] = MediumSize / Count - 1;
                // Создание графиков;
                Excel.Range chartRange;
                // Диаграмма;
                Excel.ChartObjects xlCharts = (Excel.ChartObjects)ExcelWorksheet.ChartObjects(Type.Missing);
                Excel.ChartObject myChart = xlCharts.Add(10, 80, 300, 250);
                Excel.Chart chartPage = myChart.Chart;
                chartRange = ExcelWorksheet.get_Range("C1", $"C{Count}");
                chartPage.SetSourceData(chartRange, Type.Missing);
                chartPage.ChartType = Excel.XlChartType.xlColumnClustered;
                // Вторая диаграмма 

                // Сохранение Excel документа;
                ExcelWorkbook.SaveAs(MainDirectory + "InformationAboutYouDirectory" + ".xls", Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                ExcelApplication.Quit();
                MessageBox.Show(text: "Процесс анализа и создания Excel документа закончено! Находится файл .xls в папке, в которой проходило иследование.", caption: "Создание выполнено успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show(text: "Вы не выбрали папку для анализа", caption: "Ошибка пользователя", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(text: "Ошибка получения доступа к файлу, закройте файл из данной директивы и попробуйте снова!", caption: "Ошибка пользователя", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
