using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;

namespace IndieUpdater
{
    public partial class frmMain : Form
    {
        public int cVersion = 0;
        public int sVersion = 0;
        public string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public frmMain()
        {
            InitializeComponent();

            var MyIni = new IniFile(exePath + "\\data files\\config.ini");

            if (MyIni.KeyExists("Version", "UPDATER"))
            {
                cVersion = Int32.Parse(MyIni.Read("Version", "UPDATER"));
                lblVersion.Text = "Current Revision: " + cVersion;
            }

            WebClient webClient = new WebClient();

            txtText.AppendText("\nRetrieving revision...");

            try
            {
                sVersion = Int32.Parse(webClient.DownloadString("http://website.com/updates/version.txt"));
            }
            catch
            {
                txtText.AppendText("\nCould not connect to update server.");
                lblUpdate.Enabled = false;
                return;
            }

            if(sVersion < 0)
            {
                txtText.AppendText("\nCould not update (Invalid server revision).");
                lblUpdate.Enabled = false;
                return;
            }
            
            if (sVersion > cVersion)
            {
                lblPlay.Enabled = false;
                txtText.AppendText("\nClient outdated. Click 'Update' to download the latest revision (Revision: " + sVersion + ").");
            }
            else
            {
                txtText.AppendText("\nYou have the latest revision (Revision: " + sVersion + ").");
                lblPlay.Enabled = true;
                lblUpdate.Enabled = false;
            }
        }

        private void lblUpdate_Click(object sender, EventArgs e)
        {
            WebClient objWebClient = new WebClient();
            lblProgress.Visible = true;
            lblProgress.Text = "Progress: 0%";
            try
            {
                txtText.AppendText("\nDownloading update...");
                objWebClient.DownloadFileAsync(new Uri("http://website.com/updates/Patch" + sVersion + ".zip"), "Update.zip");
            }
            catch
            {
                txtText.AppendText("\nFailed to download update.");
                return;
            }
            objWebClient.DownloadProgressChanged += (s, a) =>
            {
                lblProgress.Text = "Progress: " + a.ProgressPercentage + "%";
            };
            objWebClient.DownloadFileCompleted += (s, a) =>
            {
                txtText.AppendText("\nDownload completed.");
                //lblProgress.Visible = false;

                // Unzip the update file
                using (ZipFile zip = ZipFile.Read("Update.zip"))
                {
                    txtText.AppendText("\nExtracting, please wait...");
                    foreach (ZipEntry i in zip)
                    {
                        i.Extract(exePath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                File.Delete("Update.zip");

            };
            objWebClient.Dispose();

            var MyIni = new IniFile(exePath + "\\data files\\config.ini");

            if (MyIni.KeyExists("Version", "UPDATER"))
            {
                MyIni.Write("Version", "" + sVersion, "UPDATER");
                lblVersion.Text = "Current Revision: " + sVersion;
            }

            txtText.AppendText("\nUpdate complete!");

            lblUpdate.Enabled = false;
            lblPlay.Enabled = true;
        }

        private void lblPlay_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "Game.exe";
            Process.Start(startInfo);
            Application.Exit();
        }
    }
}
