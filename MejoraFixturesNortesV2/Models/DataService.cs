using System.Collections.ObjectModel;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Services
{
    public static class DataService
    {
        public static ObservableCollection<FixtureItem> Fixtures { get; set; } = new();
    }
}