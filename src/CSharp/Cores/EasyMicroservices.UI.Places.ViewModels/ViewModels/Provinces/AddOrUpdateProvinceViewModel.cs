using EasyMicroservices.ServiceContracts;
using EasyMicroservices.UI.Cores;
using EasyMicroservices.UI.Cores.Commands;
using Places.GeneratedServices;

namespace EasyMicroservices.UI.Places.ViewModels.Provinces
{
    public class AddOrUpdateProvinceViewModel : BaseViewModel
    {
        public AddOrUpdateProvinceViewModel(ProvinceClient citiyClient, CountryClient countryClient)
        {
            _citiyClient = citiyClient;
            _countryClient = countryClient;
            SaveCommand = new TaskRelayCommand(this, Save);
            Clear();
        }

        public TaskRelayCommand SaveCommand { get; set; }

        readonly ProvinceClient _citiyClient;
        readonly CountryClient _countryClient;

        public Action OnSuccess { get; set; }
        ProvinceContract _UpdateProvinceContract;
        /// <summary>
        /// for update
        /// </summary>
        public ProvinceContract UpdateProvinceContract
        {
            get
            {
                return _UpdateProvinceContract;
            }
            set
            {
                if (value is not null)
                {
                    Name = value.Name;
                }
                _UpdateProvinceContract = value;
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

        ICollection<CountryContract> _Countries;
        public ICollection<CountryContract> Countries
        {
            get => _Countries;
            set
            {
                _Countries = value;
                OnPropertyChanged(nameof(Countries));
            }
        }

        public long SelectedCountryId { get; set; }

        public async Task Save()
        {
            if (UpdateProvinceContract is not null)
                await UpdateProvince();
            else
                await AddProvince();
            OnSuccess?.Invoke();
        }

        public async Task AddProvince()
        {
            await _citiyClient.AddAsync(new CreateProvinceRequestContract()
            {
                Names = GetNames(),
                CountryId = SelectedCountryId
            }).AsCheckedResult(x => x.Result);
            Clear();
        }

        public async Task LoadConfig()
        {
            var items = await _countryClient.GetAllByLanguageAsync(new GetByLanguageRequestContract()
            {
                LanguageShortName = "fa-IR"
            }).AsCheckedResult(x => x.Result);
            Countries = items;
        }

        public async Task UpdateProvince()
        {
            await _citiyClient.UpdateChangedValuesOnlyAsync(new UpdateProvinceRequestContract()
            {
                Id = UpdateProvinceContract.Id,
                Names = GetNames(),
                CountryId = SelectedCountryId
            }).AsCheckedResult(x => x.Result);
            Clear();
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

        public void Clear()
        {
            Name = "";
            UpdateProvinceContract = default;
        }
    }
}
