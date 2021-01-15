using desafio.Models;
using desafio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class InitialViewModel : BaseViewModel
    {
        private ObservableCollection<Barbecue> _barbecues;
        public ObservableCollection<Barbecue> Barbecues
        {
            get => _barbecues;
            set => SetProperty(ref _barbecues, value);
        }
        private INavigation Navigation { get; }
        public Command ToAddCommad { get; }
        public Command ToDetailCommand { get; }
        public InitialViewModel(INavigation navigation)
        {
            Title = "Home";
            Navigation = navigation;
            ToAddCommad = new Command(async () => {
                await Navigation.PushAsync(new AddBarbecuePage());
            });
            ToDetailCommand = new Command<string>(async (idBarbecue) => {
                await Navigation.PushAsync(new BarbecueDetail(idBarbecue),true);
            });
            Barbecues = new ObservableCollection<Barbecue>();
        }
        public async void OnAppearing()
        {
            Barbecues = LoadBarbecue();
            if (!App.Current.Properties.ContainsKey("user"))
            {
                var currentUser = new Person { Id = Guid.NewGuid().ToString(), Name = "Pedro Gustavo" };
                if (ServicePerson.AddItem(currentUser))
                {
                    App.Current.Properties.Add("user", currentUser.Id);
                    await App.Current.SavePropertiesAsync();
                }
                else
                    throw new NotImplementedException();
            }
        }
        private ObservableCollection<Barbecue> LoadBarbecue()
        {
            return new ObservableCollection<Barbecue>(ServiceBarbecue.GetItems());
        }
    }
}
