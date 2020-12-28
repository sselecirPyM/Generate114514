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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading;
using Generate114514.Utility;

namespace Generate114514.Pages
{
    /// <summary>
    /// SimpleArgumentPage.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleArgumentPage : Page
    {
        public SimpleArgumentPage()
        {
            InitializeComponent();
            theElements.Add(new Functions.IntContent(1));
            theElements.Add(new Functions.IntContent(1));
            theElements.Add(new Functions.IntContent(4));
            theElements.Add(new Functions.IntContent(5));
            theElements.Add(new Functions.IntContent(1));
            theElements.Add(new Functions.IntContent(4));
            viewElements.ItemsSource = theElements;
        }

        public ObservableCollection<Functions.IntContent> theElements = new ObservableCollection<Functions.IntContent>();
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            theElements.Add(new Functions.IntContent());
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (viewElements.SelectedIndex != -1)
                theElements.RemoveAt(viewElements.SelectedIndex);
            else if (theElements.Count > 0)
                theElements.RemoveAt(theElements.Count - 1);
        }
        public int targetValue { get; set; }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TaskHolder.CancelTasks();
            e.Handled = true;

            int[] intArray = new int[theElements.Count];
            for (int i = 0; i < theElements.Count; i++)
            {
                intArray[i] = theElements[i].Value;
            }
            double[] progressReport = new double[1];
            TaskHolder.cts = new CancellationTokenSource();
            TaskHolder.task = Task.Run(() => Functions.Algorithms.SimpleArgument114115(intArray, targetValue, TaskHolder.cts.Token, ref progressReport[0]));
            ResultPage page = new ResultPage(TaskHolder.task, progressReport);
            NavigationService.Navigate(page);
        }
    }
}
