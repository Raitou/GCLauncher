using GCLauncher.Source;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace GCLauncher{
    public partial class Launcher : Window{
        public static Info launcherInfo = new Info();
        public Launcher(){
            InitializeComponent();
            newsInt();
            workerUpdating();
        }
        private void workerComplete(){
            fileText.Text = "Atualização Concluida!";
            btnplay.IsEnabled = true;
            btn_image_play.Source = new BitmapImage(new Uri(@"/images/play/open.png", UriKind.Relative));
            DownloadBarTotal.Maximum = 1;
            DownloadBarTotal.Value = 1;
        }
        private void workerMaintenance(){
            fileText.Text = "Servidor em Manutenção!";
            DownloadBarTotal.Maximum = 0;
            DownloadBarTotal.Value = 0;
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e){
            string myFileNameID = ((System.Net.WebClient)(sender)).QueryString["fileName"];
            string myFilePosition = ((System.Net.WebClient)(sender)).QueryString["fileCurrent"];
            string filesTotal = ((System.Net.WebClient)(sender)).QueryString["fileTotal"];
            fileText.Text = "Baixando Arquivo: " + myFileNameID + " - " + e.ProgressPercentage + "% [" + myFilePosition + "/" + filesTotal + "]";
            DownloadBarTotal.Value = e.ProgressPercentage;
        }
        private async void workerUpdating(){
            string launcherMaintenance = launcherInfo.getJson(2);
            int updateRemaining = launcherInfo.filePopulate();
            int ammountUpdated = 1;
            foreach(Arquivos fileUnit in launcherInfo.fileInfo){
                if(fileUnit.toUpdate){
                    string path = launcherInfo.fileData(fileUnit.filePath, 1);
                    string inverted = launcherInfo.fileData(fileUnit.filePath, 2);
                    if (!Directory.Exists(path) && path != ""){
                        Directory.CreateDirectory(path);
                    }
                    if(File.Exists(fileUnit.filePath)){
                        File.Delete(fileUnit.filePath);
                    }
                    string output = null;
                    using(var client = new WebClient()){
                        client.DownloadFileCompleted += (sender, e) =>{
                            output = e.ToString();
                        };
                        client.QueryString.Add("fileName", launcherInfo.fileData(fileUnit.filePath, 0).ToString());
                        client.QueryString.Add("fileCurrent", ammountUpdated.ToString());
                        client.QueryString.Add("fileTotal", updateRemaining.ToString());
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgressChanged);
                        client.DownloadFileAsync(new Uri(launcherInfo.launcherURL + "/Update/" + inverted), fileUnit.filePath);
                        var n = DateTime.Now;
                        while(output == null){
                            await Task.Delay(1); // wait for respsonse
                        }
                    }
                    ammountUpdated++;
                }
            }
            if(launcherMaintenance == "true"){
                workerMaintenance();
            }
            else{
                workerComplete();
            }
        }
        private void gameStart(){
            launcherInfo.StartMain();
            Hide();
            Thread.Sleep(10000);
            Close();
        }
        private void newsInt(){
            news1.Text = null;
            news2.Text = null;
            news3.Text = null;
            news4.Text = null;
            string newsText = null;
            launcherInfo.newsPopulate();
            foreach(News newsUnit in launcherInfo.newsInfo){
                newsText = "["+newsUnit.newsDate+"]"+newsUnit.newsTitle;
                switch(newsUnit.newsID){
                    case 1: news1.Text = newsText; break;
                    case 2: news2.Text = newsText; break;
                    case 3: news3.Text = newsText; break;
                    case 4: news4.Text = newsText; break;
                }
            }
            fileText.Text = "";
            DownloadBarTotal.Value = 0;
        }
        private void ClickLauncher(object sender, RoutedEventArgs e){
            int newsController = 0;
            string content = (sender as Button).Content.ToString();
            switch(content){
                case "btnplay": newsController = 0;  gameStart(); break;
                case     "clb": newsController = 0; this.Close(); break;
                case  "news1b": newsController = 1;               break;
                case  "news2b": newsController = 2;               break;
                case  "news3b": newsController = 3;               break;
                case  "news4b": newsController = 4;               break;
            }
            if(newsController > 0){
                foreach(News newsUnit in launcherInfo.newsInfo){
                    if(newsUnit.newsID == newsController){
                        Process.Start(newsUnit.newsURL);
                    }
                }
            }
        }
        private void mouseEnter(object sender, MouseEventArgs e){
            string content = (sender as Button).Content.ToString();
            switch(content){
                default: break;
                case "btnplay": if (btnplay.IsEnabled) { btn_image_play.Source = new BitmapImage(new Uri(@"/images/play/hover.png", UriKind.Relative)); } break;
            }
        }
        private void mouseLeave(object sender, MouseEventArgs e){
            string content = (sender as Button).Content.ToString();
            switch(content){
                default: break;
                case "btnplay": if (btnplay.IsEnabled) { btn_image_play.Source = new BitmapImage(new Uri(@"/images/play/open.png", UriKind.Relative)); } break;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e){
            DragMove();
        }
    }
}