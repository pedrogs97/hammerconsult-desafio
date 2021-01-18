using System;
using System.Collections.Generic;
using System.Text;

namespace desafio.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Drink { get; set; }
        public Person InvitedBy { get; set; }
    }
}
