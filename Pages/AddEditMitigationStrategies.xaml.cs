using Risk_Work.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace Risk_Work.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditMitigationStrategies.xaml
    /// </summary>
    
    public partial class AddEditMitigationStrategies : Page
    {
        private MitigationStrategies _strategy;
        private ListMitigationStrategies _parentPage;

        public AddEditMitigationStrategies(ListMitigationStrategies parentPage, MitigationStrategies strategy)
        {
            InitializeComponent();
            DataContext = this;
            _parentPage = parentPage;
            _strategy = strategy;

            CmbRiskName.ItemsSource = RiskBDEntities.GetContext().Risks.ToList();
            CmbRiskName.SelectedValuePath = "RiskID";
            CmbRiskName.DisplayMemberPath = "RiskName";

            CmbCatRisk.ItemsSource = RiskBDEntities.GetContext().RiskCategories.ToList();
            CmbCatRisk.SelectedValuePath = "CategoryID";
            CmbCatRisk.DisplayMemberPath = "CategoryName";

            if (_strategy != null)
            {
                Risks risks = RiskBDEntities.GetContext().Risks.FirstOrDefault(r => r.RiskID == _strategy.RiskID);
                if (risks != null)
                {
                    CmbRiskName.SelectedValue = risks.RiskID;
                    CmbCatRisk.SelectedValue = risks.CategoryID;
                    DeskKatRisk.Text = risks.RiskCategories.Description;
                }

                DeskKatRisk.Text = risks.RiskCategories.Description;

                MitigationStrategies strategies = RiskBDEntities.GetContext().MitigationStrategies.FirstOrDefault(s => s.StrategyID == _strategy.StrategyID);
                if (strategies != null)
                {
                    StrategName.Text = strategies.StrategyName;
                    DeskStrateg.Text = strategies.Description;
                    DateImplementation.SelectedDate = strategies.ImplementationDate;
                }

                parentPage.DtgMitStrat.ItemsSource = RiskBDEntities.GetContext().MitigationStrategies.ToList();
            }

        }

        private void CmbCatRisk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCatRisk.SelectedItem != null)
            {
                var selectedCategory = (RiskCategories)CmbCatRisk.SelectedItem;
                DeskKatRisk.Text = selectedCategory.Description;
            }
        }

        private void BntSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = RiskBDEntities.GetContext();

                if (string.IsNullOrWhiteSpace(StrategName.Text) || string.IsNullOrWhiteSpace(DeskStrateg.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CmbRiskName.SelectedValue == null || CmbCatRisk.SelectedValue == null)
                {
                    MessageBox.Show("Выберите риск и категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int riskID = (int)CmbRiskName.SelectedValue;
                Risks risk = context.Risks.FirstOrDefault(r => r.RiskID == riskID);

                if (risk == null)
                {
                    MessageBox.Show("Выбранный риск не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                risk.CategoryID = (int)CmbCatRisk.SelectedValue;

                MitigationStrategies strategy = _strategy ?? new MitigationStrategies();

                strategy.StrategyName = StrategName.Text;
                strategy.Description = DeskStrateg.Text;
                strategy.ImplementationDate = DateImplementation.SelectedDate ?? DateTime.Now;
                strategy.RiskID = risk.RiskID; 

                if (_strategy == null)
                {
                    context.MitigationStrategies.Add(strategy);
                }

                context.SaveChanges();

                _parentPage.DtgMitStrat.ItemsSource = null;
                RiskBDEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                _parentPage.DtgMitStrat.ItemsSource = RiskBDEntities.GetContext().MitigationStrategies
                    .Include(ms => ms.Risks)
                    .Include(ms => ms.Risks.RiskCategories)
                    .ToList();

                ClassFrame.frmObj.Navigate(new ListMitigationStrategies());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
