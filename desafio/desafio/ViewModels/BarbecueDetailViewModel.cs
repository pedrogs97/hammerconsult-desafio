using desafio.Models;
using desafio.Views;
using System;
using System.Collections.ObjectModel;
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
        private string _textButton;
        private float _total;
        private float _collected;
        private float _estimated;
        private Barbecue _barbecue;
        private bool creator;
        private readonly Page Page;
       
        public Command LeaveCommand { get; }
        public Command PaidCommand { get; }
        public Command TapCommand { get; }
        public Command ShareCommand { get; }
        public Command OpenModalCommand { get; }
        public ObservableCollection<ParticipantsModel> Participants
        {
            get => _participants;
            set => SetProperty(ref _participants, value);
        }
        public string TextButton
        {
            get => _textButton;
            set => SetProperty(ref _textButton, value);
        }
        public float Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }
        public float Estimated
        {
            get => _estimated;
            set => SetProperty(ref _estimated, value);
        }
        public float Collected
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
            Participants = new ObservableCollection<ParticipantsModel>();
            TapCommand = new Command<ParticipantsModel>(Tap);
            PaidCommand = new Command<string>(Paid);
            ShareCommand = new Command(ShareLink);
            LeaveCommand = new Command(Leave);
            OpenModalCommand = new Command(OpenModal);
        }

        private async void Leave()
        {
            if (creator)
            {
                ServiceBarbecue.DeleteItem(Barbecue.Id);
                await Page.Navigation.PopAsync();
            }
            else
            {
                var user = ServicePerson.GetItem(App.Current.Properties["user"].ToString());
                Barbecue.Participants.Remove(user);
                var p = new ParticipantsModel();
                foreach (var participant in Participants)
                {
                    if (participant.Id == user.Id)
                    {
                        p = participant;
                        break;
                    }
                }
                Participants.Remove(p);
                await Page.Navigation.PopAsync();
            }
        }

        private async void OpenModal()
        {
            await Page.Navigation.PushModalAsync(new FinancesPage(Barbecue.Id));
        }
        private async void Tap(ParticipantsModel participant)
        {
            if (creator)
                if (await Page.DisplayAlert("Ação", "Remover participante?", "Sim", "Não"))
                {
                    Participants.Remove(participant);
                    Barbecue.Participants.Remove(ServicePerson.GetItem(participant.Id));
                    UpdateValues();
                }
        }
        private async void Paid(string id)
        {
            if (creator)
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
                        UpdateParticipantPayment(person.Id, false);
                    }
                }
                else
                {
                    confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {person.Name} pagou o valor inteiro?", "Sim", "Não");
                    if (confirmed)
                    {
                        Barbecue.TotalCollected += 20.00f;
                        Barbecue.ParticipantsPaid.Add(person);
                        UpdateParticipantPayment(person.Id, true);
                    }
                }
                if (confirmed)
                {
                    ServiceBarbecue.UpdateItem(Barbecue);
                    Collected = Barbecue.TotalCollected;
                    Total = Collected - Barbecue.TotalSpent;
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
                var userInvated = new Person
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Novo Convidado"
                };
                ServicePerson.AddItem(userInvated);
                App.Current.Properties["user"] = userInvated.Id;
                App.Current.MainPage = new NavigationPage(new InitialPage());
            }
        }

        private void UpdateParticipantPayment(string id, bool paid)
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
        private void UpdateValues()
        {
            var estimated = 0.00f;
            var collected = 0.00f;
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
                {
                    participant.Paid = true;
                    collected += 20;
                }
                if (Barbecue.Participants.Count != Participants.Count)
                {
                    if (participant.Paid)
                        Participants.Insert(0, participant);
                    else
                        Participants.Add(participant);
                }
                estimated += 20.00f;
            });
            Estimated = estimated;
            Total = Barbecue.TotalCollected - Barbecue.TotalSpent;
            Collected = Barbecue.TotalCollected = collected;
        }
        public void OnAppearing(string id)
        {
            Barbecue = ServiceBarbecue.GetItem(id);

            if (ServicePerson.GetItem(App.Current.Properties["user"].ToString()).Id == Barbecue.Creator.Id)
            {
                creator = true;
                TextButton = "Desfazer churrasco";
            }
            else
            {
                creator = false;
                TextButton = "Deixar churrasco";
            }

            UpdateValues();
        }        
    }
}
