using ClockMobile.Models;
using Plugin.BLE.Abstractions.Contracts;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IBluetoothLE Ble { get; set; }
        private IAdapter Adapter { get; set; }
        private IDevice BleDevice { get; set; }
        private IService Service { get; set; }
        private ICharacteristic SwitchCharacteristic { get; set; }
        private ICharacteristic BrightnessCharacteristic { get; set; }
        private ICharacteristic ColorCharacteristic { get; set; }
        private ICharacteristic TimeCharacteristic { get; set; }
        public DelegateCommand SendSwitchCommand { get; private set; }
        public DelegateCommand SendBrightnessCommand { get; private set; }
        public DelegateCommand SendColorCommand { get; private set; }

        private ClockModel _clock;
        public ClockModel Clock
        {
            get => _clock;
            set => SetProperty(ref _clock, value);
        }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Ustawienia Zegara";
            Clock = new ClockModel();
            SendSwitchCommand = new DelegateCommand(SendSwitch);
            SendBrightnessCommand = new DelegateCommand(SendBrightness);
            SendColorCommand = new DelegateCommand(SendColor);
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Ble = parameters.GetValue<IBluetoothLE>("Ble");
            Adapter = parameters.GetValue<IAdapter>("Adapter");
            BleDevice = parameters.GetValue<IDevice>("BleDevice");
            Service = parameters.GetValue<IService>("Service");
            SwitchCharacteristic = parameters.GetValue<ICharacteristic>("SwitchCharacteristic");
            BrightnessCharacteristic = parameters.GetValue<ICharacteristic>("BrightnessCharacteristic");
            ColorCharacteristic = parameters.GetValue<ICharacteristic>("ColorCharacteristic");
            Clock = parameters.GetValue<ClockModel>("Clock");
        }

        private bool sendingSomething = false;

        private async void SendSwitch()
        {
            if (!sendingSomething)
            {
                sendingSomething = true;
                await SwitchCharacteristic.WriteAsync(BitConverter.GetBytes(Clock.On));
            }
            sendingSomething = false;
        }
        private async void SendBrightness()
        {
            if (!sendingSomething)
            {
                sendingSomething = true;
                await BrightnessCharacteristic.WriteAsync(Encoding.UTF8.GetBytes(Clock.Brightness.ToSend));
            }
            sendingSomething = false;
        }
        private async void SendColor()
        {
            if (!sendingSomething)
            {
                sendingSomething = true;
                await ColorCharacteristic.WriteAsync(BitConverter.GetBytes(Clock.Color.ToSend));
            }
            sendingSomething = false;
    
        }
    }
}