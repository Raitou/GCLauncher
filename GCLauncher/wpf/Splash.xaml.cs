using GCLauncher.Source;
using System.Threading;
using System.Windows;

namespace GCLauncher{
    public partial class Splash : Window{
        public Splash(){
            InitializeComponent();
            Show();
            Thread.Sleep(1000);
            Info status = new Info();
            string jsonData = status.getJson(0);
            if(jsonData == null){
                MessageBox.Show("Sem conexão!", "Sem conexão!");
            }
            else{
                var Launcher = new Launcher();
                Launcher.Show();
            }
            Close();
        }
    }
}
