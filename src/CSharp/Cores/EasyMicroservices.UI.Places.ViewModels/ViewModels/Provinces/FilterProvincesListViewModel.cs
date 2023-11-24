using EasyMicroservices.ServiceContracts;
using EasyMicroservices.UI.Cores;
using EasyMicroservices.UI.Cores.Commands;
using EasyMicroservices.UI.Cores.Interfaces;
using Places.GeneratedServices;
using System.Collections.ObjectModel;

namespace EasyMicroservices.UI.Places.ViewModels.Provinces
{
    public class FilterProvincesListViewModel : BaseViewModel
    {
        public FilterProvincesListViewModel(ProvinceClient provinceClient)
        {
            _provinceClient = provinceClient;
            SearchCommand = new TaskRelayCommand(this, Search);
            DeleteCommand = new TaskRelayCommand<ProvinceContract>(this, Delete);
            SearchCommand.Execute(null);
        }

        public ICommandAsync SearchCommand { get; set; }
        public ICommandAsync DeleteCommand { get; set; }

        public Action<ProvinceContract> OnDelete { get; set; }
        readonly ProvinceClient _provinceClient;
        ProvinceContract _SelectedProvinceContract;
        public ProvinceContract SelectedProvinceContract
        {
            get => _SelectedProvinceContract;
            set
            {
                _SelectedProvinceContract = value;
                OnPropertyChanged(nameof(SelectedProvinceContract));
            }
        }

        public int Index { get; set; } = 0;
        public int Length { get; set; } = 10;
        public int TotalCount { get; set; }
        public string SortColumnNames { get; set; }
        public ObservableCollection<ProvinceContract> Provinces { get; set; } = new ObservableCollection<ProvinceContract>();

        public async Task Search()
        {
            var filteredResult = await _provinceClient.FilterAsync(new FilterRequestContract()
            {
                IsDeleted = false,
                Index = Index,
                Length = Length,
                SortColumnNames = SortColumnNames
            }).AsCheckedResult(x => (x.Result, x.TotalCount));

            Provinces.Clear();
            TotalCount = (int)filteredResult.TotalCount;
            foreach (var province in filteredResult.Result)
            {
                Provinces.Add(province);
            }
        }

        public async Task Delete(ProvinceContract contract)
        {
            await _provinceClient.SoftDeleteByIdAsync(new Int64SoftDeleteRequestContract()
            {
                Id = contract.Id,
                IsDelete = true
            }).AsCheckedResult(x => x);
            Provinces.Remove(contract);
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

