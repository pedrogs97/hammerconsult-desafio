using desafio.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class OptionsParticipantViewModel : BaseViewModel
    {
        private bool _notDrink;
        private bool _notDrinkInvited;
        private bool _invited;
        private bool _isVisible;
        private bool _isEnable;
        private string _invitedName;
        private string _txtButton;
        private Person Person;
        private Barbecue Barbecue;
        private string invitedId;
        private Page Page { get; }

        public Command CloseModalCommand { get; }
        public Command RemovePersonCommand { get; }
        public Command AddInvitedCommand { get; }
        public string InvitedName 
        {
            get => _invitedName;
            set => SetProperty(ref _invitedName,value); 
        }
        public string TxtButton
        {
            get => _txtButton;
            set => SetProperty(ref _txtButton, value);
        }
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }
        public bool NotDrink
        {
            get => _notDrink;
            set => SetProperty(ref _notDrink, value, onChanged:()=> 
            { 
                Person.Drink = value;
                ServicePerson.UpdateItem(Person);
            });
        }
        public bool NotDrinkInvited
        {
            get => _notDrinkInvited;
            set => SetProperty(ref _notDrinkInvited, value);
        }
        public bool Invited
        {
            get => _invited;
            set => SetProperty(ref _invited, value, onChanged: () =>
            {
                if (value)
                    IsEnable = true;
                else
                    IsEnable = false;
            });
        }

        public OptionsParticipantViewModel(Page page)
        {
            Page = page;
            CloseModalCommand = new Command(CloseModal);
            RemovePersonCommand = new Command(RemovePerson);
            AddInvitedCommand = new Command(AddInvited);
            invitedId = String.Empty;
            IsEnable = false;
            TxtButton = "Adicionar convidado";
            Invited = false;
            NotDrinkInvited = false;
            InvitedName = String.Empty;
        }

        private async void AddInvited()
        {
            if (TxtButton == "Adicionar convidado")
            {
                if (!InvitedName.Equals(String.Empty))
                {
                    Barbecue.Participants.Add(new Person
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"{InvitedName} convidado por {Person.Name}",
                        Drink = !NotDrinkInvited,
                        InvitedBy = Person
                    });
                    ServiceBarbecue.UpdateItem(Barbecue);
                    TxtButton = "Remover convidado";
                }
                else
                    await Page.DisplayAlert("Atenção", "Informe o nome do convidado.", "OK");
            }
            else
            {
                var person = new Person();
                Barbecue.Participants.ForEach((p) =>
                {
                    if (p.Id == invitedId)
                        person = p;
                });
                Barbecue.Participants.Remove(person);
                Barbecue.ParticipantsPaid.Remove(person);
                ServiceBarbecue.UpdateItem(Barbecue);
                invitedId = String.Empty;
                TxtButton = "Adicionar convidado";
                Invited = false;
                NotDrinkInvited = false;
                InvitedName = String.Empty;
            }
        }
        private async void RemovePerson()
        {
            Barbecue.Participants.Remove(Person);
            Barbecue.ParticipantsPaid.Remove(Person);
            await Page.Navigation.PopModalAsync();
        }
        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }

        public void OnAppearing(string idPerson, string idBarbecue)
        {
            Barbecue = ServiceBarbecue.GetItem(idBarbecue);
            Person = ServicePerson.GetItem(idPerson);
            Barbecue.Participants.ForEach((person) =>
            {
                if (!(person.InvitedBy is null))
                {
                    if (person.InvitedBy.Id == Person.Id)
                    {
                        invitedId = person.Id;
                        TxtButton = "Remover convidado";
                        Invited = true;
                        InvitedName = person.Name;
                        NotDrinkInvited = person.Drink;
                    }
                }
            });
            NotDrink = Person.Drink;
            
            if (ServicePerson.GetItem(App.Current.Properties["user"].ToString()) == Barbecue.Creator)
                IsVisible = true;
            else
                IsVisible = false;
        }
    }
}
