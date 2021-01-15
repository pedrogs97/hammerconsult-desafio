using System;
using System.Collections.Generic;

namespace desafio.Models
{
    public class Barbecue
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Local { get; set; }
        public List<Person> Participants { get; set; }
        public List<Person> ParticipantsPaid { get; set; }
        public Person Creator { get; set; }
        public float TotalCollected { get; set; }
        public float TotalSpent { get; set; }
        public  float TotalDrink { get; set; }
        public float TotalFood { get; set; }
    }
}