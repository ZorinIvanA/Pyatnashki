namespace Pyatnashki
{
    public class ButtonViewModel
    {
        public int Position { get; set; }
        public int? Value { get; set; }

        public int XPosition => Position % 4;
        public int YPosition => Position / 4;

        public ButtonViewModel(int position, int? value)
        {
            Position = position;
            Value = value;
        }
    }
}
