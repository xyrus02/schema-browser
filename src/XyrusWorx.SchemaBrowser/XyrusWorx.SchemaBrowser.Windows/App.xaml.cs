using System.Windows;
using XyrusWorx.SchemaBrowser.Windows.ViewModels;

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None,
	ResourceDictionaryLocation.SourceAssembly
)]

namespace XyrusWorx.SchemaBrowser.Windows
{
	public partial class App
	{
		public MainViewModel MainViewModel { get; } = new MainViewModel();

		protected override void OnStartup(StartupEventArgs e)
		{
			ShutdownMode = ShutdownMode.OnMainWindowClose;

			MainWindow = new Views.MainWindow();
			MainWindow.DataContext = MainViewModel;
			MainWindow.Show();

			MainViewModel.Load();
		}
	}
}
