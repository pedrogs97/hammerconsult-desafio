using desafio.Models;
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
        private string _estimated;
        private Barbecue _barbecue;
        private bool _enable;
        private readonly Page Page;
       
        public Command PaidCommand { get; }
        public Command ShareCommand { get; set; }
        public bool EnableButton
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }
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
        public Barbecue Barbecue
        {
            get => _barbecue;
            set => SetProperty(ref _barbecue, value);
        }
        public BarbecueDetailViewModel(Page page)
        {
            Page = page;
            Participants = new ObservableCollection<ParticipantsModel>();
            PaidCommand = new Command<string>(Paid);
            ShareCommand = new Command(ShareLink);
        }

        private async void Paid(string id)
        {
            var person = ServicePerson.GetItem(id);
            bool confirmed;
            if (Barbecue.ParticipantsPaid.Contains(person))
            {
                confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {person.Name} não pagou o valor inteiro?", "Sim", "Não");
                if (confirmed)
                {
                    Barbecue.ParticipantsPaid.Remove(person);
                    UpdateParticipants(person.Id, false);
                }
            }
            else
            {
                confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {person.Name} pagou o valor inteiro?", "Sim", "Não");
                if (confirmed)
                {
                    Barbecue.ParticipantsPaid.Add(person);
                    UpdateParticipants(person.Id, true);
                }
            }
            if (confirmed)
                ServiceBarbecue.UpdateItem(Barbecue);
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
            Participants.Insert(0,p);
        }

        public void OnAppearing(string id)
        {
            EnableButton = false;
            Barbecue = ServiceBarbecue.GetItem(id);
            var estimated = 0.00;
            if (Barbecue.Participants.Count != Participants.Count)
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
                    Participants.Add(participant);
                    estimated += 20.00;
                });
            Estimated = estimated.ToString();
        }

        
    }
}
