﻿using Treehopper.WPF.Message;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Treehopper.WPF.ViewModel
{
    public class TreehopperSelectorViewModel : ViewModelBase
    {
        /// <summary>
        /// Bind to this property to get an updated list of boards
        /// </summary>
        public ObservableCollection<TreehopperUSB> Boards { get { return manager.Boards; } }
        /// <summary>
        /// Bind the IsEnabled property of your control to this property to prevent changing the selected board once it's connected.
        /// </summary>
        public bool CanChangeBoardSelection { get; set; }
        /// <summary>
        /// Bind this property to the Selected property of your control to it.
        /// </summary>
        public TreehopperUSB SelectedBoard
        {
            get
            {
                return selectedBoard;
            }
            set
            {
                selectedBoard = value;
                if(selectedBoard != null)
                    ConnectCommand.RaiseCanExecuteChanged();
                else
                {
                    Disconnect();
                }
            }
        }


        private TreehopperUSB selectedBoard;

        public RelayCommand WindowClosing { get; set; }
        
        /// <summary>
        /// Bind this command to your "Connect" button.
        /// </summary>
        public RelayCommand ConnectCommand { get; set; }
        /// <summary>
        /// Bind this text string to your "Connect" button's label to have it automatically change between "Connect" and "Disconnect" when appropriate.
        /// </summary>
        public string ConnectButtonText { get; set; }

        /// <summary>
        /// Call this to close and clean up
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        bool isConnected = false;

        private TreehopperManager manager;

        public TreehopperSelectorViewModel()
        {
            manager = new TreehopperManager();
            ConnectButtonText = "Connect";
            CanChangeBoardSelection = true;
            ConnectCommand = new RelayCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute, CloseCommandCanExecute);
            WindowClosing = new RelayCommand(WindowClosingExecute);
            // This allows us to automatically close the device when the window closes
            Application.Current.MainWindow.Closing += MainWindow_Closing;

            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        private void WindowClosingExecute()
        {
            
        }

        // If the collection changed, we may have lost our board
        void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseCommandExecute();
            if (SelectedBoard != null)
                SelectedBoard.Dispose();
            else
                TreehopperUSB.UsbExit();
        }

        private bool CloseCommandCanExecute()
        {
            return isConnected;
        }

        private void CloseCommandExecute()
        {
            Debug.WriteLine("Closing...");
            if(isConnected)
            {
                isConnected = false;
                Disconnect();
            }
        }

        private bool ConnectCommandCanExecute()
        {
            return (SelectedBoard != null);
        }

        private void ConnectCommandExecute()
        {
            if (isConnected)
            {
                Disconnect();
            } else
            {
                Connect();
            }
        }

        private void Connect()
        {
            isConnected = true;
            ConnectButtonText = "Disconnect";
            RaisePropertyChanged("ConnectButtonText");
            CanChangeBoardSelection = false;
            RaisePropertyChanged("CanChangeBoardSelection");
            SelectedBoard.Open();
            Messenger.Default.Send(new BoardConnectedMessage() { Board = SelectedBoard });
        }

        private void Disconnect()
        {
            isConnected = false;
            ConnectButtonText = "Connect";
            RaisePropertyChanged("ConnectButtonText");

            CanChangeBoardSelection = true;
            RaisePropertyChanged("CanChangeBoardSelection");
            if(SelectedBoard != null)
            {
                SelectedBoard.Close();
            }
                
            Messenger.Default.Send(new BoardDisconnectedMessage() { Board = SelectedBoard });
        }
    }
}
