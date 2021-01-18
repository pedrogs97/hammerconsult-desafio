using desafio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio.Services
{
    public class PersonDataStore : IDataStore<Person>
    {
        readonly List<Person> people;
        public PersonDataStore()
        {
            people = new List<Person>()
            {
                new Person { Id = Guid.NewGuid().ToString(), Name = "João", Drink = true, InvitedBy = null },
                new Person { Id = Guid.NewGuid().ToString(), Name = "Marina", Drink = true, InvitedBy = null },
                new Person { Id = Guid.NewGuid().ToString(), Name = "Stephanie", Drink = true, InvitedBy = null },
                new Person { Id = Guid.NewGuid().ToString(), Name = "Paulo", Drink = true, InvitedBy = null },
                new Person { Id = Guid.NewGuid().ToString(), Name = "Ariel", Drink = true, InvitedBy = null }
            };
        }
        public bool AddItem(Person person)
        {
            try
            {
                people.Add(person);
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
                var oldPerson = people.Where((Person p) => p.Id == id).FirstOrDefault();
                people.Remove(oldPerson);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Person GetItem(string id)
        {
            try
            {
                return people.FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return new Person();
            }
        }

        public IEnumerable<Person> GetItems()
        {
            try
            {
                return people;
            }
            catch
            {
                return new List<Person>();
            }
        }

        public bool UpdateItem(Person person)
        {
            try
            {
                var oldPerson = people.Where((Person p) => p.Id == person.Id).FirstOrDefault();
                people.Remove(oldPerson);
                people.Add(person);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
