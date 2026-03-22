using System.Collections.ObjectModel;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Services
{
    public static class NotasService
    {
        public static ObservableCollection<Nota> Notas { get; set; } = new ObservableCollection<Nota>();
    }
}