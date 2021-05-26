using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            System.ComponentModel.BackgroundWorker bgWorker = new System.ComponentModel.BackgroundWorker();
            bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(bgWorker_DoWork); //Здесь происходят вычислиения
            bgWorker.WorkerReportsProgress = true; // Разрешаем сообщать о прогрессе
            bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bgWorker_ProgressChanged); //Этот метод вызывается когда из потока вызывается ReportProgress
            bgWorker.RunWorkerAsync(); //Запускаем поток на выполнение
            while (bgWorker.IsBusy)
            { }
            Console.ReadKey(true);
        }
        static void bgWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Console.WriteLine(((double)e.UserState).ToString("F4"));
        }
        static void bgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            double w = 0;
            double x = 1.5;
            for (int i = 0; i < 100; i++)
            {
                if (i % 2 == 0)
                    w += Math.Pow(2, (double)i) * i + Math.Cos(x);
                else
                    w += Math.Pow(2, (double)i) * i + Math.Sin(x);
                (sender as System.ComponentModel.BackgroundWorker).ReportProgress(0, w);
                //Здесь 1й параметр - процент выполнения нам не нужен, во втором передается результат вычисление
            }
        }
    }
}
