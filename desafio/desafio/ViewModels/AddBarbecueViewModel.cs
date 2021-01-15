using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using desafio.Models;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class AddBarbecueViewModel: BaseViewModel
    {
        private Barbecue Barbecue;
        private Page Page { get; }
        private string _titleBarbecue;
        public string TitleBarbecue
        {
            get => _titleBarbecue;
            set => SetProperty(ref _titleBarbecue, value);
        }
        private string _description;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        private string _local;
        public string Local
        {
            get => _local;
            set => SetProperty(ref _local, value);
        }
        public DateTime BarbecueDate { get; set; }
        public Command AddBarbecue { get; }
        public AddBarbecueViewModel(Page page)
        {
            Title = "Adicionar churrasco";
            Page = page;
            AddBarbecue = new Command(async () =>
            {
                IsBusy = true;
                if (Validate())
                {
                    Barbecue = new Barbecue
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = TitleBarbecue,
                        Date = BarbecueDate,
                        Description = Description,
                        Local = Local,
                        Participants = ServicePerson.GetItems().ToList(),
                        ParticipantsPaid = new List<Person>(),
                        TotalCollected = 0.00f,
                        TotalDrink = 0.00f,
                        TotalFood = 0.00f,
                        TotalSpent = 0.00f,
                        Creator = ServicePerson.GetItem(App.Current.Properties["user"].ToString())
                    };
                    if (ServiceBarbecue.AddItem(Barbecue))
                    {
                        await Page.DisplayAlert("", "Churrasco criado com sucesso!", "OK");
                        await Page.Navigation.PopAsync(true);
                    }
                    else
                    {
                        await Page.DisplayAlert("OPS", "Não foi possível criar o churrasco.", "OK");
                        await Page.Navigation.PopAsync(true);
                    }
                }
                else
                    await Page.DisplayAlert("Atenção", "Verifique se preencheu todos os campos obrigatórios.", "OK");
            });
        }

        private bool Validate()
        {
            return !String.IsNullOrWhiteSpace(_titleBarbecue)
                && !String.IsNullOrWhiteSpace(_local)
                && (BarbecueDate != null);
        }
    }
}
