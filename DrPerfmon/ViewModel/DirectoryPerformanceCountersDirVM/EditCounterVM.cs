using DrPerfmon.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrPerfmon.ViewModel.DirectoryPerformanceCountersDirVM
{
    [NotMapped]
    public class EditCounterVM : PerformanceCounterModel, INotifyPropertyChanged, IDisposable
    {
        public DrPerfmonContext db { get; set; }
        PerformanceCounterModel SelectedCounter;

        public EditCounterVM(PerformanceCounterModel SelectedCounter)
        {
            this.SelectedCounter = SelectedCounter;
            db = new DrPerfmonContext();
            CategoryName = SelectedCounter.CategoryName;
            CounterName = SelectedCounter.CounterName;
            CategoryNameRus = SelectedCounter.CategoryNameRus;
            CounterNameRus = SelectedCounter.CounterNameRus;
            InstanceName = SelectedCounter.InstanceName;
            MachineName = SelectedCounter.MachineName;
        }

        private RelayCommand editCommand;
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                    (editCommand = new RelayCommand(obj =>
                    {
                        foreach (var edit in db.PerformanceCounterModels.Where(x => x.Id == SelectedCounter.Id))
                        {
                            edit.CategoryName = CategoryName;
                            edit.CounterName = CounterName;
                            edit.CategoryNameRus = CategoryNameRus;
                            edit.CounterNameRus = CounterNameRus;
                            edit.InstanceName = InstanceName;
                            edit.MachineName = MachineName;
                            edit.TimeEditCounter = DateTime.Now;
                        }
                            db.SaveChanges();
                        (obj as Window).Close();
                    }));
            }
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
