using DrPerfmon.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DrPerfmon.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged, IDisposable
    {
        public int SampleEvery = 2 * 1000; //Снимать показания через каждые (секунд)
        public int Duration = 1200 * 1000; //Продолжительность (секунд)

        public delegate void DisplayHandler();

        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get { return seriesCollection; }
            set
            {
                seriesCollection = value;
                OnPropertyChanged("SeriesCollection");
            }
        }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public DrPerfmonContext db { get; set; }

        DispatcherTimer Timer = new DispatcherTimer();
        public Dispatcher _dispatcher;

        public ActionCommand TimerStart { get; set; }
        public ActionCommand TimerStop { get; set; }

        private ObservableCollection<ParameterPerformance> parameterPerformanceList { get; set; }
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

        public ObservableCollection<PerformanceCounter> performanceCounters = new ObservableCollection<PerformanceCounter>(); //Счетчики производительности
        public ObservableCollection<ChartValuesModel> chartValuesModels = new ObservableCollection<ChartValuesModel>(); //Значения графиков производительности

        public MainWindowVM()
        {
            db = new DrPerfmonContext();
            _dispatcher = Dispatcher.CurrentDispatcher;
            TimerStart = new ActionCommand(TimerStartCommand) { IsExecutable = true };
            TimerStop = new ActionCommand(TimerStopCommand) { IsExecutable = true };

            Timer.Tick += Timer_Tick; // don't freeze the ui
            Timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            Timer.IsEnabled = false;

            //SeriesCollection = new SeriesCollection();
            foreach (var param in db.PerformanceCounterModels)
            {
                performanceCounters.Add(new PerformanceCounter(param.CategoryName, param.CounterName, param.InstanceName, param.MachineName));
                //SeriesCollection.Add(new LineSeries() { Title = param.CounterName });
            }

            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Средняя длина очереди диска",
            //        Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
            //    }
            //};

            //Прописываем лейблы 1 лейбл под 1 значение
            Labels = new string[Duration];
            int CounterLabel = 0;
            for (int i = 0; i < Duration; i += SampleEvery/1000)
            {
                Labels[CounterLabel] = i.ToString();
                CounterLabel++;
            }
        }

        int CounterCurrent = 0;
        public void Timer_Tick(System.Object sender, System.EventArgs e)
        {
            DisplayHandler handler = new DisplayHandler(Display);
            IAsyncResult resultObj = handler.BeginInvoke(null, null);

            Thread.Sleep(SampleEvery);

            CounterCurrent += SampleEvery;

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
                    SeriesCollection = new SeriesCollection();
                    foreach (var param in performanceCounters)
                    {
                        double paramValue = param.NextValue();
                        chartValuesModels.Add(new ChartValuesModel() { CategoryName = param.CategoryName, ChartValue = paramValue });

                        ParameterPerformanceList.Add(new ParameterPerformance()
                        {
                            ValueCounter = paramValue,
                            CategoryName = param.CategoryName,
                            CounterName = param.CounterName,
                            MachineName = param.MachineName,
                            TimeAdd = DateTime.Now
                        });

                        seriesCollection.Add(new LineSeries() { Title = param.CategoryName, Values = new ChartValues<double>(GetChartValues(param.CategoryName)) });
                    }

                }));
        }

        private ChartValues<double> GetChartValues(string CategoryName)
        {
            ChartValues<double> chartValues = new ChartValues<double>();
            foreach (var val in chartValuesModels.Where(val => val.CategoryName == CategoryName))
            {
                chartValues.Add(val.ChartValue);
            }
            return chartValues;
        }

        private void TimerStartCommand()
        {
            Timer.Start();
        }

        private void TimerStopCommand()
        {
            Timer.Stop();
        }

        /// <summary>
        /// Заполнить список сварочных автоматов, установить true false для отметки строк
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
