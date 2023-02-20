using PaymentDataProcessingService.WindowsService;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace PDPS.WinForms.ServiceUI
{
    internal static class ServiceHelper
    {
        public static bool TryGetServiceName(string serviceAssembly, out string serviceName)
        {
            try
            {
                var assembly = Assembly.LoadFrom(serviceAssembly);
                var types = assembly.GetTypes();
                var installerType = types.FirstOrDefault(t => (t.Name == nameof(ProjectInstaller) || t.Name == "Installer"));
                var installerInstans = Activator.CreateInstance(installerType);
                var fields = installerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var serviceInstallerField = fields.FirstOrDefault(f => f.FieldType == typeof(ServiceInstaller));

                if (serviceInstallerField != null && installerInstans != null)
                {
                    var serviseInstallerInstans = serviceInstallerField.GetValue(installerInstans) as ServiceInstaller;
                    serviceName = serviseInstallerInstans.ServiceName; 
                }
                else
                {
                    serviceName = null;
                }

                return !string.IsNullOrEmpty(serviceName);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
