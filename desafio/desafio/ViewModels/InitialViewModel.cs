using desafio.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace desafio.ViewModels
{
    public class InitialViewModel : BaseViewModel
    {
        public InitialViewModel()
        {
            Title = "Home";
        }
        public View OnAppearing()
        {
            IsBusy = true;
            var allBarbecue = ServiceBarbecue.GetItems().ToList();
            if (allBarbecue.Count == 0)
            {
                return new EmptyBarbecuesView();
            }
            return new EmptyBarbecuesView();
        }
    }
}
