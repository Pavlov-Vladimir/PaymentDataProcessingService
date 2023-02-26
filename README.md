# PaymentDataProcessingService

The application is developed as a windows service if it is built in Realeas mode. The service is called PDPS_WindowsService.exe

The PDPS.WinForms.ServiceUI project is used to manage the service (install/aninstall, start/stop/restart). The output application is ServiceManagerLite.exe. Attention! Be sure to run as an administrator. ServiceManagerLite.exe can work with other similar services that are built with ProjectInstaller.

If you build in Debug mode then the application runs as a simple console application for debugging purposes. (You must first set the output type in the application properties to type - console application)
