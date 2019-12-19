using Plugin.BLE.Abstractions.Contracts;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClockMobile.ViewModels
{
    public class SnakePageViewModel : ViewModelBase, IApplicationLifecycleAware
    {
        private INavigationService _navigationService;
        private ICharacteristic SnakeCharacteristic { get; set; }
        private ICharacteristic SwitchCharacteristic { get; set; }
        public DelegateCommand TurnRightCommand { get; private set; }
        public DelegateCommand TurnLeftCommand { get; private set; }
        public DelegateCommand StopSnakeCommand { get; private set; }
        public DelegateCommand ResetSnakeCommand { get; private set; }


        private bool turning;
        public SnakePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            turning = false;
            TurnRightCommand = new DelegateCommand(TurnRight);
            TurnLeftCommand = new DelegateCommand(TurnLeft);
            StopSnakeCommand = new DelegateCommand(StopSnake);
            ResetSnakeCommand = new DelegateCommand(ResetSnake);


        }
        private async void TurnRight() { await Turn(true); }
        private async void TurnLeft() { await Turn(false); }

        private async Task Turn(bool turnRight)
        {
            if (!turning)
            {
            turning = true;
            var turnDirection = turnRight ? Snake.Right : Snake.Left;
            await SnakeCharacteristic.WriteAsync(turnDirection);
            turning = false;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            SnakeCharacteristic = parameters.GetValue<ICharacteristic>("SnakeCharacteristic");
            SwitchCharacteristic = parameters.GetValue<ICharacteristic>("SwitchCharacteristic");
        }

        public async void OnResume()
        {
            try
            {
                await SwitchCharacteristic.ReadAsync();
            }
            catch
            {
                await _navigationService.GoBackToRootAsync();
            }
        }

        private async void ResetSnake()
        {
            await SnakeCharacteristic.WriteAsync(Snake.Stop);
            await SnakeCharacteristic.WriteAsync(Snake.Start);
        }
        private async void StopSnake()
        { 
            await SnakeCharacteristic.WriteAsync(Snake.Stop);
            await _navigationService.GoBackToRootAsync();
            
        }
        public async void OnSleep()
        {
            await SnakeCharacteristic.WriteAsync(Snake.Stop);
            await _navigationService.GoBackToRootAsync();
        }
    }
}
