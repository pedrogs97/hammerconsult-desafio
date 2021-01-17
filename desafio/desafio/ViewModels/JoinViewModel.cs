using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class JoinViewModel : BaseViewModel
    {
        private Page Page;
        public Command CloseModalCommand { get; }

        public JoinViewModel(Page page)
        {
            Page = page;
            CloseModalCommand = new Command(CloseModal);
        }

        private async void CloseModal()
        {
            await Page.Navigation.PopModalAsync();
        }
    }
}
