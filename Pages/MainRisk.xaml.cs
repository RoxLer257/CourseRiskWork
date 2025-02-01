using Risk_Work.Classes;
using System.Linq;
using System.Windows.Controls;
using System.Data.Entity;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Windows;
using System.Drawing;
using System.Data;
using System.IO;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.Windows.Media;

namespace Risk_Work.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainRisk.xaml
    /// </summary>
    public partial class MainRisk : Page
    {
        public MainRisk()
        {
            InitializeComponent();
            UpdateRiskList();
            CheckUserRole();
        }

        public void CheckUserRole()
        {
            if(ClassFrame.UserID == 2)
            {
                BtnStatic.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnStatic.Visibility= Visibility.Visible;
            }

            if(ClassFrame.UserID == 3)
            {
                EditRisk.Visibility = Visibility.Collapsed;
                DelRisk.Visibility = Visibility.Collapsed;
                AddRisk.Visibility = Visibility.Collapsed;
            }
            else
            {
                EditRisk.Visibility = Visibility.Visible;
                DelRisk.Visibility = Visibility.Visible;
                AddRisk.Visibility = Visibility.Visible;
            }
        }
        public void UpdateRiskList()
        {
            DtgRisks.ItemsSource = RiskBDEntities.GetContext().RiskAssessments
                .Include(r => r.Risks)
                .Include(r => r.RiskLevels)  
                .ToList();
        }


        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TxtSearch.Text;
            var riskSearch = RiskBDEntities.GetContext().RiskAssessments.
                Where(r => r.Risks.RiskName.Contains(search)).ToList();

            DtgRisks.ItemsSource = riskSearch;
        }

        private void BtnMitStrat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClassFrame.frmObj.Navigate(new ListMitigationStrategies());
        }

        private void ResetFilters_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartDate.SelectedDate = null;
            EndDate.SelectedDate = null;
            DtgRisks.ItemsSource = RiskBDEntities.GetContext().RiskAssessments.ToList();
            TxtSearch.Text = "";
        }

        private void AddRisk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClassFrame.frmObj.Navigate(new AddEditRisk(this, null));
        }
        private void DelRisk_Click(object sender, RoutedEventArgs e)
        {
            var risksForRemoving = DtgRisks.SelectedItems.Cast<RiskAssessments>().ToList();

            if (risksForRemoving.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите риски для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить {risksForRemoving.Count()} риск(и)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new RiskBDEntities())
                    {
                        var riskIdsToCheck = risksForRemoving.Select(r => r.RiskID).Distinct().ToList();

                        foreach (var riskAssessment in risksForRemoving)
                        {
                            var assessment = context.RiskAssessments.FirstOrDefault(r => r.AssessmentID == riskAssessment.AssessmentID);
                            if (assessment != null)
                            {
                                context.RiskAssessments.Remove(assessment);
                            }
                        }

                        context.SaveChanges();

                        foreach (var riskID in riskIdsToCheck)
                        {
                            bool isRiskUsedElsewhere = context.RiskAssessments.Any(ra => ra.RiskID == riskID);

                            if (!isRiskUsedElsewhere)
                            {
                                var risk = context.Risks.FirstOrDefault(r => r.RiskID == riskID);
                                if (risk != null)
                                {
                                    context.Risks.Remove(risk);
                                }
                            }
                        }

                        context.SaveChanges();

                        context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());

                        DtgRisks.ItemsSource = context.RiskAssessments
                            .Include(r => r.Risks)
                            .Include(r => r.RiskLevels)
                            .ToList();

                        MessageBox.Show("Данные удалены");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
        private void EditRisk_Click(object sender, RoutedEventArgs e)
        {
            if (DtgRisks.SelectedItem is RiskAssessments selectedRisk)
            {
                NavigationService.Navigate(new AddEditRisk(this, selectedRisk));
            }
            else
            {
                MessageBox.Show("Выберите риск для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LowLvlRisk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string lowLvlRisk = "Низкий";

                var risk = RiskBDEntities.GetContext().RiskAssessments.Include("RiskLevels").
                    Where(r => r.RiskLevels.LevelName == lowLvlRisk).ToList();

                DtgRisks.ItemsSource = risk;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке уровня риска: {ex.Message}");
            }
        }

        private void MidlLvlRisk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string midlLvlRisk = "Средний";

                var risk = RiskBDEntities.GetContext().RiskAssessments.Include("RiskLevels").
                    Where(r => r.RiskLevels.LevelName == midlLvlRisk).ToList();

                DtgRisks.ItemsSource = risk;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке уровня риска: {ex.Message}");
            }
        }
        private void HighLvlRisk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string highLvlRisk = "Высокий";

                var risk = RiskBDEntities.GetContext().RiskAssessments.Include("RiskLevels").
                    Where(r => r.RiskLevels.LevelName == highLvlRisk).ToList();

                DtgRisks.ItemsSource = risk;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке уровня риска: {ex.Message}");
            }
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? startDate = StartDate.SelectedDate;
                DateTime? endDate = EndDate.SelectedDate;

                if (startDate > endDate)
                {
                    MessageBox.Show("Дата начала не может быть позже даты окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var query = RiskBDEntities.GetContext().RiskAssessments
                    .Include(ra => ra.Risks)
                    .Include(r => r.Risks.RiskCategories)
                    .Include(ra => ra.RiskLevels)
                    .AsQueryable();

                if (startDate.HasValue)
                {
                    query = query.Where(ra => ra.AssessmentDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(ra => ra.AssessmentDate <= endDate.Value);
                }

                DtgRisks.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации по дате: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnStatic_Click(object sender, RoutedEventArgs e)
        {
            GenerateRiskReport();
        }

        private void GenerateRiskReport()
        {
            try
            {
                var highRisks = RiskBDEntities.GetContext().RiskAssessments
                    .Include(r => r.Risks)
                    .Include(r => r.RiskLevels)
                    .Include(r => r.Risks.RiskCategories)
                    .Where(r => r.RiskLevels.LevelName == "Высокий")
                    .ToList();

                if (!highRisks.Any())
                {
                    MessageBox.Show("Нет данных о высоких рисках.");
                    return;
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Высокие риски");

                    string[] headers = { "Риск", "Описание", "Вероятность", "Влияние", "Уровень риска", "Дата оценки", "Категория" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    using (ExcelRange range = worksheet.Cells["A1:G1"])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 216, 230));
                    }

                    int row = 2;
                    foreach (var risk in highRisks)
                    {
                        worksheet.Cells[row, 1].Value = risk.Risks.RiskName;
                        worksheet.Cells[row, 2].Value = risk.Risks.Description;
                        worksheet.Cells[row, 3].Value = risk.Risks.Probability;
                        worksheet.Cells[row, 4].Value = risk.Risks.Impact;
                        worksheet.Cells[row, 5].Value = risk.RiskLevels.LevelName;
                        worksheet.Cells[row, 6].Value = risk.AssessmentDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[row, 7].Value = risk.Risks.RiskCategories.CategoryName;
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    var chart = worksheet.Drawings.AddChart("RiskChart", eChartType.ColumnClustered);
                    chart.Title.Text = "Анализ рисков";

                    var xValues = worksheet.Cells["A2:A" + (row - 1)];
                    var yValuesProbability = worksheet.Cells["C2:C" + (row - 1)];
                    var yValuesImpact = worksheet.Cells["D2:D" + (row - 1)];

                    chart.Series.Add(yValuesProbability, xValues).Header = "Вероятность";
                    chart.Series.Add(yValuesImpact, xValues).Header = "Влияние";

                    chart.SetPosition(0, 150, 8, 0);
                    chart.SetSize(800, 400);

                    string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excel report");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    string filePath = Path.Combine(directoryPath, "Risk_Report.xlsx");

                    excelPackage.SaveAs(new FileInfo(filePath));
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n\nДетали: {ex.InnerException?.Message}");
            }
        }
    }
}
