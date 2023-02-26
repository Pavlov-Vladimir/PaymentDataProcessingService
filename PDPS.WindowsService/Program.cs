using System.ServiceProcess;

namespace PaymentDataProcessingService.WindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            new PDPService().ConsoleRun();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PDPService()
            };
            ServiceBase.Run(ServicesToRun); 
#endif
        }
    }
}
