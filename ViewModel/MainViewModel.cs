using ACCess.Helpers;
using ACCess.Model;
using ACCess.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ACCess.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        #region Services & Helpers

        private readonly SettingsHelper<FavouriteSettings> _favSettings;
        private readonly IGameService _game;
        private readonly SettingsHelper<UserSettings> _userSettings;

        #endregion Services & Helpers

        #region Properties

        private string? _addFavouriteAddress;
        private string? _addFavouriteDescription;
        private string? _customServerInput;
        private string? _directory;
        private bool _loadingStatus;
        private string? _loadingStatusText;
        private SavedServer? _selectedSavedServer;
        private string? _selectedServer;

        /// <summary>
        /// The address of the saved server being added.
        /// </summary>
        public string? AddFavouriteAddress
        {
            get => _addFavouriteAddress;
            set => SetProperty(ref _addFavouriteAddress, value);
        }

        /// <summary>
        /// The description of the saved server being added.
        /// </summary>
        public string? AddFavouriteDescription
        {
            get => _addFavouriteDescription;
            set => SetProperty(ref _addFavouriteDescription, value);
        }

        /// <summary>
        /// A UI input that allows the user to set a server that's not in the saved list.
        /// </summary>
        public string? CustomServerInput
        {
            get => _customServerInput;
            set => SetProperty(ref _customServerInput, value);
        }

        /// <summary>
        /// The path to view & save the game config files.
        /// </summary>
        public string? Directory
        {
            get => _directory;
            set => SetProperty(ref _directory, value);
        }

        /// <summary>
        /// True if the UI needs to display a loading state, otherwise false.
        /// </summary>
        public bool LoadingStatus
        {
            get => _loadingStatus;
            set => SetProperty(ref _loadingStatus, value);
        }

        /// <summary>
        /// If the UI is in a loading state, a message which can be displayed to the user.
        /// </summary>
        public string? LoadingStatusText
        {
            get => _loadingStatusText;
            set => SetProperty(ref _loadingStatusText, value);
        }

        /// <summary>
        /// Contains a list of locally saved server details, which can be selected by the user.
        /// </summary>
        public ObservableCollection<SavedServer> SavedServers { get; set; }

        /// <summary>
        /// The selected saved server (if the user has selected one in the UI).
        /// </summary>
        public SavedServer? SelectedSavedServer
        {
            get => _selectedSavedServer;
            set => SetProperty(ref _selectedSavedServer, value);
        }

        /// <summary>
        /// The string that needs to be injected into the game files.
        /// </summary>
        public string? SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }

        #endregion Properties

        #region Commands

        public IRelayCommand AddFavourite { get; }
        public IRelayCommand DeleteFavourite { get; }
        public IAsyncRelayCommand Refresh { get; }
        public IRelayCommand Reset { get; }
        public IAsyncRelayCommand Save { get; }
        public IRelayCommand SelectFavourite { get; }

        #endregion Commands

        public MainViewModel(IGameService game)
        {
            // Set DI services
            _game = game;

            // Load saved services & settings
            _favSettings = new SettingsHelper<FavouriteSettings>("favourites.json");
            _userSettings = new SettingsHelper<UserSettings>("settings.json");

            // Register commands
            Refresh = new AsyncRelayCommand(RefreshHandlerAsync);
            Reset = new RelayCommand(ResetHandler);
            Save = new AsyncRelayCommand(SaveHandlerAsync);
        }

        public async Task RefreshHandlerAsync()
        {
            LoadingStatus = true;
            LoadingStatusText = "Loading settings...";
            var settings = await _userSettings.LoadAsync();
            if (settings != null && System.IO.Directory.Exists(settings.Directory))
                Directory = settings.Directory;
            else
                Directory = FileHelper.GetDefaultDirectory();

            LoadingStatusText = "Reading serverList.json...";
            var serverList = await _game.ReadServerListAsync(Directory);
            if (serverList != null && !string.IsNullOrWhiteSpace(serverList.LeagueServerIp))
                SelectedServer = serverList?.LeagueServerIp;
            else
                SelectedServer = null;

            LoadingStatus = false;
            LoadingStatusText = null;
        }

        public void ResetHandler()
        {
            Directory = FileHelper.GetDefaultDirectory();
        }

        public async Task SaveHandlerAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedServer))
            {
                // Deal with this
                return;
            }

            LoadingStatus = true;
            LoadingStatusText = "Updating serverList.json...";

            // Validate the input (either selected favourite or custom input)
            if (!System.IO.Directory.Exists(Directory))
            {
                // Directory doesn't exist - show a validation error
                return;
            }

            // Save to the game files
            await _game.SetServerListAsync(new ServerList
            {
                LeagueServerIp = SelectedServer
            }, Directory);

            // Save the changed user settings
            await _userSettings.SaveAsync(new UserSettings
            {
                Directory = Directory
            });

            LoadingStatus = false;
            LoadingStatusText = null;
        }
    }
}