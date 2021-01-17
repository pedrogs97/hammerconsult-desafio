using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using desafio.Models;

namespace desafio.Services
{
    public class BarbecueDataStore : IDataStore<Barbecue>
    {
        private readonly List<Barbecue> barbecues;

        public BarbecueDataStore()
        {
            barbecues = new List<Barbecue>();
        }
        public bool AddItem(Barbecue barbecue)
        {
            try
            {
                barbecues.Add(barbecue);
                return true;
            }
            catch
            {
                return false;
            }
        } 

        public bool DeleteItem(string id)
        {
            try
            {
                var oldItem = barbecues.Where((Barbecue b) => b.Id == id).FirstOrDefault();
                barbecues.Remove(oldItem);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Barbecue GetItem(string id)
        {
            try
            {
                return barbecues.FirstOrDefault(b => b.Id == id);
            }
            catch
            {
                return new Barbecue();
            }
        }

        public IEnumerable<Barbecue> GetItems()
        {
            try
            {
                return barbecues;
            }
            catch
            {
                return new List<Barbecue>();
            }
        }

        public bool UpdateItem(Barbecue barbecue)
        {
            try
            {
                var oldBarbecue = barbecues.Where((Barbecue b) => b.Id == barbecue.Id).FirstOrDefault();
                barbecues.Remove(oldBarbecue);
                barbecues.Add(barbecue);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}