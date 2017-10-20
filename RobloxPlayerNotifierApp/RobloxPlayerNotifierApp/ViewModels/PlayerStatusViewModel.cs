using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RobloxPlayerNotifierApp.Models;
using System.Windows.Input;

namespace RobloxPlayerNotifierApp.ViewModels
{
    public class PlayerStatusViewModel : INotifyPropertyChanged
    {
        private PlayerStatus _status;
        private string _name;
        private string _playerProfileUrl;

        public PlayerStatusViewModel(string name)
        {
            _name   = name;
            _status = PlayerStatus.Offline;
        }

        public event PropertyChangedEventHandler PropertyChanged;





        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public PlayerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged("ProfileButtonVisibility");
            }
        }

        public string PlayerProfileUrl
        {
            get => _playerProfileUrl;
            set
            {
                _playerProfileUrl = value;
                OnPropertyChanged();
                OnPropertyChanged("ProfileButtonVisibility");
            }
        }

        public Visibility ProfileButtonVisibility
        {
            get
            {
                return String.IsNullOrEmpty(_playerProfileUrl) == false && _status == PlayerStatus.Playing
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
