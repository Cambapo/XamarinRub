namespace App2
{
    public class Student
    {
        public Evaluation[] evaluations { set; get; }
        public Classes[] classes { set; get; }
        public string name { set; get; }

        public Student(string name)
        {
            this.name = name;
        }
    }
}