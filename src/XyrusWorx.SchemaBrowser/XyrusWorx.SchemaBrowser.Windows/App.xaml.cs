using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XyrusWorx.Diagnostics;
using XyrusWorx.Runtime;
using XyrusWorx.SchemaBrowser.Business;
using XyrusWorx.SchemaBrowser.Windows.Services;
using XyrusWorx.SchemaBrowser.Windows.ViewModels;
using XyrusWorx.SchemaBrowser.Windows.Views;
using XyrusWorx.Windows.Runtime;
using Application = XyrusWorx.Runtime.Application;

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None,
	ResourceDictionaryLocation.SourceAssembly
)]

namespace XyrusWorx.SchemaBrowser.Windows
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public partial class App : ICommandLine, IApplicationHost
	{
		private readonly IServiceLocator mServiceLocator = new ServiceLocator();

		public App()
		{
			mServiceLocator.Register(mServiceLocator);
			mServiceLocator.Register<IApplicationHost>(this);

			mServiceLocator.Register(SystemLog = mServiceLocator.CreateInstance<NullLogWriter>());
			
			mServiceLocator.RegisterSingleton<LocalizationService>();
			mServiceLocator.RegisterSingleton<SchemaLoader>();

			mServiceLocator.Register<IOpenFileDialog, WindowsOpenFileDialog>();
			mServiceLocator.Register<IOpenFolderDialog, WindowsOpenFolderDialog>();
			mServiceLocator.Register<IMessageBox, WindowsMessageBox>();
		}

		public ILogWriter SystemLog { get; }

		public new MainWindow MainWindow
		{
			get => base.MainWindow as MainWindow;
			private set => base.MainWindow = value;
		}
		public MainViewModel MainViewModel { get; private set; }

		[CommandLineValues]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		public string[] SchemaPaths { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			mServiceLocator.Register(MainViewModel = mServiceLocator.CreateInstance<MainViewModel>());
			mServiceLocator.Register(MainWindow = mServiceLocator.CreateInstance<MainWindow>());

			MainWindow.DataContext = MainViewModel;
			ShutdownMode = ShutdownMode.OnMainWindowClose;
			
			var clp = new CommandLineProcessor(typeof(App), SystemLog);
			var clkvs = new CommandLineKeyValueStore(e.Args);
			
			clp.Read(clkvs, this);
			
			SystemLog.LinkedDispatchers.Add(new DelegateLogWriter(LogDbgOut));

			MainWindow.Show();
			MainViewModel.Load(this);
		}

		private static void LogDbgOut(LogMessage obj)
		{
			var type = obj.Class switch
			{
				LogMessageClass.Debug => "debug",
				LogMessageClass.Warning => "warn",
				LogMessageClass.Error => "error",
				_ => "info"
			};

			var date = obj.Timestamp.ToString("s");
			var scope = obj.Scope?.ToString();
			
			Debug.Print($"[{type}] {date} {scope} - {obj.Text}");
		}

		public void Execute(Action action, TaskPriority priority = TaskPriority.Normal)
		{
			if (Dispatcher == null)
			{
				action();
				return;
			}
			
			Dispatcher?.Invoke(action, (DispatcherPriority) (int) priority);
		}
		public T Execute<T>(Func<T> func, TaskPriority priority = TaskPriority.Normal)
		{
			return Dispatcher == null ? func() : Dispatcher.Invoke(func, (DispatcherPriority) (int) priority);
		}
		public async Task ExecuteAsync(Action action, TaskPriority priority = TaskPriority.Normal)
		{
			if (Dispatcher == null)
			{
				await Task.Run(action);
				return;
			}
			
			await Dispatcher.InvokeAsync(action, (DispatcherPriority) (int) priority);
		}
		public async Task<T> ExecuteAsync<T>(Func<T> func, TaskPriority priority = TaskPriority.Normal)
		{
			return Dispatcher == null ? await Task.Run(func) : await Dispatcher.InvokeAsync(func, (DispatcherPriority) (int) priority);
		}

		Application IApplicationHost.Application => throw new NotSupportedException();
	}
}
