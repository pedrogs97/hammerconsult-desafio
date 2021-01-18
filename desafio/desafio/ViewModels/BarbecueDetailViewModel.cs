using desafio.Models;
using desafio.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
       
        public Command SetParticipantsCommand { get; }
        public Command SetInvitedsCommand { get; }
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
            SetParticipantsCommand = new Command(SetParticipants);
            SetInvitedsCommand = new Command(SetInviteds);
        }

        private void SetParticipants()
        {
            Participants.Clear();
            UpdateValuesParticipants();
        }
        private void SetInviteds()
        {
            Participants.Clear();
            UpdateValuesInviteds();
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
            if ((creator && !participant.Name.Contains("convidado por")) || participant.Id == App.Current.Properties["user"].ToString())
                await Page.Navigation.PushModalAsync(new OptionsParticipant(participant.Id, Barbecue.Id));
        }
        private async void Paid(string id)
        {
            if (creator)
            {
                var person = ServicePerson.GetItem(id);
                bool confirmed;
                if (person is null)
                {
                    var p = new Person();

                    Barbecue.Participants.ForEach((partcipant) =>
                    {
                        if (partcipant.Id == id)
                            p = partcipant;
                    });
                    if (Barbecue.ParticipantsPaid.Contains(p))
                    {
                        confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {p.Name} não pagou o valor inteiro?", "Sim", "Não");
                        if (confirmed)
                        {
                            Barbecue.TotalCollected -= 20.00f;
                            Barbecue.ParticipantsPaid.Remove(p);
                            UpdateParticipantPayment(p.Id, false);
                        }
                    }
                    else
                    {
                        confirmed = await Page.DisplayAlert("Atenção", $"Confirmar que {p.Name} pagou o valor inteiro?", "Sim", "Não");
                        if (confirmed)
                        {
                            Barbecue.TotalCollected += 20.00f;
                            Barbecue.ParticipantsPaid.Add(p);
                            UpdateParticipantPayment(p.Id, true);
                        }
                    }
                    if (confirmed)
                    {
                        ServiceBarbecue.UpdateItem(Barbecue);
                        Collected = Barbecue.TotalCollected;
                        Total = Collected - Barbecue.TotalSpent;
                    }
                }
                else
                {
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
        }

        private async void ShareLink()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = "Convite para um churrasco.",
                Title = "Envie o convite para um amigo",
                Text = $"Código para participar do churrasco:\n{Barbecue.Id}" 
            });
            if (await Page.DisplayAlert("", "Mudar visão para novo participante?", "Sim", "Não"))
            {
                var userInvated = new Person
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Novo Participante"
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
        private void UpdateValuesParticipants()
        {
            var estimated = 0.00f;
            var collected = 0.00f;
            Participants.Clear();
            Barbecue.Participants.ForEach((person) =>
            {
                if (person.InvitedBy is null)
                {
                    var participant = new ParticipantsModel
                    {
                        Id = person.Id,
                        Name = person.Name
                    };
                    if (!Barbecue.ParticipantsPaid.Contains(person))
                        participant.Paid = false;
                    else
                    {
                        participant.Paid = true;
                        collected += 20.00f;
                    }
                    if (participant.Paid)
                        Participants.Insert(0, participant);
                    else
                        Participants.Add(participant);
                }
                if (person.Drink)
                    estimated += 20.00f;
                else
                    estimated += 10.00f;
            });
            Estimated = estimated;
            Total = Barbecue.TotalCollected - Barbecue.TotalSpent;
            Collected = Barbecue.TotalCollected = collected;
        }
        private void UpdateValuesInviteds()
        {
            var estimated = 0.00f;
            var collected = 0.00f;
            Participants.Clear();
            Barbecue.Participants.ForEach((person) =>
            {
                if (!(person.InvitedBy is null))
                {
                    var participant = new ParticipantsModel
                    {
                        Id = person.Id,
                        Name = person.Name
                    };
                    if (!Barbecue.ParticipantsPaid.Contains(person))
                        participant.Paid = false;
                    else
                    {
                        participant.Paid = true;
                        collected += 20.00f;
                    }
                    if (participant.Paid)
                        Participants.Insert(0, participant);
                    else
                        Participants.Add(participant);
                }
                if (person.Drink)
                    estimated += 20.00f;
                else
                    estimated += 10.00f;
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
            UpdateValuesParticipants();
        }        
    }
}
