using System.Threading.Tasks;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
	public class MainViewModel : ViewModel
	{
		private bool mIsLoading = true;

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

		public async void Load()
		{
			IsLoading = true;

			// todo
			await Task.Delay(2000);

			IsLoading = false;
		}
	}
}
