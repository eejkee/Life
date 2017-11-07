using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace eejkee.Life
{
    public class World
    {
        public static class Rules
        {
            public static readonly int CellsToBorn = 3;
            public static readonly int CellsToLifeMax = 3;
            public static readonly int CellsToLifeMin = 2;
        }

        public readonly int Size = 200;

        private static readonly int RandomMax = 3;
        private static readonly int Period = 42;

        private Cell[,] _cells;
        private CancellationTokenSource _doneCts = new CancellationTokenSource();

        public event Action<World> NewState;

        public void Start() =>
            Task.Run(async () =>
            {
                _cells = GenerateInitialState();
                NewState?.Invoke(this);
                while (!_doneCts.Token.IsCancellationRequested)
                {
                    Thread.Sleep(Period);
                    _cells = await GenerateNewState(_cells);
                    NewState?.Invoke(this);
                }
            }, _doneCts.Token);

        public void Stop() => _doneCts.Cancel();

        public Cell GetCell(int x, int y) => _cells[CheckIndex(x), CheckIndex(y)];

        private Cell[,] GenerateInitialState()
        {
            var cells = new Cell[Size, Size];
            var rnd = new Random();
            var rndC = new Random(42);
            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    var cr = rndC.Next(1, 100);
                    var color = Color.FromArgb(255, (byte)(cr < 34 ? 255 : 1), (byte)(cr >= 34 && cr < 67 ? 255 : 1), (byte)(cr >= 67 ? 255 : 1));
                    cells[x, y] = new Cell(rnd.Next(1, RandomMax) == 1, color);
                }
            }
            return cells;
        }

        private Task<Cell[,]> GenerateNewState(Cell[,] oldState) =>
            Task.Run(() =>
            {
                var cells = new Cell[Size, Size];
                for (var x = 0; x < Size; x++)
                {
                    for (var y = 0; y < Size; y++)
                    {
                        var neighbors = GetNeighbors(x, y, oldState);
                        var neighborCount = neighbors.Count;
                        if (oldState[x, y].State == true)
                            cells[x, y] = new Cell(neighborCount <= Rules.CellsToLifeMax && neighborCount >= Rules.CellsToLifeMin, oldState[x, y].Color);
                        else if (oldState[x, y].State == false)
                            cells[x, y] = new Cell(neighborCount == Rules.CellsToBorn, neighborCount == Rules.CellsToBorn ? GetNeighborAverageColor(neighbors) : oldState[x, y].Color);
                        else
                            cells[x, y] = oldState[x, y];
                    }
                }
                return cells;
            });

        private List<Cell> GetNeighbors(int x, int y, Cell[,] cells)
        {
            var neighbs = new List<Cell>();
            var nx = CheckIndex(x - 1);
            var ux = CheckIndex(x + 1);
            var ny = CheckIndex(y - 1);
            var uy = CheckIndex(y + 1);
            Cell cell;
            cell = cells[nx, ny]; if (cell.State) neighbs.Add(cell);
            cell = cells[nx, y]; if (cell.State) neighbs.Add(cell);
            cell = cells[nx, uy]; if (cell.State) neighbs.Add(cell);
            cell = cells[ux, ny]; if (cell.State) neighbs.Add(cell);
            cell = cells[ux, y]; if (cell.State) neighbs.Add(cell);
            cell = cells[ux, uy]; if (cell.State) neighbs.Add(cell);
            cell = cells[x, ny]; if (cell.State) neighbs.Add(cell);
            cell = cells[x, uy]; if (cell.State) neighbs.Add(cell);
            return neighbs;
        }

        private Color GetNeighborAverageColor(List<Cell> neibghs)
        {
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
            neibghs.ForEach(n =>
            {
                sumA += n.Color.A;
                sumR += n.Color.R;
                sumG += n.Color.G;
                sumB += n.Color.B;
            });
            return Color.FromArgb((byte)(sumA / neibghs.Count),
                (byte)(sumR / neibghs.Count),
                (byte)(sumG / neibghs.Count),
                (byte)(sumB / neibghs.Count));
        }

        private int CheckIndex(int index) => index < 0 ? index += Size : index >= Size ? index -= Size : index;
    }
}
