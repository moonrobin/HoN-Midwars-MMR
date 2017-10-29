using System;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace Unhide_Midwars_MMR
{
    public partial class MainForm : Form
    {
        private HonZip honZip;

        public MainForm()
        {
            InitializeComponent();
            if (!IsAdministrator())
            {
                MessageBox.Show("Please run this application as an Administrator.", "Midwars MMR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            var installDir = ProgramFilesx86() + @"\Heroes of Newerth\";
            if (!Directory.Exists(installDir))
            {
                SetPathManually();
            }
            honZip = new HonZip(installDir);
        }

        private static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void SetPathManually()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    honZip = new HonZip(fbd.SelectedPath);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            honZip.Install();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void setPaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPathManually();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            honZip.Uninstall();
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void githubLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/moonrobin/HoN-Midwars-MMR");
        }
    }
}
