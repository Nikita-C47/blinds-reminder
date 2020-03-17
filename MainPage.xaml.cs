using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BlindsRandomizerUWP.Entities;
using BlindsRandomizerUWP.Helpers;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace BlindsRandomizerUWP
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Таймер, отсчитывающий время
        /// </summary>
        private DispatcherTimer timer;
        /// <summary>
        /// Флаг того, что таймер запускается в первый раз
        /// </summary>
        private bool firstStart = true;
        /// <summary>
        /// Флаг того, что нужно перезапустить таймер (для сброса последовательности блайндов)
        /// </summary>
        private bool reloadTimer = true;        
        /// <summary>
        /// Количество минут для таймера по-умолчанию
        /// </summary>
        private int defaultTimerMinutes = 7;
        /// <summary>
        /// Фактическое количество минут, на которое запускается таймер
        /// </summary>
        private int timerMinutes;
        /// <summary>
        /// Дата и время, с которого начинается таймер
        /// </summary>
        private DateTime timerDateTime;
        /// <summary>
        /// Дата и время, на которых заканчивается таймер
        /// </summary>
        private DateTime minDateTime;
        /// <summary>
        /// Список блайндов
        /// </summary>
        private List<string> blinds;
        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        public MainPage()
        {
            // Инициализируем окно
            this.InitializeComponent();
            // Инициализируем таймер
            InitTimer();
            // Загружаем блайнды
            LoadBlinds();
        }
        /// <summary>
        /// Загружает блайнды при запуске новой последовательности блайндов
        /// </summary>
        private void LoadBlinds()
        {
            // Загружаем блайнды
            blinds = new List<string>
            {
                "25 / 50",
                "50 / 100",
                "100 / 200",
                "200 / 400",
                "400 / 800",
                "800 / 1 600",
                "1 500 / 3 000",
                "3 000 / 6 000",
                "5 000 / 10 000",
                "10 000 / 20 000",
                "20 000 / 40 000",
                "50 000 / 100 000"
            };
        }
        /// <summary>
        /// Получает блайнды
        /// </summary>
        private void GetBlinds()
        {
            // Инициализируем рандомизатор
            var randomizer = new Random();
            // Строка с блайндами
            string blind = "";
            // Если еще есть обычные блайнды
            if (blinds.Count > 0)
            {
                // Индекс по-умолчанию равен 0 (первый элемент в списке)
                int index = 0;
                // Получаем блайнд
                blind = blinds.ElementAt(index);
                // Удаляем его из списка
                blinds.RemoveAt(index);
            }
            // Если блайнды уже закончились
            else
            {
                // Заканчиваем игру
                timerToggle.IsEnabled = false;
                timerToggle.Background = new SolidColorBrush(Colors.Gray);
                timerToggle.BorderBrush = new SolidColorBrush(Colors.Gray);
                blind = "Game over!";
            }
            // Указываем текущие блайнды
            currentBlinds.Text = blind;
        }
        /// <summary>
        /// Загружает таймер
        /// </summary>
        private void LoadTimer()
        {
            // Пробуем привести ввод пользователя к целому числу
            bool parsingSuccess = Int32.TryParse(minutesCount.Text, out timerMinutes);
            // Если не удалось - берем значение по-умолчанию
            if (!parsingSuccess)
            {
                timerMinutes = defaultTimerMinutes;
            }
            // Заводим временный экземпляр текущей даты и времени и сбрасываем время
            DateTime temp = DateTime.Now.Date;
            // Добавляем количество минут таймера для временного экземпляра и устанавливаем его как конечное время таймера
            timerDateTime = temp.AddMinutes(timerMinutes);
            // Указываем время таймера
            currentTime.Text = timerDateTime.ToString("m:ss");
            // В качестве минимальной даты будет дата и время с 00:00:00
            minDateTime = temp;
            // Указываем что таймер перезагружен
            reloadTimer = false;
        }        
        /// <summary>
        /// Инициализирует таймер
        /// </summary>
        private void InitTimer()
        {
            // Заводим экземпляр таймера
            timer = new DispatcherTimer();
            // Указываем событие для истечения таймера
            timer.Tick += timer_Tick;
            // Указываем интервал таймера
            timer.Interval = new TimeSpan(0, 0, 1);
        }
        /// <summary>
        /// Запускает таймер
        /// </summary>
        private async void StartTimer()
        {
            // Если это первый запуск таймера
            if (firstStart)
            {
                // Надо загрузить блайнды
                GetBlinds();               
            }
            // Устанавливаем текст кнопки
            timerToggle.Content = "Остановить";
            // Запускаем таймер
            timer.Start();
            // Если первый запуск - нужно еще получить данные о последней игре
            if (firstStart)
            {
                // Показываем загрузку новой игры
                lastGame.Visibility = Visibility.Visible;
                // Заводим помощник для API
                ApiHelper helper = new ApiHelper();
                // Получаем результаты последней игры
                GameResults results = await helper.GetLastGameResults();
                // Устанавливаем тип игры
                lastGameType.Text = results.type;
                // Если запрос успешен (об этом говорит наличие мест)
                if(results.places.Count > 0)
                {
                    // Перебираем места
                    foreach (GamePlace place in results.places)
                    {
                        // Получаем текст
                        string text = place.place + ". " + place.player;
                        // Добавляем место
                        AddPlace(text, HorizontalAlignment.Left);                        
                    }
                }    
                // Если запрос неудачен
                else
                {
                    // Добавляем место
                    AddPlace("-", HorizontalAlignment.Center);
                }
                // Прячем загрузку
                lastGameLoading.Visibility = Visibility.Collapsed;
                lastGameLoading.IsActive = false;
                // Показываем информацию о последней игре
                lastGameInfo.Visibility = Visibility.Visible;
                // Указываем что первый запуск произведен
                firstStart = false;
            }
        }
        /// <summary>
        /// Останавливает таймер
        /// </summary>
        private void StopTimer()
        {
            // Останавливаем таймер
            timer.Stop();
            // Меняем текст кнопки
            timerToggle.Content = "Запустить";
        }
        /// <summary>
        /// Проверяет, не пора ли обновлять блайнды
        /// </summary>
        private void CheckTimer()
        {
            // Если таймер дошел до 0
            if (timerDateTime == minDateTime)
            {
                // Останавливаем таймер
                StopTimer();
                // Получаем новые блайнды
                GetBlinds();
                // Проигрываем звук
                media.Play();
                // Указываем что надо перезагрузить таймер
                reloadTimer = true;
                // Если еще остались блайнды
                if (blinds.Count > 0)
                {
                    // Перезапускаем таймер
                    ToggleTimer();
                }
            }
        }

        /// <summary>
        /// Заканчивает игру
        /// </summary>
        private void EndGame()
        {
            // Блокируем кнопку управления таймером
            timerToggle.IsEnabled = false;
        }

        /// <summary>
        /// Обработчик события истечения таймера (срабатывает каждую секунду)
        /// </summary>
        /// <param name="sender">Объект-инициатор события</param>
        /// <param name="e">Аргументы события</param>
        private void timer_Tick(object sender, object e)
        {
            // Уменьшаем дату таймера на секунду
            timerDateTime = timerDateTime.AddSeconds(-1);
            // Указываем текущее состояние таймера
            currentTime.Text = timerDateTime.ToString("m:ss");
            //CommandManager.InvalidateRequerySuggested();
            // Проверяем, не закончился ли таймер
            CheckTimer();
        }
        /// <summary>
        /// Обработчик нажатия на кнопку запуска / остановки таймера
        /// </summary>
        /// <param name="sender">Объект-инициатор события</param>
        /// <param name="e">Аргументы события</param>
        private void timerToggle_Click(object sender, RoutedEventArgs e)
        {
            ToggleTimer();
        }

        /// <summary>
        /// Запускает / останавливает таймер
        /// </summary>
        private void ToggleTimer()
        {
            // Если таймер запущен - останавливаем его
            if (timer.IsEnabled)
            {
                StopTimer();
            }
            // Иначе
            else
            {
                // Если таймер надо перезагрузить - перезагружаем его
                if (reloadTimer)
                {
                    LoadTimer();
                }
                // Запускаем таймер               
                StartTimer();
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку сброса таймера
        /// </summary>
        /// <param name="sender">Объект-инициатор события</param>
        /// <param name="e">Аргументы события</param>
        private void timerReset_Click(object sender, RoutedEventArgs e)
        {
            // Останавливаем таймер
            StopTimer();
            // Сбрасываем текст текущего времени таймера
            currentTime.Text = "0:00";
            // Сбрасываем текущие блайнды
            currentBlinds.Text = "- / -";
            // Указываем что надо перезагрузить таймер
            reloadTimer = true;
            // Указываем что следующий старт будет первым
            firstStart = true;
            // Заново загружаем блайнды
            LoadBlinds();
            // Если игра закончилась, разблокируем кнопку запуска
            if (!timerToggle.IsEnabled)
            {
                timerToggle.IsEnabled = true;
                timerToggle.Background = new SolidColorBrush(Color.FromArgb(100, 57, 168, 88));
            }
            // Устанавливаем инфо о последней игре как при запуске приложения

            // Прячем инфо о последней игре
            lastGame.Visibility = Visibility.Collapsed;
            // Показываем дочерний текст о результатах
            lastGameLabel.Visibility = Visibility.Visible;
            // Прячем информацию о последней пигре
            lastGameInfo.Visibility = Visibility.Collapsed;
            // Показываем загрузку
            lastGameLoading.Visibility = Visibility.Visible;
            lastGameLoading.IsActive = true;
            // Очищаем список мест
            lastGameResults.Children.Clear();

        }
        /// <summary>
        /// Добавляет строку в StackPanel мест
        /// </summary>
        /// <param name="text">Текст строки</param>
        /// <param name="alignment">Выравнивание строки</param>
        private void AddPlace(string text, HorizontalAlignment alignment)
        {
            // Заводим текстовый блок
            TextBlock label = new TextBlock();
            // Указываем текст
            label.Text = text;
            // Устанавливаем размер шрифта
            label.FontSize = 21;
            // Выравнивание текста
            label.HorizontalAlignment = alignment;
            // Добавляем новый текстовый блок в StackPanel
            lastGameResults.Children.Add(label);
        }
    }
}
