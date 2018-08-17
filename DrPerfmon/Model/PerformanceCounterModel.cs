using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrPerfmon.Model
{
    public class PerformanceCounterModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string categoryName { get; set; }
        private string counterName { get; set; }
        private string instanceName { get; set; }
        private string machineName { get; set; }
        private string categoryNameRus { get; set; }
        private string counterNameRus { get; set; }
        public DateTime TimeAddCounter { get; set; }
        public DateTime TimeEditCounter { get; set; }

        public String CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                OnPropertyChanged("CategoryName");
            }
        }
        public String CounterName
        {
            get { return counterName; }
            set
            {
                counterName = value;
                OnPropertyChanged("CounterName");
            }
        }
        public String InstanceName
        {
            get { return instanceName; }
            set
            {
                instanceName = value;
                OnPropertyChanged("InstanceName");
            }
        }
        public String MachineName
        {
            get { return machineName; }
            set
            {
                machineName = value;
                OnPropertyChanged("MachineName");
            }
        }
        public String CategoryNameRus
        {
            get { return categoryNameRus; }
            set
            {
                categoryNameRus = value;
                OnPropertyChanged("CategoryNameRus");
            }
        }
        public String CounterNameRus
        {
            get { return counterNameRus; }
            set
            {
                counterNameRus = value;
                OnPropertyChanged("CounterNameRus");
            }
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
    }
}
