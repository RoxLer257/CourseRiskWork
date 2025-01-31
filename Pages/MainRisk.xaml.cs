using Risk_Work.Classes;
using System.Linq;
using System.Windows.Controls;
using System.Data.Entity;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Windows;

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
    }
}
