using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ThreatDatabase
{

    public delegate void ButtonAction();

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Threat> listOfThreats = new ObservableCollection<Threat>();
        private int pageNumber;
        private int PageNumber
        {
            get
            {
                return pageNumber;
            }
            set
            {
                pageNumber = value;
                PageLabel.Text = (pageNumber + 1).ToString();
            }
        }
        private int pageCapacity = 25;
        private bool dataBaseNotFound;

        Database db = new Database();

        ButtonAction messageButtonAction;

        public MainWindow()
        {
            InitializeComponent();
            ListOfThreats.ItemsSource = listOfThreats;

            if (db.Database.Exists())
            {
                SetPage(0);
                return;
            }
            else
            {
                dataBaseNotFound = true;
                ShowMessage("Привет", "В данный момент база данных угроз не загружена. Хотите загрузить с официального сайта?", UpdateDatabase, "Загрузить");
            }

        }


        public void SetPage(int i)
        {
            if (i < 0 || i > db.Threats.Count() / pageCapacity)
                return;

            PageNumber = i;

            listOfThreats.Clear();
            int startIndex = pageCapacity * PageNumber;
            var currentPage = db.Threats.OrderBy(x => x.Number).Skip(startIndex).Take(pageCapacity);
            foreach (var item in currentPage)
            {
                listOfThreats.Add(item);
            }
        }


        private void ShowMessage(string title, string text, ButtonAction action = null, string buttonText = "Ок")
        {
            MessageLayerTitle.Content = title;
            MessageLayerText.Text = text;
            MessageLayerButton.Content = buttonText;

            if (action == null)
            {
                MessageLayerButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                messageButtonAction = action;
                MessageLayerButton.Visibility = Visibility.Visible;
            }

            MessageLayer.Visibility = Visibility.Visible;
        }
        private void HideMessage()
        {
            MessageLayer.Visibility = Visibility.Collapsed;
        }

        public async void UpdateDatabase()
        {
            ShowMessage("Идёт загрузка...", "");

            Queue<ThreatChange> changes;
            try
            {
                changes = await Task.Run(db.Update);
            }
            catch (Exception e)
            {
                ShowMessage("Произошла ошибка", e.Message, UpdateDatabase, "Повторить");
                return;
            }

            if (changes.Count < 1)
            {
                ShowMessage("Готово", "Обновлений не найдено", HideMessage);
                return;
            }

            HideMessage();

            if (!dataBaseNotFound)
            {
                new ThreatWindow(changes).Show();
            }

        }

        private void MessageLayerButton_Click(object sender, RoutedEventArgs e)
        {
            messageButtonAction?.Invoke();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDatabase();
        }

        private void LeftPageButton_Click(object sender, RoutedEventArgs e)
        {
            SetPage(PageNumber - 1);
        }

        private void RightPageButton_Click(object sender, RoutedEventArgs e)
        {
            SetPage(PageNumber + 1);
        }

        private void ListOfThreats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(ListOfThreats.SelectedItem is Threat item)) return;

            new ThreatWindow(item).Show();
        }
    }
}
