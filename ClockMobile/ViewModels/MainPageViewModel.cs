using ClockMobile.Models;
using Plugin.BLE.Abstractions.Contracts;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ClockMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IApplicationLifecycleAware
    {
        private readonly INavigationService _navigationService;
        private INavigationParameters saveParameters;
        private ICharacteristic SwitchCharacteristic { get; set; }
        private ICharacteristic BrightnessCharacteristic { get; set; }
        private ICharacteristic ColorCharacteristic { get; set; }
        private ICharacteristic SnakeCharacteristic { get; set; }

        public DelegateCommand SendSwitchCommand { get; private set; }
        public DelegateCommand SendBrightnessCommand { get; private set; }
        public DelegateCommand SendColorCommand { get; private set; }
        public DelegateCommand PlaySnakeCommand { get; private set; }



        private TimeSpan _startTime;
        public TimeSpan StartTime {
            get { return _startTime; }
            set 
            {    
                SetProperty(ref _startTime, value);
                if (clockExists)
                {
                    Clock.Brightness.StartTime = value;
                    new Task(SendBrightness).Start();
                }
            } 
        }

        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get { return _endTime; }
            set
            {
                SetProperty(ref _endTime, value);
                if (clockExists)
                {
                    Clock.Brightness.EndTime = value;
                    new Task(SendBrightness).Start();
                }
            }
        }


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
            _navigationService = navigationService;
            Clock = new ClockModel();
            StartTime = new TimeSpan();
            EndTime = new TimeSpan();
            SendSwitchCommand = new DelegateCommand(SendSwitch);
            SendBrightnessCommand = new DelegateCommand(SendBrightness);
            SendColorCommand = new DelegateCommand(SendColor);
            PlaySnakeCommand = new DelegateCommand(PlaySnake);
        }


        private bool clockExists = false;
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            saveParameters = parameters;
            SwitchCharacteristic = parameters.GetValue<ICharacteristic>("SwitchCharacteristic");
            BrightnessCharacteristic = parameters.GetValue<ICharacteristic>("BrightnessCharacteristic");
            ColorCharacteristic = parameters.GetValue<ICharacteristic>("ColorCharacteristic");
            SnakeCharacteristic = parameters.GetValue<ICharacteristic>("SnakeCharacteristic");
            Clock = parameters.GetValue<ClockModel>("Clock");
            clockExists = true;
            StartTime = Clock.Brightness.StartTime;
            EndTime = Clock.Brightness.EndTime;
        }

        private bool sendingSomething = false;
        private async void SendSwitch()
        {
            if (!sendingSomething)
            {
                sendingSomething = true;
                await SwitchCharacteristic.WriteAsync(BitConverter.GetBytes(Clock.On));
                sendingSomething = false;
            }
        }
        private async void SendBrightness()
        {
            if (!sendingSomething)
            {
                sendingSomething = true;
                await BrightnessCharacteristic.WriteAsync(Encoding.UTF8.GetBytes(Clock.Brightness.ToSend));
                sendingSomething = false;
            }
        }
        private async void SendColor()
        {
            if (!sendingSomething)
            {
                sendingSomething = true;
                await ColorCharacteristic.WriteAsync(BitConverter.GetBytes(Clock.Color.ToSend));
                sendingSomething = false;
            }
    
        }

        private async void PlaySnake()
        {

            await SnakeCharacteristic.WriteAsync(Snake.Start);
            var navigationParams = saveParameters;
            navigationParams.Add("SnakeCharacteristic", SnakeCharacteristic);
            await _navigationService.NavigateAsync("SnakePage", navigationParams );
        }

        public async void OnResume()
        {
            try
            {
                await SwitchCharacteristic.ReadAsync();
            }
            catch
            {
                await _navigationService.NavigateAsync("StartPage");
            }
        }

        public void OnSleep()
        {
            
        }
    }
}