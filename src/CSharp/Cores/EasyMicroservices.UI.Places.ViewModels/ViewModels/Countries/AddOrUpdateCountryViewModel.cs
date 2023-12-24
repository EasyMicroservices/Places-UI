using EasyMicroservices.ServiceContracts;
using EasyMicroservices.UI.Cores;
using EasyMicroservices.UI.Cores.Commands;
using Places.GeneratedServices;

namespace EasyMicroservices.UI.Places.ViewModels.Countries
{
    public class AddOrUpdateCountryViewModel : BaseViewModel
    {
        public AddOrUpdateCountryViewModel(CountryClient citiyClient)
        {
            _citiyClient = citiyClient;
            SaveCommand = new TaskRelayCommand(this, Save);
            Clear();
        }

        public TaskRelayCommand SaveCommand { get; set; }

        readonly CountryClient _citiyClient;

        public Action OnSuccess { get; set; }
        CountryContract _UpdateCountryContract;
        /// <summary>
        /// for update
        /// </summary>
        public CountryContract UpdateCountryContract
        {
            get
            {
                return _UpdateCountryContract;
            }
            set
            {
                if (value is not null)
                {
                    Name = value.Name;
                }
                _UpdateCountryContract = value;
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

        public async Task Save()
        {
            if (UpdateCountryContract is not null)
                await UpdateCountry();
            else
                await AddCountry();
            OnSuccess?.Invoke();
        }

        public async Task AddCountry()
        {
            await _citiyClient.AddAsync(new CreateCountryRequestContract()
            {
                Names = GetNames(),
            }).AsCheckedResult(x => x.Result);
            Clear();
        }

        public async Task UpdateCountry()
        {
            await _citiyClient.UpdateChangedValuesOnlyAsync(new UpdateCountryRequestContract()
            {
                Id = UpdateCountryContract.Id,
                Names = GetNames(),
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
            UpdateCountryContract = default;
        }
    }
}
