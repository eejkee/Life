using System.Windows.Media;

namespace eejkee.Life
{
    public class Cell
    {
        public Cell(bool state, Color color)
        {
            State = state;
            Color = color;
        }

        public bool State { get; set; }
        public Color Color { get; set; }
    }
}
