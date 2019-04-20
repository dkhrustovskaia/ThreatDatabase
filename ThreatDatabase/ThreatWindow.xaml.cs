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
using System.Windows.Shapes;

namespace ThreatDatabase
{
    /// <summary>
    /// Логика взаимодействия для ThreatWindow.xaml
    /// </summary>
    public partial class ThreatWindow : Window
    {
        private bool needClose;
        private Queue<ThreatChange> changes;

        public ThreatWindow(Threat threat)
        {
            InitializeComponent();
            SetThreatToLayout(threat);
            needClose = true;
            Title = "Просмотр угрозы";

        }

        public ThreatWindow(Queue<ThreatChange> changes)
        {
            InitializeComponent();
            this.changes = changes;
            needClose = false;
            ShowNextChange();
            Title = "Просмотр обновлений";
        }

        public void SetThreatToLayout(Threat t)
        {
            NumberLabel.Text = t.FullNumber.ToString();
            NameLabel.Text = t.Name;
            DiscriptionLabel.Text = t.Discription;
            SourceLabel.Text = t.Source;
            ObjectLabel.Text = t.Object;
            IsPrivacyViolationLabel.Text = t.IsPrivacyViolation.MyToString();
            IsIntegrityViolationLabel.Text = t.IsIntegrityViolation.MyToString();
            IsAccessibilityViolationLabel.Text = t.IsAccessibilityViolation.MyToString();
        }

        private void ShowChangeThreat(ThreatChange change)
        {
            switch (change.Type)
            {
                case ThreatChange.ChangeType.Add:
                    ThreatLayerStatusLabel.Content = "[Добавлено]";
                    SetThreatToLayout(change.Current);
                    break;
                case ThreatChange.ChangeType.Remove:
                    ThreatLayerStatusLabel.Content = "[Удалено]";
                    SetThreatToLayout(change.Previous);
                    break;
                case ThreatChange.ChangeType.Edit:
                    ThreatLayerStatusLabel.Content = "[Изменено]";

                    NumberLabel.Text = change.NumberChange();
                    NameLabel.Text = change.NameChange();
                    DiscriptionLabel.Text = change.DiscriptionChange();
                    SourceLabel.Text = change.SourceChange();
                    ObjectLabel.Text = change.ObjectChange();
                    IsPrivacyViolationLabel.Text = change.IsPrivacyViolationChange();
                    IsIntegrityViolationLabel.Text = change.IsIntegrityViolationCHange();
                    IsAccessibilityViolationLabel.Text = change.IsAccessibilityViolationChange();

                    break;
            }
        }

        private void ThreatLayerOkButton_Click(object sender, RoutedEventArgs e)
        {
            if (needClose)
            {
                Close();
                return;
            }

            ShowNextChange();
                     
        }

        private void ShowNextChange()
        {
            if (changes.Count > 0)
            {
                var change = changes.Dequeue();
                ShowChangeThreat(change);

                if (changes.Count < 1)
                    needClose = true;

                return;
            }
        }
    }
}
