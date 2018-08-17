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
    /// Логика взаимодействия для AddCounter.xaml
    /// </summary>
    public partial class AddCounter : Window
    {
        AddCounterVM _model = new AddCounterVM();
        public AddCounter()
        {
            InitializeComponent();
            DataContext = _model;
        }
    }
}
