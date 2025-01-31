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
    /// Логика взаимодействия для AddEditRisk.xaml
    /// </summary>

    public partial class AddEditRisk : Page
    {
        private RiskAssessments _riskAssessment;
        private MainRisk mainRisk;

        public AddEditRisk(MainRisk mainRisk, RiskAssessments riskAssessment)
        {
            InitializeComponent();
            DataContext = this;
            this.mainRisk = mainRisk;
            this._riskAssessment = riskAssessment;

            CmbCatRisk.ItemsSource = RiskBDEntities.GetContext().RiskCategories.ToList();
            CmbCatRisk.SelectedValuePath = "CategoryID";
            CmbCatRisk.DisplayMemberPath = "CategoryName";

            CmbLevelRisk.ItemsSource = RiskBDEntities.GetContext().RiskLevels.ToList();
            CmbLevelRisk.SelectedValuePath = "LevelID";
            CmbLevelRisk.DisplayMemberPath = "LevelName";

            if (_riskAssessment != null)
            {
                Risks risk = RiskBDEntities.GetContext().Risks.FirstOrDefault(r => r.RiskID == _riskAssessment.RiskID);
                if (risk != null)
                {
                    RiskName.Text = risk.RiskName;
                    RiskDesk.Text = risk.Description;
                    ProbalityRisk.Text = risk.Probability?.ToString();
                    ImpactRisk.Text = risk.Impact?.ToString();
                    CmbCatRisk.SelectedValue = risk.CategoryID;
                }

                CmbLevelRisk.SelectedValue = _riskAssessment.LevelID;
                DateAssessment.SelectedDate = _riskAssessment.AssessmentDate;
                mainRisk.DtgRisks.ItemsSource = RiskBDEntities.GetContext().RiskAssessments.ToList();
            }
        }
        private void BntSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = RiskBDEntities.GetContext();

                if (string.IsNullOrWhiteSpace(RiskName.Text) || string.IsNullOrWhiteSpace(RiskDesk.Text) ||
                        string.IsNullOrWhiteSpace(ProbalityRisk.Text) || string.IsNullOrWhiteSpace(ImpactRisk.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(ProbalityRisk.Text, out double probability) || probability < 0 || probability > 1 ||
                    !double.TryParse(ImpactRisk.Text, out double impact) || impact < 0 || impact > 1)
                {
                    MessageBox.Show("Значения вероятности и влияния должны быть числами от 0 до 1!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CmbLevelRisk.SelectedValue == null)
                {
                    MessageBox.Show("Выберите уровень риска!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int levelID = (int)CmbLevelRisk.SelectedValue;

                int? riskId = _riskAssessment?.RiskID;
                Risks risk = riskId.HasValue ? context.Risks.FirstOrDefault(r => r.RiskID == riskId.Value) : null;

                if (risk == null)
                {
                    risk = new Risks
                    {
                        RiskName = RiskName.Text,
                        Description = RiskDesk.Text,
                        Probability = probability,
                        Impact = impact,
                        CategoryID = CmbCatRisk.SelectedValue != null ? (int)CmbCatRisk.SelectedValue : 0
                    };

                    context.Risks.Add(risk);
                    context.SaveChanges();
                }
                else
                {
                    risk.RiskName = RiskName.Text;
                    risk.Description = RiskDesk.Text;
                    risk.Probability = probability;
                    risk.Impact = impact;
                    risk.CategoryID = CmbCatRisk.SelectedValue != null ? (int)CmbCatRisk.SelectedValue : 0;
                }

                if (_riskAssessment == null)
                {
                    _riskAssessment = new RiskAssessments();
                    context.RiskAssessments.Add(_riskAssessment);
                }

                _riskAssessment.RiskID = risk.RiskID;
                _riskAssessment.AssessmentDate = DateAssessment.SelectedDate ?? DateTime.Now;
                _riskAssessment.LevelID = levelID;

                context.SaveChanges();

                mainRisk.DtgRisks.ItemsSource = null;
                RiskBDEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                mainRisk.DtgRisks.ItemsSource = RiskBDEntities.GetContext().RiskAssessments
                    .Include(r => r.Risks)
                    .Include(r => r.RiskLevels)
                    .ToList();

                ClassFrame.frmObj.Navigate(new MainRisk());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateRiskLevel()
        {
            if (!double.TryParse(ProbalityRisk.Text, out double probability) ||
                !double.TryParse(ImpactRisk.Text, out double impact))
            {
                MessageBox.Show("Некорректные данные! Введите числовые значения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (probability < 0 || probability > 1 || impact < 0 || impact > 1)
            {
                MessageBox.Show("Значения вероятности и влияния должны быть от 0 до 1!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double totalRiskLevel = probability * impact;

            var riskLevels = RiskBDEntities.GetContext().RiskLevels.ToList();
            int? riskLevelId = null;

            if (totalRiskLevel <= 0.3)
            {
                riskLevelId = riskLevels.FirstOrDefault(r => r.LevelName == "Низкий")?.LevelID;
            }
            else if (totalRiskLevel <= 0.7)
            {
                riskLevelId = riskLevels.FirstOrDefault(r => r.LevelName == "Средний")?.LevelID;
            }
            else
            {
                riskLevelId = riskLevels.FirstOrDefault(r => r.LevelName == "Высокий")?.LevelID;
            }

            CmbLevelRisk.SelectedValue = riskLevelId;
        }
        private void ProbalityRisk_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateRiskLevel();
        }

        private void ImpactRisk_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateRiskLevel();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
