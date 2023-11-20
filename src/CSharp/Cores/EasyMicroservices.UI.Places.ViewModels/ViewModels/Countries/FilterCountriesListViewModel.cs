using EasyMicroservices.ServiceContracts;
using EasyMicroservices.UI.Cores;
using EasyMicroservices.UI.Cores.Commands;
using EasyMicroservices.UI.Cores.Interfaces;
using Places.GeneratedServices;
using System.Collections.ObjectModel;

namespace EasyMicroservices.UI.Places.ViewModels.Countries
{
    public class FilterCountriesListViewModel : BaseViewModel
    {
        public FilterCountriesListViewModel(CountryClient countryClient)
        {
            _countryClient = countryClient;
            SearchCommand = new TaskRelayCommand(this, Search);
            DeleteCommand = new TaskRelayCommand<CountryContract>(this, Delete);
            SearchCommand.Execute(null);
        }

        public ICommandAsync SearchCommand { get; set; }
        public ICommandAsync DeleteCommand { get; set; }

        public Action<CountryContract> OnDelete { get; set; }
        readonly CountryClient _countryClient;
        CountryContract _SelectedCountryContract;
        public CountryContract SelectedCountryContract
        {
            get => _SelectedCountryContract;
            set
            {
                _SelectedCountryContract = value;
                OnPropertyChanged(nameof(SelectedCountryContract));
            }
        }

        public int Index { get; set; } = 0;
        public int Length { get; set; } = 10;
        public int TotalCount { get; set; }
        public string SortColumnNames { get; set; }
        public ObservableCollection<CountryContract> Countries { get; set; } = new ObservableCollection<CountryContract>();

        private async Task Search()
        {
            var filteredResult = await _countryClient.FilterAsync(new FilterRequestContract()
            {
                IsDeleted = false,
                Index = Index,
                Length = Length,
                SortColumnNames = SortColumnNames
            }).AsCheckedResult(x => (x.Result, x.TotalCount));

            Countries.Clear();
            TotalCount = (int)filteredResult.TotalCount;
            foreach (var country in filteredResult.Result)
            {
                Countries.Add(country);
            }
        }

        public async Task Delete(CountryContract contract)
        {
            await _countryClient.SoftDeleteByIdAsync(new Int64SoftDeleteRequestContract()
            {
                Id = contract.Id,
                IsDelete = true
            }).AsCheckedResult(x => x);
            Countries.Remove(contract);
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

