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
        private ICharacteristic SnakeCharacteristic { get; set; }


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
        private readonly CancellationTokenSource cts;
        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "START PAGE";
            cts = new CancellationTokenSource();// stops trying to connect after 10 seconds
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
            var deviceGuid = Device.RuntimePlatform == Device.Android ? Guids.DeviceOnAndroidGuid : Guids.DeviceOnIosGuid;
            if (Adapter.ConnectedDevices.Any(connectedDevice => connectedDevice.Id == deviceGuid))
            {
                await NavigateToMainPage();
            }
            else
            {
                try
                {
                    cts.CancelAfter(10000);
                    BleDevice = await Adapter.ConnectToKnownDeviceAsync(deviceGuid, cancellationToken: cts.Token);
                    Service = await BleDevice.GetServiceAsync(Guids.ServiceGuid);
                    SwitchCharacteristic = await Service.GetCharacteristicAsync(Guids.SwitchGuid);
                    BrightnessCharacteristic = await Service.GetCharacteristicAsync(Guids.BrightnessGuid);
                    ColorCharacteristic = await Service.GetCharacteristicAsync(Guids.ColorGuid);
                    TimeCharacteristic = await Service.GetCharacteristicAsync(Guids.TimeGuid);
                    SnakeCharacteristic = await Service.GetCharacteristicAsync(Guids.SnakeGuid);


                    await TimeCharacteristic.WriteAsync(BitConverter.GetBytes(Convert.ToInt32(DateTime.Now.ToString("ddMMHHmm"))));

                    await GetClock();
                
                    await NavigateToMainPage();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    await userDialogs.AlertAsync("Nie można połączyć z zegarem.");
                }
            }
        }

        private async Task NavigateToMainPage()
        {
            var navigationParams = new NavigationParameters
            {
                { "Clock", Clock },
                { "SwitchCharacteristic", SwitchCharacteristic },
                { "BrightnessCharacteristic", BrightnessCharacteristic },
                { "ColorCharacteristic", ColorCharacteristic },
                { "SnakeCharacteristic", SnakeCharacteristic },

            };

            await _navigationService.NavigateAsync("MainPage", navigationParams);
        }

        private async Task GetClock()
        {
            var switchResult = await SwitchCharacteristic.ReadAsync();
            var colorResult = await ColorCharacteristic.ReadAsync();
            var brightness = await BrightnessCharacteristic.ReadAsync();

            var color = BitConverter.ToInt32(colorResult, 0);

            Clock = new ClockModel()
            {
                On = Convert.ToBoolean(switchResult[0]),
                Brightness = new BrightnessSettings()
                {
                    Day = Convert.ToDouble(String.Format("{0}{1}{2}", brightness[0], brightness[1], brightness[2])),
                    NightMode = String.Format("{0}", brightness[3]) == "1" ? true : false,
                    Night = Convert.ToDouble(String.Format("{0}{1}{2}", brightness[4], brightness[5], brightness[6])),
                    StartTime = new TimeSpan(Convert.ToInt32(String.Format("{0}{1}", brightness[7], brightness[8])),
                                            Convert.ToInt32(String.Format("{0}{1}", brightness[9], brightness[10])), 00),
                    EndTime = new TimeSpan(Convert.ToInt32(String.Format("{0}{1}", brightness[11], brightness[12])),
                                            Convert.ToInt32(String.Format("{0}{1}", brightness[13], brightness[14])), 00),
                },
                Color = new ColorSettings() { R = color/1000000, G = (color% 1000000)/1000, B = color%1000 }
            };
        }
    }
}