using desafio.Models;
using desafio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class BarbecueDetailViewModel : BaseViewModel
    {
        public class ParticipantsModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Paid { get; set; }
        }
        private ObservableCollection<ParticipantsModel> _participants;
        private string _collected;
        private string _estimated;
        private Barbecue _barbecue;
        private bool enablePayment;
        private readonly Page Page;
       
        public Command PaidCommand { get; }
        public Command ShareCommand { get; set; }
        public Command OpenModalCommand { get; set; }
        public ObservableCollection<ParticipantsModel> Participants
        {
            get => _participants;
            set => SetProperty(ref _participants, value);
        }
        public string Estimated
        {
            get => _estimated;
            set => SetProperty(ref _estimated, value);
        }
        public string Collected
        {
            get => _collected;
            set => SetProperty(ref _collected, value);
        }
        public Barbecue Barbecue
        {
            get => _barbecue;
            set => SetProperty(ref _barbecue, value);
        }
        public BarbecueDetailViewModel(Page page)
        {
            Page = page;
            Collected = "0";
            Participants = new ObservableCollection<ParticipantsModel>();
            PaidCommand = new Command<string>(Paid);
            ShareCommand = new Command(ShareLink);
            OpenModalCommand = new Command(OpenModal);
        }

        private async void OpenModal()
        {
            await Page.Navigation.PushModalAsync(new FinancesPage(Barbecue.Id));
        }

        private async void Paid(string id)
        {
            if (enablePayment)
            {
                var person = ServicePerson.GetItem(id);
                bool confirmed;
                if (Barbecue.ParticipantsPaid.Contains(person))
                {
                    confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {person.Name} não pagou o valor inteiro?", "Sim", "Não");
                    if (confirmed)
                    {
                        Barbecue.TotalCollected -= 20.00f;
                        Barbecue.ParticipantsPaid.Remove(person);
                        UpdateParticipants(person.Id, false);
                    }
                }
                else
                {
                    confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {person.Name} pagou o valor inteiro?", "Sim", "Não");
                    if (confirmed)
                    {
                        Barbecue.TotalCollected += 20.00f;
                        Barbecue.ParticipantsPaid.Add(person);
                        UpdateParticipants(person.Id, true);
                    }
                }
                if (confirmed)
                {
                    ServiceBarbecue.UpdateItem(Barbecue);
                    Collected = Barbecue.TotalCollected.ToString();
                }
            }
        }

        private async void ShareLink()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = "Convite para um churrasco.",
                Title = "Envie o convite para um amigo",
                Text = $"Código para participar do churras:\n{Barbecue.Id}" 
            });
            if (await Page.DisplayAlert("", "Mudar visão para convidado?", "Sim", "Não"))
            {
                // fazer alteração de user para virar convidado
            }
        }

        private void UpdateParticipants(string id, bool paid)
        {
            var p = new ParticipantsModel();
            foreach (var participant in Participants)
            {
                if (participant.Id == id)
                    p = participant;
            }
            Participants.Remove(p);
            p.Paid = paid;
            if (paid)
                Participants.Insert(0, p);
            else
                Participants.Insert(Participants.Count, p);
        }

        public void OnAppearing(string id)
        {
            Barbecue = ServiceBarbecue.GetItem(id);
            if (ServicePerson.GetItem(App.Current.Properties["user"].ToString()).Id == Barbecue.Creator.Id)
                enablePayment = true;
            else
                enablePayment = false;
            var estimated = 0.00;
            Barbecue.Participants.ForEach((person) =>
            {
                var participant = new ParticipantsModel
                {
                    Id = person.Id,
                    Name = person.Name,
                };
                if (!Barbecue.ParticipantsPaid.Contains(person))
                    participant.Paid = false;
                else
                    participant.Paid = true;
                if (Barbecue.Participants.Count != Participants.Count)
                    Participants.Add(participant);
                estimated += 20.00;
            });
            Estimated = estimated.ToString();
        }        
    }
}
