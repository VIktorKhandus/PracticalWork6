using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WorkWithWord.HelperClasses;
using WpfAppLR6.ModelClasses;

namespace WpfAppLR6
{
    public partial class MainWindow : Window
    {
        // Поле для хранения контекста базы данных
        private Model1 model;

        // Список всех пользователей
        private List<Users> users;

        // Список всех автомобилей
        private List<Auto> autos;

        // Конструктор главного окна
        public MainWindow()
        {
            // Инициализирует компоненты окна

            InitializeComponent();
            // Создает новый экземпляр контекста базы данных
            model = new Model1();

            // Инициализирует списки пользователей и автомобилей пустыми списками
            users = new List<Users>();
            autos = new List<Auto>();
        }

        // Метод для заполнения combobox'ов данными о пользователях и автомобилях
        private void ComboLoadData()
        {
            // Очищает элементы combobox'а с пользователями
            comboBoxUsers.Items.Clear();

            // Заполняет список пользователей данными из базы данных
            users = model.Users.ToList();

            // Добавляет данные о каждом пользователе в combobox
            foreach (var item in users)
                comboBoxUsers.Items.Add($"{item.FullName} {item.PSeria} {item.PNumber}");

            // Устанавливает первый элемент как выбранный
            comboBoxUsers.SelectedIndex = 0;

            // Получает автомобили текущего выбранного пользователя
            autos = users[comboBoxUsers.SelectedIndex].Auto.ToList();

            // Очищает элементы combobox'а с автомобилями
            comboBoxAutos.Items.Clear();

            // Добавляет данные об автомобилях в combobox
            foreach (var item in autos)
                comboBoxAutos.Items.Add($"{item.Model} {item.YearOfRelease.Value.Year} {item.VIN}");

            // Устанавливает первый автомобиль как выбранный
            comboBoxAutos.SelectedIndex = 0;
        }

        // Метод вызывается при загрузке окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Загружает данные в combobox'ы
            ComboLoadData();
        }

        // Метод вызывается при смене выбранного элемента в combobox'е с пользователями
        private void comboBoxUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получает автомобили нового выбранного пользователя
            autos = users[comboBoxUsers.SelectedIndex].Auto.ToList();

            // Очищает элементы combobox'а с автомобилями
            comboBoxAutos.Items.Clear();

            // Добавляет данные об автомобилях в combobox
            foreach (var item in autos)
                comboBoxAutos.Items.Add($"{item.Model} {item.YearOfRelease.Value.Year} {item.VIN}");

            // Устанавливает первый автомобиль как выбранный
            comboBoxAutos.SelectedIndex = 0;
        }

        // Метод обработки нажатия кнопки "Сохранить документ"
        private void SaveDocument_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалоговое окно для выбора директории
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            // Задаем описание для окна выбора директории
            fbd.Description = "Выберите место сохранения";

            // Проверяем результат открытия диалогового окна
            if (System.Windows.Forms.DialogResult.OK == fbd.ShowDialog())
            {
                // Получаем активного пользователя
                Users activeUser = users[comboBoxUsers.SelectedIndex];

                // Получаем активный автомобиль
                Auto activeAuto = activeUser.Auto.ToList()[comboBoxAutos.SelectedIndex];

                // Создаем документ и сохраняем его в указанную директорию
                CreateDocument(
                    $@"{fbd.SelectedPath}\Купля-Продажа-Автомобиля-{activeUser.FullName}.docx",
                    activeUser,
                    activeAuto);

                // Выводим сообщение о сохранении файла
                System.Windows.MessageBox.Show("Файл сохранен");
            }
        }

        // Метод создания документа с подстановкой данных пользователя и автомобиля
        private void CreateDocument(string directorypath, Users users, Auto auto)
        {
            // Получаем текущую дату
            var today = DateTime.Now.ToShortDateString();

            // Создаем объект для работы с документом Word
            WordHelper word = new WordHelper("ContractSale.docx");

            // Создаем словарь для замены ключевых слов в документе
            var items = new Dictionary<string, string>()
            {
                // Замена ключевого слова <Today> на текущую дату
                { "<Today>", today },

                // Данные пользователя
                { "<fullName>", users.FullName },
                { "<DateOfBirth>", users.DateOfBirth.Value.ToShortDateString() }, // дата рождения
                { "<Adress>", users.Adress },
                { "<PSeria>", users.PSeria.ToString() }, // Серия паспорта
                { "<PNumber>", users.PNumber.ToString() }, // Номер паспорта
                { "<PVidam>", users.PVidan }, // Кем выдан паспорт

                // Данные автомобиля
                { "<Model>", auto.Model }, // Модель автомобиля
                { "<Category>", auto.Category }, // Категория автомобиля
                { "<Type>", auto.TypeV }, // Тип автомобиля
                { "<VIN>", auto.VIN }, // VIN номер
                { "<RegistrationMark>", auto.RegistrationMark }, // Регистрационный знак
                { "<YearV>", auto.YearOfRelease.Value.Year.ToString() }, // Год выпуска
                { "<EngineV>", auto.EngineNumber }, // Номер двигателя
                { "<ChassisV>", auto.Chassis }, // Шасси
                { "<BodyworkV>", auto.Bodywork }, // Кузов
                { "<ColorV>", auto.Color }, // Цвет
                { "<SeriaPV>", auto.SeriaPasport }, // Серия ПТС
                { "<NumberPV>", auto.NumbePasport }, // Номер ПТС
                { "<VidanPV>", auto.VidanPasport } // Кем выдан ПТС
            };

            // Обрабатывает документ, подставляя значения из словаря вместо ключевых слов
            word.Process(items, directorypath);
        }
    }
}
