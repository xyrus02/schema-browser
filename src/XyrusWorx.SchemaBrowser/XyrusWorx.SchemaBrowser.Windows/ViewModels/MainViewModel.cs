using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Runtime;
using XyrusWorx.SchemaBrowser.Windows.Services;
using XyrusWorx.SchemaBrowser.Windows.Views;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class MainViewModel : ViewModel
	{
		private readonly IServiceLocator mServices;
		private bool mIsLoading = true;

		public MainViewModel([NotNull] IServiceLocator services)
		{
			mServices = services ?? throw new ArgumentNullException(nameof(services));
		}
		
		public bool IsLoading
		{
			get => mIsLoading;
			private set
			{
				if (value == mIsLoading) return;
				mIsLoading = value;
				OnPropertyChanged();
			}
		}
		
		public SchemaCollectionViewModel Schemas { get; } = new SchemaCollectionViewModel();

		public async void Load([NotNull] ICommandLine commandLine)
		{
			if (commandLine == null) throw new ArgumentNullException(nameof(commandLine));

			var app = mServices.Resolve<IApplicationHost>();
			
			using var methodScope = new Scope(
				onLeave: () => IsLoading = false,
				onEnter: () => { }).Enter();

			string schemaPath;
			
			if (commandLine.SchemaPaths == null || commandLine.SchemaPaths.Length == 0)
			{
				var askResult = await mServices
					.Resolve<IOpenFileDialog>()
					.Owner(mServices.Resolve<MainWindow>())
					.Title("Open schema...")
					.Format("*.xsd", "XML Schema")
					.Async.Ask();

				if (askResult.HasError)
				{
					app.Shutdown();
					return;
				}

				schemaPath = askResult.Data;
			}
			else
			{
				schemaPath = commandLine.SchemaPaths[0];
			}

			try
			{
				var loader = mServices.Resolve<SchemaLoader>();
				var result = await loader.LoadAsync(schemaPath);
				
				result.ThrowIfError();

				await foreach (var model in loader.GetRootsAsync())
				{
					var vm = new ComplexTypeViewModel(mServices, new HashSet<XName>(), model, item => true, true);
					app.Execute(() => Schemas.Items.Add(vm));
				}
				
				Schemas.Selection.SelectFirst();
			}
			catch (Exception e)
			{
				await mServices.Resolve<IMessageBox>()
					.Owner(mServices.Resolve<MainWindow>())
					.Title("Error opening schema")
					.Error(e.Message)
					.Async.Display();
				
				app.Shutdown();
			}
		}
	}
}
