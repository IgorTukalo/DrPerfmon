using DrPerfmon.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrPerfmon.ViewModel.DirectoryPerformanceCountersDirVM
{
    [NotMapped]
    public class AddCounterVM : PerformanceCounterModel, INotifyPropertyChanged, IDisposable
    {
        public DrPerfmonContext db { get; set; }

        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                    (removeCommand = new RelayCommand(obj =>
                    {
                        db.PerformanceCounterModels.Add(new PerformanceCounterModel()
                        {
                            CategoryName = CategoryName,
                            CounterName = CounterName,
                            CategoryNameRus = CategoryNameRus,
                            CounterNameRus = CounterNameRus,
                            InstanceName = InstanceName,
                            MachineName = MachineName,
                            TimeAddCounter = DateTime.Now
                        });
                        db.SaveChanges();
                        (obj as Window).Close();
                    }));
            }
        }

        public AddCounterVM()
        {
            db = new DrPerfmonContext();
        }

        #region IDisposable

        public void Dispose()
        {
            //мы должны освободить ресурсы контекста при удалении ViewModel
            db.Dispose();
        }

        #endregion
    }
}
