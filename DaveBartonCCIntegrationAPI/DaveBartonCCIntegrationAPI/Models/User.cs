using System.ComponentModel;

namespace DaveBartonCCIntegrationAPI.Models
{
    public class User : INotifyPropertyChanged
    {
        public User()
        {
            IsNew = true;
            IsModified = false;
        }

        private int userId;
        private string username;
        private string passwordHash;
        private string firstName;
        private string lastName;
        private string email;
        private string password;
        private bool isNew;
        private bool isModified;

        public bool IsNew
        {
            get => isNew;
            set { isNew = value; OnPropertyChanged(nameof(IsNew)); }
        }

        public bool IsModified
        {
            get => isModified;
            set { isModified = value; OnPropertyChanged(nameof(IsModified)); }
        }

        public int UserId
        {
            get => userId;
            set { userId = value; OnPropertyChanged(nameof(UserId)); }
        }

        public string Username
        {
            get => username;
            set
            {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged(nameof(Username));
                    if (!IsNew) IsModified = true;
                }
            }
        }

        public string PasswordHash
        {
            get => passwordHash;
            set
            {
                if (passwordHash != value)
                {
                    passwordHash = value;
                    OnPropertyChanged(nameof(PasswordHash));
                    if (!IsNew) IsModified = true;
                }
            }
        }

        public string FirstName
        {
            get => firstName;
            set
            {
                if (firstName != value)
                {
                    firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                    if (!IsNew) IsModified = true;
                }
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                if (lastName != value)
                {
                    lastName = value;
                    OnPropertyChanged(nameof(LastName));
                    if (!IsNew) IsModified = true;
                }
            }
        }

        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                    if (!IsNew) IsModified = true;
                }
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                    if (!IsNew) IsModified = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
