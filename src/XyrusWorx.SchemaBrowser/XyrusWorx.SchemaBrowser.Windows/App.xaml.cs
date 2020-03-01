using System.Windows;

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None,
	ResourceDictionaryLocation.SourceAssembly
)]

namespace XyrusWorx.SchemaBrowser.Windows
{
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			MainWindow = new MainWindow();
			MainWindow.Show();

			ShutdownMode = ShutdownMode.OnMainWindowClose;
		}
	}
}
