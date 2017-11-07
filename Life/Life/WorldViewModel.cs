using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace eejkee.Life
{
    public class WorldViewModel : INotifyPropertyChanged
    {
        public WorldViewModel(World world)
        {
            world.NewState += w => Application.Current.Dispatcher.BeginInvoke(new Action(async () => Frame = await RenderFrame(w)), args: null);
        }

        private ImageSource _frame;
        public ImageSource Frame
        {
            get { return _frame; }
            set
            {
                _frame = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Frame)));
            }
        }

        private Task<ImageSource> RenderFrame(World world)
        {
            return Task.Run(() =>
            {
                var size = 50;
                var dg = new DrawingGroup();
                for (var x = 0; x < world.Size; x++)
                {
                    for (var y = 0; y < world.Size; y++)
                    {
                        var cell = world.GetCell(x, y);
                        if (cell.State)
                        {
                            dg.Children.Add(new GeometryDrawing(new SolidColorBrush(cell.Color), null,
                                new RectangleGeometry(new Rect(new Point(size * x, size * y), new Size(size, size)))));
                        }
                    }
                }
                var additionalSquares = new GeometryGroup();
                dg.Children.Add(new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(new Point(0, 0), new Size(1, 1)))));
                dg.Children.Add(new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(new Point(world.Size * size - 1, world.Size * size - 1), new Size(1, 1)))));
                var di = new DrawingImage(dg);
                di.Freeze();
                return di as ImageSource;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
