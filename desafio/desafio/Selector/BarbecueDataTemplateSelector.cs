using desafio.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace desafio.Selector
{
    public class BarbecueDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Expired { get; set; }
        public DataTemplate NotExpired { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((item as Barbecue).Date <= DateTime.Now) ? NotExpired : Expired;
        }
    }
}
