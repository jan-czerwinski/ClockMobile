using Acr.UserDialogs;
using ClockMobile.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace ClockMobile.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private  IBluetoothLE Ble { get; set; }
        private  IAdapter Adapter { get; set; }
        private IDevice  BleDevice {get;set;}
        private IService Service { get; set; }
        private ICharacteristic SwitchCharacteristic { get; set; }
        private ICharacteristic BrightnessCharacteristic { get; set; }
        private ICharacteristic ColorCharacteristic { get; set; }
        private ICharacteristic TimeCharacteristic { get; set; }

        private readonly IPermissions permissions;
        private readonly IUserDialogs userDialogs;


        
        public ClockModel Clock;
        public DelegateCommand StartCommand { get; private set; }


        private bool _activityIndicatorVisibility = false;
        public bool IsActivityIndicatorRunning
        {
            get { return  _activityIndicatorVisibility; }
            set {
                IsButtonVisible = !value;
                SetProperty(ref _activityIndicatorVisibility, value); }
        }
        private bool _isButtonVisible = true;
        public bool IsButtonVisible
        {
            get { return _isButtonVisible; }
            set { SetProperty(ref _isButtonVisible, value); }
        }
    
        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "START PAGE";
            _navigationService = navigationService;
            Ble = CrossBluetoothLE.Current;
            Adapter = CrossBluetoothLE.Current.Adapter;
            permissions = CrossPermissions.Current;
            userDialogs = UserDialogs.Instance;

            StartCommand = new DelegateCommand(Start);
        
        }

        private async void Start()
        {
            IsActivityIndicatorRunning = true;
            await CheckConnectionAbility();
            IsActivityIndicatorRunning = false;

        }

        private async Task  CheckConnectionAbility()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                var status = await permissions.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var permissionResult = await permissions.RequestPermissionsAsync(Permission.Location);

                    if (permissionResult.First().Value != PermissionStatus.Granted)
                    {
                        await userDialogs.AlertAsync("Aby korzystać z aplikacji włącz lokalizację.");
                        permissions.OpenAppSettings();
                        return;
                    }
                }
            }

            if (Ble.IsOn)
            {
                await Connect();
            }
            else
            {
                await userDialogs.AlertAsync("Aby korzystać z aplikacji włącz bluetooth."); //TODO open app settings 
            }
        }

        private async Task Connect()
        {
            try
            {
                BleDevice = await Adapter.ConnectToKnownDeviceAsync(Constants.DeviceGuid); //TODO add cancellation token
                Service = await BleDevice.GetServiceAsync(Constants.ServiceGuid);
                SwitchCharacteristic = await Service.GetCharacteristicAsync(Constants.SwitchGuid);
                BrightnessCharacteristic = await Service.GetCharacteristicAsync(Constants.BrightnessGuid);
                ColorCharacteristic = await Service.GetCharacteristicAsync(Constants.ColorGuid);
                TimeCharacteristic = await Service.GetCharacteristicAsync(Constants.TimeGuid);

                await TimeCharacteristic.WriteAsync(BitConverter.GetBytes(Convert.ToInt32(DateTime.Now.ToString("ddMMHHmm"))));

                await GetClock();
                
                await NavigateToMainPage();
            }
            catch (DeviceConnectionException e)
            {
                Console.WriteLine(e);
                await userDialogs.AlertAsync("Nie można połączyć z zegarem.");
            }
        }

        private async Task NavigateToMainPage()
        {
            var navigationParams = new NavigationParameters
            {
                { "Clock", Clock },
                { "SwitchCharacteristic", SwitchCharacteristic },
                { "BrightnessCharacteristic", BrightnessCharacteristic },
                { "ColorCharacteristic", ColorCharacteristic }
            };

            await _navigationService.NavigateAsync("MainPage", navigationParams);
        }

        private async Task GetClock()
        {
            var switchResult = await SwitchCharacteristic.ReadAsync();
            var colorResult = await ColorCharacteristic.ReadAsync();
            var brightnessResult = await BrightnessCharacteristic.ReadAsync();

            var color = BitConverter.ToInt32(colorResult, 0);
            //var brightness = Encoding.UTF8.GetString(brightnessResult); 

            Clock = new ClockModel()
            {
                On = Convert.ToBoolean(switchResult[0]),
                /*Brightness = new BrightnessSettings()
                {
                    Day = Convert.ToDouble(brightness.Substring(0,3)),
                    NightMode = brightness[3] == '1' ? true : false,
                    Night = Convert.ToDouble(brightness.Substring(4, 3)),
                    StartTime = new TimeSpan(Convert.ToInt32(brightness.Substring(7, 2)), Convert.ToInt32(brightness.Substring(9, 2)), 00),
                    EndTime = new TimeSpan(Convert.ToInt32( brightness.Substring(11, 2)), Convert.ToInt32(brightness.Substring(13, 2)), 00) 
                },
                */
                Brightness = new BrightnessSettings(),
                Color = new ColorSettings() { R = color/1000000, G = (color% 1000000)/1000, B = color%1000 }
            };
        }
    }
}