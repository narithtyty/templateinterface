using Renci.SshNet;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TemplateTimerService
{
    public partial class Service1 : ServiceBase
    {
        public Boolean chkprocess = true;
        private static System.Timers.Timer myTimer = new System.Timers.Timer();
        public static int scantime = 5000;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Lib.Lib.writeLOG("Service Start");
            try
            {
                scantime = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["scantime"].ToString()) * 1000;
            }
            catch (Exception ex)
            {
            }
            Lib.Lib.writeLOG("Scan Time " + scantime);
            myTimer.Interval = scantime;
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerEventProcessor);
            myTimer.Start();
        }

        protected override void OnStop()
        {
            Lib.Lib.writeLOG("Service Stop");
        }
        private void TimerEventProcessor(object myObject, EventArgs myEventArgs)
        {
            if (chkprocess == true)
            {
                chkprocess = false;
                processMain();
            }
            else
            {
                myTimer.Stop();
            }
        }
        private void processMain()
        {
            Lib.Lib.writeLOG("In process main");
            string path = System.Configuration.ConfigurationSettings.AppSettings["Path"].ToString();
            Lib.Lib.writeLOG("Path:" + path);
            if (!string.IsNullOrEmpty(path))
            {
                string extension = System.Configuration.ConfigurationSettings.AppSettings["Filter"].ToString();
                string subdire = System.Configuration.ConfigurationSettings.AppSettings["subfolder"].ToString();
                string[] datafile = Directory.GetFiles(path, extension, subdire == "Y" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                Lib.Lib.writeLOG("file:" + datafile.Length);
                for (int j = 0; j <= datafile.Length - 1; j++)
                {
                    Lib.Lib.writeLOG("filename:" + datafile[j].ToString());
                    //string filecsv = toCSV(datafile[j].ToString(), path);
                    // UploadSFTPFile(filecsv);
                    // File.Delete(datafile[j].ToString());
                    // File.Delete(filecsv);
                }
            }
            else
            {
                Lib.Lib.writeLOG("Path Not Set in App.config");
            }

        }

        private void UploadSFTPFile(string filename)
        {
            try
            {
                using (SftpClient client = new SftpClient(System.Configuration.ConfigurationSettings.AppSettings["server"].ToString(), int.Parse(System.Configuration.ConfigurationSettings.AppSettings["port"].ToString()), System.Configuration.ConfigurationSettings.AppSettings["user"].ToString(), System.Configuration.ConfigurationSettings.AppSettings["pass"].ToString()))
                {
                    client.Connect();
                    client.ChangeDirectory(System.Configuration.ConfigurationSettings.AppSettings["folder"].ToString());
                    using (FileStream fs = new FileStream(filename, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(fs, Path.GetFileName(filename));
                        Lib.Lib.writeLOG("Upload CSV Success " + filename);
                    }
                }
                File.Delete(filename);
            }
            catch (Exception e)
            {
                // updateStatus(e.Message);
            }
        }
    }
}
