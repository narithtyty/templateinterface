using System;
using System.IO;

namespace TemplateTimerService.Lib
{
    class Lib
    {
        public static Boolean writeLOG(string msg)
        {
            string[] logfile = null;
            Boolean result = true;
            msg = "[" + DateTime.Now.ToString("yyyy-MM-dd HH mm ss") + "]: " + msg;
            try
            {
                logfile = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "LOG*.txt");
                if (logfile.Length == 0)
                {
                    using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LOG.txt", true))
                    {
                        sw.WriteLine(msg);
                        sw.Close();
                    }
                }
                else
                {
                    int lastfile = 0;
                    string lastfilepath = "";
                    for (int i = 0; i <= logfile.Length - 1; i++)
                    {
                        int numlog = 0;
                        try
                        {
                            numlog = int.Parse(logfile[i].Split('\\')[logfile[i].Split('\\').Length - 1].Replace("SLIMSLOG", "").Replace(".txt", ""));
                        }
                        catch (Exception ex)
                        {
                        }

                        if (numlog > lastfile)
                        {
                            lastfile = numlog;
                            lastfilepath = logfile[i];
                        }
                        else
                        {
                            lastfilepath = logfile[i];
                        }
                    }
                    FileInfo mylogfile = new FileInfo(lastfilepath);
                    long filesize = mylogfile.Length / 1000000;
                    if (filesize > 4)
                    {
                        lastfilepath = lastfilepath.Replace(lastfilepath.Split('\\')[lastfilepath.Split('\\').Length - 1], "SLIMSLOG" + lastfile + 1 + ".txt");
                    }
                    using (StreamWriter sw = new StreamWriter(lastfilepath, true))
                    {
                        sw.WriteLine(msg);
                        sw.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public static void backupFile(string pathfile, string backuppath)
        {
            try
            {
                string backpath = backuppath + "\\PROCESSED";
                if (Directory.Exists(backpath) == false)
                {
                    Directory.CreateDirectory(backpath);
                }
                backpath += "\\" + System.DateTime.Now.ToString("yyyy");
                if (Directory.Exists(backpath) == false)
                {
                    Directory.CreateDirectory(backpath);
                }
                backpath += "\\" + System.DateTime.Now.ToString("MM");
                if (Directory.Exists(backpath) == false)
                {
                    Directory.CreateDirectory(backpath);
                }
                backpath += "\\" + System.DateTime.Now.ToString("dd");
                if (Directory.Exists(backpath) == false)
                {
                    Directory.CreateDirectory(backpath);
                }
                backpath += "\\" + pathfile.Split(new string[] { "\\" }, StringSplitOptions.None)[pathfile.Split(new string[] { "\\" }, StringSplitOptions.None).Length - 1].Split(new string[] { "." }, StringSplitOptions.None)[0] + "-" + DateTime.Now.ToString("yyyy-MM-dd HH mm ss") + "." + pathfile.Split(new string[] { "\\" }, StringSplitOptions.None)[pathfile.Split(new string[] { "\\" }, StringSplitOptions.None).Length - 1].Split(new string[] { "." }, StringSplitOptions.None)[pathfile.Split(new string[] { "\\" }, StringSplitOptions.None)[pathfile.Split(new string[] { "\\" }, StringSplitOptions.None).Length - 1].Split(new string[] { "." }, StringSplitOptions.None).Length - 1];
                File.Move(pathfile, backpath);
            }
            catch (Exception ex)
            {
                writeLOG("Backup File Error " + ex.Message);
            }

        }
    }
}
