using System.Windows;

namespace eejkee.Life
{
    public partial class App : Application
    {
        public App()
        {
            var model = new World();
            var vm = new WorldViewModel(model);
            var view = new WorldView();
            view.DataContext = vm;
            view.Closed += (s, a) => model.Stop();
            view.Show();
            model.Start();
        }
    }
}
