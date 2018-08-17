using DrPerfmon.Model;
using DrPerfmon.View.DirectoryPerformanceCountersDirV;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrPerfmon.ViewModel
{
    public class DirectoryPerformanceCountersVM : INotifyPropertyChanged, IDisposable
    {
        public DrPerfmonContext db { get; set; }

        private ObservableCollection<PerformanceCounterModel> performanceCounterModelList { get; set; }
        /// <summary>
        /// Справочник счетчиков производительности
        /// </summary>
        public ObservableCollection<PerformanceCounterModel> PerformanceCounterModelList
        {
            get
            {
                if (performanceCounterModelList == null)
                {
                    performanceCounterModelList = FillPerformanceCounterModelList();
                }
                return performanceCounterModelList;
            }
            set { }
        }

        public PerformanceCounterModel SelectedCounter { get; set; }

        public ActionCommand AddCounterPerfmon { get; set; }
        public ActionCommand EditCounterPerfmon { get; set; }
        public ActionCommand DeleteCounterPerfmon { get; set; }

        public DirectoryPerformanceCountersVM()
        {
            db = new DrPerfmonContext();
            AddCounterPerfmon = new ActionCommand(AddCounterCommand) { IsExecutable = true };
            EditCounterPerfmon = new ActionCommand(EditCounterCommand) { IsExecutable = true };
            DeleteCounterPerfmon = new ActionCommand(DeleteCounterCommand) { IsExecutable = true };
        }

        private void AddCounterCommand()
        {
            AddCounter addCounter = new AddCounter();
            addCounter.ShowDialog();
            RefreshCountersList();

        }

        private void EditCounterCommand()
        {
            if (SelectedCounter != null)
                try
                {
                    EditCounter editCounter = new EditCounter(SelectedCounter);
                    editCounter.ShowDialog();
                    RefreshCountersList();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, " ", MessageBoxButton.OK, MessageBoxImage.Error); }
            else { MessageBox.Show("Выберите запись для изменения!", " ", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void DeleteCounterCommand()
        {
            if (SelectedCounter != null)
                try
                {
                    foreach (var counter in db.PerformanceCounterModels.Where(x => x.Id == SelectedCounter.Id))
                    {
                        db.PerformanceCounterModels.Remove(counter);
                    }
                    db.SaveChanges();
                    RefreshCountersList();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, " ", MessageBoxButton.OK, MessageBoxImage.Error); }
            else { MessageBox.Show("Выберите запись для удаления!", " ", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void RefreshCountersList()
        {
            PerformanceCounterModelList.Clear();
            db = new DrPerfmonContext();
            foreach (var counter in db.PerformanceCounterModels)
            {
                PerformanceCounterModelList.Add(counter);
            }
        }

        /// <summary>
        /// Заполнить справочник счетчиков производительности
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<PerformanceCounterModel> FillPerformanceCounterModelList()
        {
            try
            {
                ObservableCollection<PerformanceCounterModel> PerformanceCounterModelList = new ObservableCollection<PerformanceCounterModel>();
                foreach (var perf in db.PerformanceCounterModels)
                {
                    PerformanceCounterModelList.Add(perf);
                }
                return PerformanceCounterModelList;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, " ", MessageBoxButton.OK, MessageBoxImage.Error); }
            return null;
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
