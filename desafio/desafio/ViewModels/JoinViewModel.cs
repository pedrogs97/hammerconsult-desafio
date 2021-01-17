using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class JoinViewModel : BaseViewModel
    {
        private string _codeBarbecue;
        private Page Page { get; }
        public Command CloseModalCommand { get; }
        public Command JoinCommand { get; }
        public string CodeBarbecue
        {
            get => _codeBarbecue;
            set => SetProperty(ref _codeBarbecue, value);
        }
        public JoinViewModel(Page page)
        {
            Page = page;
            CloseModalCommand = new Command(CloseModal);
            JoinCommand = new Command(Join);
        }

        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }
        private async void Join()
        {
            var user = ServicePerson.GetItem(App.Current.Properties["user"].ToString());
            var barbecue = ServiceBarbecue.GetItem(CodeBarbecue);
            if (barbecue.Participants.Contains(user))
            {
                await Page.DisplayAlert("", "Você já está participando desse churrasco!", "OK");
                return;
            }
            barbecue.Participants.Add(user);
            ServiceBarbecue.UpdateItem(barbecue);
            await Page.Navigation.PopModalAsync();
        }
    }
}
