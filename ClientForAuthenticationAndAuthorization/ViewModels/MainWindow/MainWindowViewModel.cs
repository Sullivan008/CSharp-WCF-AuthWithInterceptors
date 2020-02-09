using ClientForAuthenticationAndAuthorization.AuthenticationServiceReference;
using ClientForAuthenticationAndAuthorization.Command;
using ClientForAuthenticationAndAuthorization.Dialogs;
using ClientForAuthenticationAndAuthorization.ExecuteOperationsServiceReference;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Windows.Input;

namespace ClientForAuthenticationAndAuthorization.ViewModels.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _userName;
        private string _password;

        private int? _multiplier;
        private int? _multiplicand;

        private bool _enabledOperationsBtn;
        private bool _enabledOperationsTxtBox;
        private bool _enabledMulBtn;

        public MainWindowViewModel()
        {
            _userName = string.Empty;
            _password = string.Empty;

            _multiplier = null;
            _multiplicand = null;
        }

        #region PROPERTIES Getters/ Setters

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        public int? Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;

                EnabledMulBtn = _multiplicand != null;

                OnPropertyChanged("Multiplier");
            }
        }

        public int? Multiplicand
        {
            get => _multiplicand;
            set
            {
                _multiplicand = value;

                EnabledMulBtn = _multiplier != null;

                OnPropertyChanged("Multiplicand");
            }
        }

        #endregion

        #region BTN/TXTBOX Properties Getters/ Setters

        public bool EnabledMulBtn
        {
            get => _enabledMulBtn;
            set
            {
                _enabledMulBtn = value;
                OnPropertyChanged("EnabledMulBtn");
            }
        }

        public bool EnabledOperationsBtn
        {
            get => _enabledOperationsBtn;
            set
            {
                _enabledOperationsBtn = value;
                OnPropertyChanged("EnabledOperationsBtn");
            }
        }

        public bool EnabledOperationsTxtBox
        {
            get => _enabledOperationsTxtBox;
            set
            {
                _enabledOperationsTxtBox = value;
                OnPropertyChanged("EnabledOperationsTxtBox");
            }
        }

        #endregion

        #region COMMANDS

        public ICommand AuthBtnClick =>
            new DelegateCommand(Authentication);

        public ICommand MulBtnClick =>
            new DelegateCommand(Multiplication);

        public ICommand ReadOperationBtnClick =>
            new DelegateCommand(ReadOperation);

        public ICommand WriteOperationBtnClick =>
            new DelegateCommand(WriteOperation);

        #endregion

        #region PRIVATE COMMAND Helper Methods

        private void Authentication()
        {
            using (AuthenticationServiceClient authenticationServiceClient = new AuthenticationServiceClient())
            {
                using (new OperationContextScope(authenticationServiceClient.InnerChannel))
                {
                    try
                    {
                        if (UserAuthentication(authenticationServiceClient))
                        {
                            Globals.Cookie = GetSetCookieInResponseChannelHeaders();

                            new NotificationDialog($"Authentication Success!\n\n{Globals.Cookie}", "Information").ShowInfoMessageBox();

                            EnabledOperationsTxtBox = true;
                            EnabledOperationsBtn = true;
                        }
                        else
                        {
                            new NotificationDialog("Authentication Failed!", "Error").ShowErrorMessageBox();
                        }
                    }
                    catch (FaultException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (TimeoutException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                }
            }
        }

        private void Multiplication()
        {
            using (ExecuteOperationsServiceClient executeOperationsServiceClient = new ExecuteOperationsServiceClient())
            {
                using (new OperationContextScope(executeOperationsServiceClient.InnerChannel))
                {
                    try
                    {
                        if (Multiplicand == null)
                        {
                            throw new ArgumentNullException(nameof(Multiplicand));
                        }
                        if (Multiplier != null)
                        {
                            new NotificationDialog($"{Multiplier} * {Multiplicand} = {executeOperationsServiceClient.MulOperation(Multiplier.Value, Multiplicand.Value)}",
                                    "The result of multiplication").ShowInfoMessageBox();
                        }
                        else
                        {
                            throw new ArgumentNullException(nameof(Multiplier));
                        }

                    }
                    catch (FaultException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (SecurityAccessDeniedException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (TimeoutException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                }
            }
        }

        private void ReadOperation()
        {
            using (ExecuteOperationsServiceClient executeOperationsServiceClient = new ExecuteOperationsServiceClient())
            {
                using (new OperationContextScope(executeOperationsServiceClient.InnerChannel))
                {
                    try
                    {
                        new NotificationDialog(executeOperationsServiceClient.ReadOperation(), "Service Response").ShowInfoMessageBox();
                    }
                    catch (FaultException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (SecurityAccessDeniedException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (TimeoutException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                }
            }
        }

        private void WriteOperation()
        {
            using (ExecuteOperationsServiceClient executeOperationsServiceClient = new ExecuteOperationsServiceClient())
            {
                using (new OperationContextScope(executeOperationsServiceClient.InnerChannel))
                {
                    try
                    {
                        new NotificationDialog(executeOperationsServiceClient.WriteOperation(), "Service Response").ShowInfoMessageBox();
                    }
                    catch (FaultException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (SecurityAccessDeniedException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                    catch (TimeoutException ex)
                    {
                        new NotificationDialog(ex.Message, "Service Response").ShowErrorMessageBox();
                    }
                }
            }
        }

        #endregion

        #region PRIVATE Helper Methods

        private bool UserAuthentication(AuthenticationService authenticationService) =>
            authenticationService.Login(UserName, Password, string.Empty, true);

        private string GetSetCookieInResponseChannelHeaders() =>
            ((HttpResponseMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpResponseMessageProperty.Name]).Headers.Get("Set-Cookie");

        #endregion
    }
}