using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Generate114514.Pages
{
    /// <summary>
    /// ResultPage.xaml 的交互逻辑
    /// </summary>
    public partial class ResultPage : Page
    {
        Task<string> task;
        double[] progressReport;
        #region constructors
        public ResultPage()
        {
            InitializeComponent();
        }
        public ResultPage(string result)
        {
            InitializeComponent();
            ShowResult.Text = result;
        }
        public ResultPage(Task<string> task)
        {
            InitializeComponent();
            this.task = task;
            TimerInit();
        }
        public ResultPage(Task<string> task, double[] progressReport)
        {
            InitializeComponent();
            this.task = task;
            this.progressReport = progressReport;
            TimerInit();
        }
        void TimerInit()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        #endregion

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (progressReport != null)
                progressBar.Value = progressReport[0] * 100;

            if (task.IsCompleted)
            {
                ShowResult.Text = task.Result;
                progressBar.Value = 100;
                ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            }
            else if (task.IsCanceled)
            {
                ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            }
        }
    }
}
