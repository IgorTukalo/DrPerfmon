using DrPerfmon.Model;
using DrPerfmon.ViewModel.DirectoryPerformanceCountersDirVM;
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

namespace DrPerfmon.View.DirectoryPerformanceCountersDirV
{
    /// <summary>
    /// Логика взаимодействия для EditCounter.xaml
    /// </summary>
    public partial class EditCounter : Window
    {
        public EditCounter(PerformanceCounterModel SelectedCounter)
        {
            EditCounterVM _model = new EditCounterVM(SelectedCounter);
            InitializeComponent();
            DataContext = _model;
        }
    }
}
