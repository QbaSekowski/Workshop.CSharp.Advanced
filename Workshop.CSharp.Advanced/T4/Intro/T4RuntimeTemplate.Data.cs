using System.Collections.Generic;
using System.Linq;

namespace Workshop.CSharp.Advanced
{
    public partial class T4RuntimeTemplate
    {

        public int CountOfPeople = 5;

        private IEnumerable<Person> ListOfPeople =>
            Enumerable.Range(1, CountOfPeople).Select(id => new Person { Id = id, Name = "John " + id });

        class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}