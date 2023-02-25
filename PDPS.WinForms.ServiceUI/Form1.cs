using System;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Windows.Forms;

namespace PDPS.WinForms.ServiceUI
{
    public partial class Form1 : Form
    {
        private ServiceController _controller;

        public Form1()
        {
            InitializeComponent();

            SetControlBattonsStatus();
            btn_Install.Enabled = false;
            btn_Uninstall.Enabled = false;
        }

        private void Btn_Browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.SafeFileName;
                Cursor = Cursors.WaitCursor;
                btn_Install.Enabled = true;
                btn_Uninstall.Enabled = true;
                SetControlBattonsStatus();
                Cursor = Cursors.Default;
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
                Cursor = Cursors.WaitCursor;
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { openFileDialog1.FileName });
                    Cursor = Cursors.Default;

                    MessageBox.Show("Service \"" + openFileDialog1.FileName + "\" was installed.");

                    bool serviceIsExists = ServiceHelper.TryGetServiceName(openFileDialog1.FileName, out string serviceName);
                    if (serviceIsExists)
                    {
                        _controller = new ServiceController() { ServiceName = serviceName };
                        SetControlBattonsStatus(_controller);
                    }
                    btn_Install.Enabled = false;
                    btn_Uninstall.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    try
                    {
                        bool serviceIsExists = ServiceHelper.TryGetServiceName(openFileDialog1.FileName, out string serviceName);
                        if (serviceIsExists)
                        {
                            _controller = new ServiceController() { ServiceName = serviceName };
                            SetControlBattonsStatus(_controller);
                        }
                        btn_Uninstall.Enabled = true;
                        btn_Install.Enabled = false;
                    }
                    catch (Exception exc) { MessageBox.Show(exc.Message); }
                }
                finally
                {
                    Cursor = Cursors.Default;
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
                Cursor = Cursors.WaitCursor;
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { @"/u", openFileDialog1.FileName });

                    MessageBox.Show("Service \"" + openFileDialog1.FileName + "\" was uninstalled.");

                    btn_Uninstall.Enabled = false;
                    btn_Install.Enabled = true;
                    SetControlBattonsStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                bool serviceIsExists = ServiceHelper.TryGetServiceName(openFileDialog1.FileName, out string serviceName);
                if (serviceIsExists)
                {
                    _controller = new ServiceController() { ServiceName = serviceName };
                    _controller.Start();
                    _controller.WaitForStatus(ServiceControllerStatus.Running);

                    SetControlBattonsStatus(_controller);

                    MessageBox.Show("Service started.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                bool serviceIsExists = ServiceHelper.TryGetServiceName(openFileDialog1.FileName, out string serviceName);
                if (serviceIsExists)
                {
                    _controller = new ServiceController() { ServiceName = serviceName };
                    _controller.Stop();
                    _controller.WaitForStatus(ServiceControllerStatus.Stopped);

                    SetControlBattonsStatus(_controller);

                    MessageBox.Show("Service stopped.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void Btn_Restart_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                bool serviceIsExists = ServiceHelper.TryGetServiceName(openFileDialog1.FileName, out string serviceName);
                if (serviceIsExists)
                {
                    _controller = new ServiceController() { ServiceName = serviceName };
                    if (_controller.CanStop)
                    {
                        _controller.Stop();
                        _controller.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    _controller.Start();
                    _controller.WaitForStatus(ServiceControllerStatus.Running);

                    SetControlBattonsStatus(_controller);

                    MessageBox.Show("Service restarted.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void SetControlBattonsStatus(ServiceController controller = null)
        {
            if (controller is null)
            {
                btn_Stop.Enabled = false;
                btn_Restart.Enabled = false;
                btn_Start.Enabled = false;
            }
            else
            {
                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    btn_Stop.Enabled = false;
                    btn_Restart.Enabled = false;
                    btn_Start.Enabled = true;
                }
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    btn_Start.Enabled = false;
                    btn_Stop.Enabled = true;
                    btn_Restart.Enabled = true;
                }
            }
        }
    }
}
