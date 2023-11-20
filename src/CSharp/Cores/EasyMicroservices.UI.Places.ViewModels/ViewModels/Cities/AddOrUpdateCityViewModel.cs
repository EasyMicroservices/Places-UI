using EasyMicroservices.ServiceContracts;
using EasyMicroservices.UI.Cores;
using EasyMicroservices.UI.Cores.Commands;
using Places.GeneratedServices;

namespace EasyMicroservices.UI.Places.ViewModels.Cities
{
    public class AddOrUpdateCityViewModel : BaseViewModel
    {
        public AddOrUpdateCityViewModel(CityClient citiyClient, ProvinceClient provinceClient)
        {
            _citiyClient = citiyClient;
            _provinceClient = provinceClient;
            SaveCommand = new TaskRelayCommand(this, Save);
            Clear();
        }

        public TaskRelayCommand SaveCommand { get; set; }

        readonly CityClient _citiyClient;
        readonly ProvinceClient _provinceClient;

        public Action OnSuccess { get; set; }
        CityContract _UpdateCityContract;
        /// <summary>
        /// for update
        /// </summary>
        public CityContract UpdateCityContract
        {
            get
            {
                return _UpdateCityContract;
            }
            set
            {
                if (value is not null)
                {
                    Name = value.Name;
                    SelectedProvinceId = value.ProvinceId.GetValueOrDefault();
                }
                _UpdateCityContract = value;
            }
        }

        string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        ICollection<ProvinceContract> _Provinces;
        public ICollection<ProvinceContract> Provinces
        {
            get => _Provinces;
            set
            {
                _Provinces = value;
                OnPropertyChanged(nameof(Provinces));
            }
        }

        public long SelectedProvinceId { get; set; }

        public async Task Save()
        {
            if (UpdateCityContract is not null)
                await UpdateCity();
            else
                await AddCity();
            OnSuccess?.Invoke();
        }

        public async Task AddCity()
        {
            await _citiyClient.AddAsync(new CreateCityRequestContract()
            {
                Names = GetNames(),
                ProvinceId = SelectedProvinceId
            }).AsCheckedResult(x => x.Result);
            Clear();
        }

        public override Task OnError(Exception exception)
        {
            return base.OnError(exception);
        }

        public override Task DisplayFetchError(ServiceContracts.ErrorContract errorContract)
        {
            return base.DisplayFetchError(errorContract);
        }

        public async Task UpdateCity()
        {
            await _citiyClient.UpdateChangedValuesOnlyAsync(new UpdateCityRequestContract()
            {
                Id = UpdateCityContract.Id,
                Names = GetNames(),
                ProvinceId = SelectedProvinceId
            }).AsCheckedResult(x => x.Result);
            Clear();
        }


        T GetCurrentProperty<T>(Func<CityContract, T> func)
        {
            return UpdateCityContract == null ? default : func(UpdateCityContract);
        }

        List<LanguageDataContract> GetNames()
        {
            return new List<LanguageDataContract>()
            {
                new LanguageDataContract()
                {
                    Data = Name,
                    Language = "fa-IR"
                }
            };
        }

        public async Task LoadConfig()
        {
            var items = await _provinceClient.GetAllByLanguageAsync(new GetByLanguageRequestContract()
            {
                Language = "fa-IR"
            }).AsCheckedResult(x=>x.Result);
            Provinces = items;
        }

        public void Clear()
        {
            Name = "";
            UpdateCityContract = default;
            Provinces = null;
        }
    }
}
