using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Pyatnashki
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ButtonViewModel> Buttons { get; set; }
        private Random _random;
        private const int BUTTONS_COUNT = 15;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RestartCommand { get; set; }
        public ICommand MoveCommand { get; set; }
        public bool IsNotWin { get; set; }
        public string Title { get; set; }

        public GameViewModel()
        {
            RestartCommand = new RelayCommand(Restart);
            RestartCommand.Execute(null);
            MoveCommand = new RelayCommand(Move);
            Title = "Play!";
        }

        public void Restart(object parameter)
        {
            Buttons = new ObservableCollection<ButtonViewModel>();
            IsNotWin = true;
            while (!CheckForPossibility())
            {
                _random = new Random(DateTime.Now.Millisecond);
                Buttons = new ObservableCollection<ButtonViewModel>();
                var buttons = new List<ButtonViewModel>();
                for (int button = 0; button < BUTTONS_COUNT; button++)
                {
                    buttons.Add(new ButtonViewModel(GetRandomPosition(buttons), button + 1));
                }

                foreach (var button in buttons.OrderBy(x => x.Position))
                    Buttons.Add(button);
                Buttons.Add(new ButtonViewModel(15, null));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Buttons)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotWin)));
        }

        private bool CheckForPossibility()
        {
            if (Buttons == null || Buttons.Count == 0)
                return false;

            int sum = 4;//Row number with empty button, starts from 1
            for (int i = 0; i < BUTTONS_COUNT; i++)
            {
                sum += Buttons.Count(x => x.Position > i && x.Value < Buttons[i].Value);
            }

            return sum % 2 == 0;
        }

        private int GetRandomPosition(IList<ButtonViewModel> existing)
        {
            int result = -1;
            while (result < 0 || existing.Any(x => x != null && x.Position == result))
            {
                result = _random.Next(BUTTONS_COUNT);
            }

            return result;
        }

        public void Move(object parameter)
        {
            var position = (int)parameter;
            var movingElement = Buttons.SingleOrDefault(x => x.Position == position);
            var emptyElement = Buttons.SingleOrDefault(x => !x.Value.HasValue);

            if ((Math.Abs(movingElement.XPosition - emptyElement.XPosition) == 1 && movingElement.YPosition == emptyElement.YPosition)
                ^ ((Math.Abs(movingElement.YPosition - emptyElement.YPosition) == 1) && movingElement.XPosition == emptyElement.XPosition))
                SwapElements(movingElement.Position, emptyElement.Position);

            CheckForVictory();
        }

        private void CheckForVictory()
        {
            IsNotWin = !Buttons.All(x => x.Value == null || x.Value == x.Position + 1);
            if (!IsNotWin)
                Title = "Victory! Press restart!";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotWin)));
        }

        private void SwapElements(int position1, int position2)
        {
            var elementOne = Buttons[position1];
            var elementTwo = Buttons[position2];

            elementTwo.Position = position1;
            elementOne.Position = position2;
            Buttons[position2] = elementOne;
            Buttons[position1] = elementTwo;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Buttons)));
        }
    }
}
