﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Gui.Interfaces;
using TwitchLeecher.Services.Interfaces;
using TwitchLeecher.Shared.Commands;
using TwitchLeecher.Shared.Events;

namespace TwitchLeecher.Gui.ViewModels
{
    public class DownloadsViewVM : ViewModelBase, INavigationState
    {
        #region Fields

        private ITwitchService _twitchService;
        private IDialogService _dialogService;
        private INavigationService _navigationService;
        private IEventAggregator _eventAggregator;

        private ICommand _retryDownloadCommand;
        private ICommand _cancelDownloadCommand;
        private ICommand _removeDownloadCommand;
        private ICommand _viewCommand;
        private ICommand _showLogCommand;

        private object _commandLockObject;

        #endregion Fields

        #region Constructors

        public DownloadsViewVM(
            ITwitchService twitchService,
            IDialogService dialogService,
            INavigationService navigationService,
            IEventAggregator eventAggregator)
        {
            _twitchService = twitchService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            _twitchService.PropertyChanged += TwitchService_PropertyChanged;

            _commandLockObject = new object();
        }

        #endregion Constructors

        #region Properties

        public double ScrollPosition { get; set; }

        public ObservableCollection<TwitchVideoDownload> Downloads
        {
            get
            {
                return _twitchService.Downloads;
            }
        }

        public ICommand RetryDownloadCommand
        {
            get
            {
                if (_retryDownloadCommand == null)
                {
                    _retryDownloadCommand = new DelegateCommand<string>(RetryDownload);
                }

                return _retryDownloadCommand;
            }
        }

        public ICommand CancelDownloadCommand
        {
            get
            {
                if (_cancelDownloadCommand == null)
                {
                    _cancelDownloadCommand = new DelegateCommand<string>(CancelDownload);
                }

                return _cancelDownloadCommand;
            }
        }

        public ICommand RemoveDownloadCommand
        {
            get
            {
                if (_removeDownloadCommand == null)
                {
                    _removeDownloadCommand = new DelegateCommand<string>(RemoveDownload);
                }

                return _removeDownloadCommand;
            }
        }

        public ICommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new DelegateCommand<string>(ViewVideo);
                }

                return _viewCommand;
            }
        }

        public ICommand ShowLogCommand
        {
            get
            {
                if (_showLogCommand == null)
                {
                    _showLogCommand = new DelegateCommand<string>(ShowLog);
                }

                return _showLogCommand;
            }
        }

        #endregion Properties

        #region Methods

        private void RetryDownload(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        _twitchService.Retry(id);
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void CancelDownload(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        _twitchService.Cancel(id);
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void RemoveDownload(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        _twitchService.Remove(id);
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ViewVideo(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        TwitchVideoDownload download = Downloads.Where(d => d.Id == id).FirstOrDefault();

                        if (download != null)
                        {
                            string folder = download.DownloadParams.Folder;

                            if (Directory.Exists(folder))
                            {
                                Process.Start(folder);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        private void ShowLog(string id)
        {
            try
            {
                lock (_commandLockObject)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        TwitchVideoDownload download = Downloads.Where(d => d.Id == id).FirstOrDefault();

                        if (download != null)
                        {
                            _navigationService.ShowLog(download);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowAndLogException(ex);
            }
        }

        #endregion Methods

        #region EventHandlers

        private void TwitchService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FirePropertyChanged(e.PropertyName);
        }

        #endregion EventHandlers
    }
}