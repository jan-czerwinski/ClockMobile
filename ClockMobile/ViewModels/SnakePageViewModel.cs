using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClockMobile.ViewModels
{
    public class SnakePageViewModel : ViewModelBase
    {
        private INavigationService  _navigationService;
        public DelegateCommand TurnRightCommand { get; private set; }
        public DelegateCommand TurnLeftCommand { get; private set; }
        public DelegateCommand EndSnakeCommand { get; private set; }


        public SnakePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            TurnRightCommand = new DelegateCommand(TurnRight);
            TurnLeftCommand = new DelegateCommand(TurnLeft);
            EndSnakeCommand = new DelegateCommand(EndSnake);
        }
        private void TurnRight() { Turn(true); }
        private void TurnLeft() { Turn(false); }

        private async void EndSnake()
        {
            //TODO
            await _navigationService.NavigateAsync("MainPage");
        }

        private void Turn(bool turnRight)
        {
            throw new NotImplementedException();
        }
    }
}
