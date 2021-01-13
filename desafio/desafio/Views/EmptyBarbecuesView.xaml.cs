using desafio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace desafio.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyBarbecuesView : ContentView
    {
        public EmptyBarbecuesView()
        {
            InitializeComponent();
            BindingContext = new EmptyBarbecueViewModel(Navigation);
        }
    }
}