using EasyMicroservices.ServiceContracts;
using EasyMicroservices.UI.Cores;
using EasyMicroservices.UI.Cores.Commands;
using EasyMicroservices.UI.Cores.Interfaces;
using Places.GeneratedServices;
using System.Collections.ObjectModel;

namespace EasyMicroservices.UI.Places.ViewModels.Cities
{
    public class FilterCitiesListViewModel : BaseViewModel
    {
        public FilterCitiesListViewModel(CityClient cityClient)
        {
            _cityClient = cityClient;
            SearchCommand = new TaskRelayCommand(this, Search);
            DeleteCommand = new TaskRelayCommand<CityContract>(this, Delete);
            SearchCommand.Execute(null);
        }

        public ICommandAsync SearchCommand { get; set; }
        public ICommandAsync DeleteCommand { get; set; }

        public Action<CityContract> OnDelete { get; set; }
        readonly CityClient _cityClient;
        CityContract _SelectedCityContract;
        public CityContract SelectedCityContract
        {
            get => _SelectedCityContract;
            set
            {
                _SelectedCityContract = value;
                OnPropertyChanged(nameof(SelectedCityContract));
            }
        }

        public int Index { get; set; } = 0;
        public int Length { get; set; } = 10;
        public int TotalCount { get; set; }
        public string SortColumnNames { get; set; }
        public ObservableCollection<CityContract> Cities { get; set; } = new ObservableCollection<CityContract>();

        public async Task Search()
        {
            var filteredResult = await _cityClient.FilterAsync(new FilterRequestContract()
            {
                IsDeleted = false,
                Index = Index,
                Length = Length,
                SortColumnNames = SortColumnNames
            }).AsCheckedResult(x => (x.Result, x.TotalCount));

            Cities.Clear();
            TotalCount = (int)filteredResult.TotalCount;
            foreach (var city in filteredResult.Result)
            {
                Cities.Add(city);
            }
        }

        public async Task Delete(CityContract contract)
        {
            await _cityClient.SoftDeleteByIdAsync(new Int64SoftDeleteRequestContract()
            {
                Id = contract.Id,
                IsDelete = true
            }).AsCheckedResult(x => x);
            Cities.Remove(contract);
            OnDelete?.Invoke(contract);
        }

        public override Task OnError(Exception exception)
        {
            return base.OnError(exception);
        }

        public override Task DisplayFetchError(ServiceContracts.ErrorContract errorContract)
        {
            return base.DisplayFetchError(errorContract);
        }
    }
}

