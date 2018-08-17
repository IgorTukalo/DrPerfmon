using DrPerfmon.ViewModel;
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

namespace DrPerfmon.View
{
    /// <summary>
    /// Логика взаимодействия для DirectoryPerformanceCounters.xaml
    /// </summary>
    public partial class DirectoryPerformanceCounters : Window
    {
        DirectoryPerformanceCountersVM _model = new DirectoryPerformanceCountersVM();
        public DirectoryPerformanceCounters()
        {
            InitializeComponent();
            DataContext = _model;
        }
    }
}
