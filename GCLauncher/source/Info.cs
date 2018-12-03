using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace GCLauncher.Source{
    public class Info{
        public string launcherURL = "http://127.0.0.1/Launcher";
        private string launcherParam = "PARAMETRO_DO_MAIN";
        public List<Arquivos> fileInfo { get; set; }
        public List<News> newsInfo { get; set; }
        public string getJson(int type){
            using (var client = new WebClient()){
                if(type == 0){
                    return client.DownloadString(launcherURL+"/files.json");
                }
                else if(type == 1){
                    return client.DownloadString(launcherURL+"/news.json");
                }
                else{
                    return client.DownloadString(launcherURL+"/online.txt");
                }
            }
        }
        public void StartMain(){
            ProcessStartInfo startInfo = new ProcessStartInfo(@"Main.exe");
            startInfo.Arguments = launcherParam;
            Process.Start(startInfo);
        }
        public string fileData(string path, int opt){
            if(opt == 0){
                return Path.GetFileName(path);
            }
            else if(opt == 1){
                return Path.GetDirectoryName(path);
            }
            else{
                path.Replace('\'', '/');
                return path;
            }
        }
        public int fileCheckUpdate(Arquivos unit){
            if(!File.Exists(@unit.filePath)){
                unit.toUpdate = true;
                return 1;
            }
            else{
                using (FileStream fs = new FileStream(@unit.filePath, FileMode.Open))
                using (var cryptoProvider = new SHA1CryptoServiceProvider()){
                    string hash = BitConverter.ToString(cryptoProvider.ComputeHash(fs));
                    string hash1 = hash.ToLower();
                    string hash2 = hash1.Replace("-", string.Empty);
                    if(hash2 != unit.fileHash){
                        fs.Close();
                        unit.toUpdate = true;
                        return 1;
                    }
                    else{
                        fs.Close();
                        unit.toUpdate = false;
                        return 0;
                    }
                }
            }
        }
        public int filePopulate(){
            int count = 0;
            int unitCount = 0;
            fileInfo = JsonConvert.DeserializeObject<List<Arquivos>>(getJson(0));
            foreach(Arquivos fileUnit in fileInfo){
                unitCount = fileCheckUpdate(fileUnit);
                count += unitCount;
            }
            return count;
        }
        public void newsPopulate(){
            newsInfo = JsonConvert.DeserializeObject<List<News>>(this.getJson(1));
        }
    }
}