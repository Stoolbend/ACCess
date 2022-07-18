using ACCess.Helpers;
using ACCess.Model;
using ACCess.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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
        private string? _addFavouriteErrorText;
        private string? _customServerInput;
        private string? _directory;
        private string? _errorText;
        private bool _loadingStatus;
        private string? _loadingStatusText;
        private SavedServer? _selectedSavedServer;
        private string? _selectedServer;
        private ServerList? _serverList;
        private string? _successText;
        private bool _unsavedChanges;

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
        /// Text to display if validation errors occur for adding favourites.
        /// </summary>
        public string? AddFavouriteErrorText
        {
            get => _addFavouriteErrorText;
            set => SetProperty(ref _addFavouriteErrorText, value);
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
        /// Text to be shown on the UI if there is a validation error.
        /// </summary>
        public string? ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
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
        public ObservableCollection<SavedServer> SavedServers { get; } = new ObservableCollection<SavedServer>();

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

        /// <summary>
        /// The string that needs to be injected into the game files.
        /// </summary>
        public string? ServerListAddress
        {
            get
            {
                if (_serverList != null)
                    return _serverList.LeagueServerIp;
                return null;
            }
        }

        /// <summary>
        /// Text to be shown on the UI for success messages.
        /// </summary>
        public string? SuccessText
        {
            get => _successText;
            set => SetProperty(ref _successText, value);
        }

        /// <summary>
        /// True if there are unsaved changes, otherwise false.
        /// </summary>
        public bool UnsavedChanges
        {
            get => _unsavedChanges;
            set => SetProperty(ref _unsavedChanges, value);
        }

        #endregion Properties

        #region Commands

        public IAsyncRelayCommand AddFavourite { get; }
        public IAsyncRelayCommand Clear { get; }
        public IAsyncRelayCommand DeleteFavourite { get; }
        public IRelayCommand DeselectFavourite { get; }
        public IAsyncRelayCommand Refresh { get; }
        public IRelayCommand ResetDirectory { get; }
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
            AddFavourite = new AsyncRelayCommand(AddFavouriteHandlerAsync);
            Clear = new AsyncRelayCommand(ClearHandlerAsync);
            DeleteFavourite = new AsyncRelayCommand(DeleteFavouriteHandlerAsync);
            Refresh = new AsyncRelayCommand(RefreshHandlerAsync);
            ResetDirectory = new RelayCommand(ResetDirectoryHandler);
            Save = new AsyncRelayCommand(SaveHandlerAsync);
            SelectFavourite = new RelayCommand(SelectFavouriteHandler);
        }

        public async Task AddFavouriteHandlerAsync()
        {
            AddFavouriteErrorText = null;

            // Validate (at least) address is set
            if (string.IsNullOrWhiteSpace(AddFavouriteAddress))
            {
                AddFavouriteErrorText = "You can't save an empty address to your favourites.\r\nPlease enter an address before tying again.";
                return;
            }
            if (!ValidateIPAddress(AddFavouriteAddress))
            {
                AddFavouriteErrorText = "That IP address is invalid.\r\nPlease make sure the IP address is correct before trying again.";
                return;
            }
            if (SavedServers.Where(v => v.Address.Equals(AddFavouriteAddress)).Any())
            {
                AddFavouriteErrorText = "That address is already saved in your favourites.";
                LoadingStatus = false;
                LoadingStatusText = null;
                return;
            }

            LoadingStatus = true;
            LoadingStatusText = "Adding favourite server...";

            // Add server to end of list
            var server = new SavedServer(AddFavouriteAddress)
            {
                Description = AddFavouriteDescription,
                Order = (ushort)SavedServers.Count
            };
            SavedServers.Add(server);

            // Save favourites list
            var settings = await _favSettings.LoadAsync();
            if (settings == null)
                settings = new FavouriteSettings();
            settings.FavouriteServers = new List<SavedServer>(SavedServers);
            await _favSettings.SaveAsync(settings);

            LoadingStatus = false;
            LoadingStatusText = null;
        }

        public async Task ClearHandlerAsync()
        {
            ErrorText = null;
            SuccessText = null;
            LoadingStatus = true;
            LoadingStatusText = "Deleting serverList.json...";

            // Clear properties
            SelectedServer = null;
            SelectedSavedServer = null;

            // Then clear the file
            try
            {
                _game.DeleteServerList();
                SuccessText = "serverList.json cleared successfully!";
            }
            catch (Exception ex)
            {
                ErrorText = $"Exception while deleting serverList.json\r\n{ex.Message}";
                LoadingStatus = false;
                LoadingStatusText = null;
            }
            finally
            {
                await RefreshHandlerAsync();
            }
        }

        public async Task DeleteFavouriteHandlerAsync()
        {
            AddFavouriteErrorText = null;

            // Validate (at least) address is set
            if (SelectedSavedServer == null)
            {
                AddFavouriteErrorText = "You need to select a favourite server to delete first.\r\nPlease select a server before tying again.";
                return;
            }

            LoadingStatus = true;
            LoadingStatusText = "Deleting favourite server...";

            // Remove server from list & deselct it
            SavedServers.Remove(SelectedSavedServer);
            SelectedSavedServer = null;

            // Save favourites list
            var settings = await _favSettings.LoadAsync();
            if (settings == null)
                settings = new FavouriteSettings();
            settings.FavouriteServers = new List<SavedServer>(SavedServers);
            await _favSettings.SaveAsync(settings);

            LoadingStatus = false;
            LoadingStatusText = null;

            await RefreshHandlerAsync();
        }

        public async Task RefreshHandlerAsync()
        {
            ErrorText = null;
            LoadingStatus = true;
            LoadingStatusText = "Loading settings...";
            var settings = await _userSettings.LoadAsync();
            if (settings != null && System.IO.Directory.Exists(settings.Directory))
                Directory = settings.Directory;
            else
                Directory = FileHelper.GetDefaultDirectory();

            LoadingStatusText = "Loading favourites...";
            await PopulateSavedServersAsync();

            LoadingStatusText = "Reading serverList.json...";
            _serverList = await _game.ReadServerListAsync(Directory);
            if (_serverList != null && !string.IsNullOrWhiteSpace(_serverList.LeagueServerIp))
                SelectedServer = _serverList?.LeagueServerIp;
            else
                SelectedServer = null;

            LoadingStatus = false;
            LoadingStatusText = null;
        }

        public void ResetDirectoryHandler()
        {
            ErrorText = null;
            SuccessText = null;

            Directory = FileHelper.GetDefaultDirectory();
        }

        public async Task SaveHandlerAsync()
        {
            ErrorText = null;
            SuccessText = null;
            LoadingStatus = true;
            LoadingStatusText = "Updating serverList.json...";

            // Check if we're clearing the file, or updating it
            if (string.IsNullOrWhiteSpace(SelectedServer))
            {
                await ClearHandlerAsync();
                return;
            }

            // Validate the input
            if (!ValidateIPAddress(SelectedServer))
            {
                ErrorText = "That IP address is invalid.\r\nPlease make sure the IP address is correct before trying again.";
                return;
            }
            if (!System.IO.Directory.Exists(Directory))
            {
                ErrorText = "The config directory does not exist.\r\nPlease make sure the location is correct before trying again.";
                return;
            }

            try
            {
                // Save to the game files
                var newServerList = new ServerList
                {
                    LeagueServerIp = SelectedServer
                };
                await _game.SetServerListAsync(newServerList, Directory);
                _serverList = newServerList;

                SuccessText = "serverList.json updated successfully!";
            }
            catch (Exception ex)
            {
                ErrorText = $"Exception while saving serverList.json\r\n{ex.Message}";
                LoadingStatus = false;
                LoadingStatusText = null;
            }
            finally
            {
                // Save the changed user settings
                await _userSettings.SaveAsync(new UserSettings
                {
                    Directory = Directory
                });

                LoadingStatus = false;
                LoadingStatusText = null;
            }
        }

        public void SelectFavouriteHandler()
        {
            ErrorText = null;
            SuccessText = null;

            if (SelectedSavedServer == null)
                SelectedServer = null;
            else
                SelectedServer = SelectedSavedServer.Address;
        }

        private async Task PopulateSavedServersAsync()
        {
            var favourites = await _favSettings.LoadAsync();
            if (favourites == null)
                favourites = new FavouriteSettings();
            SavedServers.Clear();
            foreach (var item in favourites.FavouriteServers.OrderBy(v => v.Order))
                SavedServers.Add(item);
        }

        private bool ValidateIPAddress(string address)
        {
            return Regex.Match(address, @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(\.(?!$)|$)){4}$").Success;
        }
    }
}