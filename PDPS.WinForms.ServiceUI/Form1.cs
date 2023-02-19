using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDPS.WinForms.ServiceUI
{
    public partial class Form1 : Form
    {
        private ServiceController _controller;

        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_Browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.SafeFileName;
            }
        }

        private void Btn_Install_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 1)
            {
                MessageBox.Show("Choose a windows service file.");
            }
            else
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    ManagedInstallerClass.InstallHelper(new[] { openFileDialog1.FileName });
                    Cursor = Cursors.Default;

                    MessageBox.Show("Service \"" + openFileDialog1.FileName + "\" was installed.");
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private void Btn_Uninstall_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 1)
            {
                MessageBox.Show("Choose a windows service file.");
            }
            else
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    ManagedInstallerClass.InstallHelper(new[] { @"/u", openFileDialog1.FileName });
                    Cursor = Cursors.Default;

                    MessageBox.Show("Service \"" + openFileDialog1.FileName + "\" was uninstalled.");
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _controller = new ServiceController() { ServiceName = "PDPService" };
                _controller.Start();
                Cursor = Cursors.Default;

                MessageBox.Show("Service started.");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _controller = new ServiceController() { ServiceName = "PDPService" };
                _controller.Stop();
                Cursor = Cursors.Default;

                MessageBox.Show("Service stopped.");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void Btn_Restart_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _controller = new ServiceController() { ServiceName = "PDPService" };
                if (_controller.CanStop)
                {
                    _controller.Stop();
                }
                _controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20));
                if (_controller.Status == ServiceControllerStatus.Stopped)
                {
                    _controller.Start();
                }

                Cursor = Cursors.Default;

                MessageBox.Show("Service restarted.");
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
