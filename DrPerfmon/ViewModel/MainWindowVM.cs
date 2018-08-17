using DrPerfmon.Model;
using DrPerfmon.View;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DrPerfmon.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Снимать показания через каждые (секунд)
        /// </summary>
        public int SampleEvery = 2 * 1000;

        /// <summary>
        /// Продолжительность (секунд)
        /// </summary>
        public int Duration = 1200 * 1000;

        /// <summary>
        /// Асинхронный делегат обновления графиков производительности
        /// </summary>
        public delegate void DisplayHandler();

        private SeriesCollection seriesCollection;
        /// <summary>
        /// Коллекция графиков
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get { return seriesCollection; }
            set
            {
                seriesCollection = value;
                OnPropertyChanged("SeriesCollection");
            }
        }

        private bool scrollDown;
        /// <summary>
        /// Прокрутка лога вниз
        /// </summary>
        public Boolean ScrollDown
        {
            get { return scrollDown; }
            set
            {
                scrollDown = value;
                OnPropertyChanged("ScrollDown");
            }
        }

        /// <summary>
        /// Подписи к графикам
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// Значения графиков
        /// </summary>
        private Func<double, string> yFormatter;
        public Func<double, string> YFormatter
        {
            get { return yFormatter; }
            set
            {
                yFormatter = value;
                OnPropertyChanged("YFormatter");
            }
        }

        public DrPerfmonContext db { get; set; }

        DispatcherTimer Timer = new DispatcherTimer();
        public Dispatcher _dispatcher;

        /// <summary>
        /// Запустить таймер
        /// </summary>
        public ActionCommand TimerStart { get; set; }

        /// <summary>
        /// Остановить таймер
        /// </summary>
        public ActionCommand TimerStop { get; set; }

        /// <summary>
        /// Открыть справочник счетчиков производительности
        /// </summary>
        public ActionCommand DirectoryPerformanceCountersOpen { get; set; }

        private ObservableCollection<ParameterPerformance> parameterPerformanceList { get; set; }
        /// <summary>
        /// Список значений счетчиков производительности (Лог)
        /// </summary>
        public ObservableCollection<ParameterPerformance> ParameterPerformanceList
        {
            get
            {
                if (parameterPerformanceList == null)
                {
                    parameterPerformanceList = FillParameterPerformanceList();
                }
                return parameterPerformanceList;
            }
            set { }
        }

        /// <summary>
        /// Счетчики производительности
        /// </summary>
        public ObservableCollection<PerformanceCounter> performanceCounters = new ObservableCollection<PerformanceCounter>();

        /// <summary>
        /// Значения графиков производительности
        /// </summary>
        //public ObservableCollection<ChartValuesModel> chartValuesModels = new ObservableCollection<ChartValuesModel>();

        public MainWindowVM()
        {
            db = new DrPerfmonContext();
            ScrollDown = false;
            _dispatcher = Dispatcher.CurrentDispatcher;

            TimerStart = new ActionCommand(TimerStartCommand) { IsExecutable = true };
            TimerStop = new ActionCommand(TimerStopCommand) { IsExecutable = true };
            DirectoryPerformanceCountersOpen = new ActionCommand(DirectoryPerformanceCountersOpenCommand) { IsExecutable = true };

            Timer.Tick += Timer_Tick; // don't freeze the ui
            Timer.Interval = new TimeSpan(0, 0, 0, 2, 0);
            Timer.IsEnabled = false;
        }

        int CounterCurrent = 0;
        int CounterLabel = 0;
        public void Timer_Tick(System.Object sender, System.EventArgs e)
        {
            DisplayHandler handler = new DisplayHandler(Display);
            IAsyncResult resultObj = handler.BeginInvoke(null, null);

            CounterCurrent += 2;
            CounterLabel++;
            if (Duration <= CounterCurrent)
            {
                Timer.Stop();
                MessageBox.Show("Таймер остановлен!");
            }
        }

        private void Display()
        {
            _dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var param in performanceCounters)
                {
                    double paramValue = param.NextValue();
                    string categoryNameRus = (from b in db.PerformanceCounterModels
                                              where b.CategoryName == param.CategoryName
                                              select b.CategoryNameRus).Distinct().Single();
                    string counterNameRus = (from b in db.PerformanceCounterModels
                                             where b.CounterName == param.CounterName
                                             select b.CounterNameRus).Distinct().Single();

                    ParameterPerformanceList.Add(new ParameterPerformance()
                    {
                        ValueCounter = paramValue,
                        CategoryNameRus = categoryNameRus,
                        CounterNameRus = counterNameRus,
                        MachineName = param.MachineName,
                        TimeAdd = DateTime.Now
                    });

                    foreach (var a in SeriesCollection.Where(a => a.Title == counterNameRus))
                    {
                        a.Values.Add(paramValue);
                        Labels[CounterLabel] = DateTime.Now.ToString("HH:mm:ss");
                    }
                }
            }));
        }

        public void dtgrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ScrollDown == true)
                if ((sender as DataGrid).SelectedItem != null)
                {
                    (sender as DataGrid).SelectedIndex = (sender as DataGrid).Items.Count;
                    (sender as DataGrid).UpdateLayout();
                    (sender as DataGrid).ScrollIntoView((sender as DataGrid).SelectedItem);
                }
        }

        private void TimerStartCommand()
        {
            SeriesCollection = new SeriesCollection();
            foreach (var param in db.PerformanceCounterModels)
            {
                try
                {
                    performanceCounters.Add(new PerformanceCounter(param.CategoryName, param.CounterName, param.InstanceName, param.MachineName));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message +string.Format("\n{0}, {1}, {2}, {3}\nПроверьте корректность данных в справочнике счетчиков производительности!", param.CategoryName, param.CounterName, param.InstanceName, param.MachineName), " ", MessageBoxButton.OK, MessageBoxImage.Error); }
                SeriesCollection.Add(new LineSeries() { Title = param.CounterNameRus, Values = new ChartValues<double>() });
            }

            //Прописываем лейблы 1 лейбл под 1 значение
            Labels = new string[Duration];

            Timer.Start();
        }

        private void TimerStopCommand()
        {
            Timer.Stop();
        }

        private void DirectoryPerformanceCountersOpenCommand()
        {
            DirectoryPerformanceCounters directoryPerformanceCounters = new DirectoryPerformanceCounters();
            directoryPerformanceCounters.ShowDialog();
        }

        /// <summary>
        /// Заполнить список счетчиков производительности (Лог)
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<ParameterPerformance> FillParameterPerformanceList()
        {
            ObservableCollection<ParameterPerformance> ParameterPerformanceList = new ObservableCollection<ParameterPerformance>();

            return ParameterPerformanceList;
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// Событие, которое мы должны вызывать каждый раз когда хотим сообщить об изменении данных.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для вызова события об изменении свойства ViewModel.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        #region IDisposable

        public void Dispose()
        {
            //мы должны освободить ресурсы контекста при удалении ViewModel
            db.Dispose();
        }

        #endregion
    }
}
