namespace App2
{
    public class Evaluation
    {

        public double grade { set; get; }
        public double weight { set; get; }
        public string description { set; get; }

        public Evaluation(string description, double weight)
        {
            this.description = description;
            this.weight = weight;
        }


    }
}