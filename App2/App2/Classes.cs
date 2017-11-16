namespace App2
{
    public class Classes
    {
        public string subject { set; get; }
        public Student[] list { set; get; }
        public Evaluation[] evaluations { set; get; }

        public Classes(string subject)
        {
            this.subject = subject;
        }
    }
}