using Risk_Work.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace Risk_Work.Pages
{
    /// <summary>
    /// Логика взаимодействия для ListMitigationStrategies.xaml
    /// </summary>
    public partial class ListMitigationStrategies : Page
    {
        public ListMitigationStrategies()
        {
            InitializeComponent();
            var mitigarstrateg = RiskBDEntities.GetContext().MitigationStrategies
                .Include(ms => ms.Risks)
                .Include(ms => ms.Risks.RiskCategories).ToList();
            DtgMitStrat.ItemsSource = mitigarstrateg;
            CheckUserRole();
        }


        public void CheckUserRole()
        {
            if (ClassFrame.UserID == 3)
            {
                AddMitStrag.Visibility = Visibility.Collapsed;
                DelStrag.Visibility = Visibility.Collapsed;
                EditStrateg.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddMitStrag.Visibility = Visibility.Visible;
                DelStrag.Visibility = Visibility.Visible;
                EditStrateg.Visibility = Visibility.Visible;
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TxtSearch.Text;
            var strategySearch = RiskBDEntities.GetContext().MitigationStrategies.
                Where(r => r.StrategyName.Contains(search)).ToList();

            DtgMitStrat.ItemsSource = strategySearch;
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            StartDate.SelectedDate = null;
            EndDate.SelectedDate = null;
            DtgMitStrat.ItemsSource = RiskBDEntities.GetContext().MitigationStrategies.ToList();
            TxtSearch.Text = "";
        }

        private void BtnRisk_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.frmObj.Navigate(new MainRisk());
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

                var query = RiskBDEntities.GetContext().MitigationStrategies
                    .Include(ra => ra.Risks)
                    .Include(r => r.Risks.RiskCategories)
                    .AsQueryable();

                if (startDate.HasValue)
                {
                    query = query.Where(ra => ra.ImplementationDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(ra => ra.ImplementationDate <= endDate.Value);
                }

                DtgMitStrat.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации по дате: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMitStrag_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.frmObj.Navigate(new AddEditMitigationStrategies(this, null));
        }
        private void DelStrag_Click(object sender, RoutedEventArgs e)
        {
            var strategiesForRemoving = DtgMitStrat.SelectedItems.Cast<MitigationStrategies>().ToList();

            if (strategiesForRemoving.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите стратегии для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить {strategiesForRemoving.Count} стратегию(ий)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new RiskBDEntities())
                    {
                        foreach (var strategy in strategiesForRemoving)
                        {
                            var mitigationStrategy = context.MitigationStrategies
                                .FirstOrDefault(ms => ms.StrategyID == strategy.StrategyID);
                            if (mitigationStrategy != null)
                            {
                                context.MitigationStrategies.Remove(mitigationStrategy);
                            }
                        }

                        context.SaveChanges();

                        context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                        DtgMitStrat.ItemsSource = context.MitigationStrategies
                            .Include(ms => ms.Risks)
                            .Include(ms => ms.Risks.RiskCategories)
                            .ToList();

                        MessageBox.Show("Стратегии удалены.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }


        private void EditStrateg_Click(object sender, RoutedEventArgs e)
        {
            if (DtgMitStrat.SelectedItem is MitigationStrategies selectedStrategy)
            {
                ClassFrame.frmObj.Navigate(new AddEditMitigationStrategies(this, selectedStrategy));
            }
            else
            {
                MessageBox.Show("Выберите стратегию для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
