using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Xamarin.Database;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Contexts;



namespace App2
{
	public partial class MainPage : ContentPage
	{

        private Context context;
        
        public Context Context { get => context; set => context = value; }

        public MainPage()
        {
            firebase = new FirebaseClient("https://i-dratherusemysql.firebaseio.com/");
            initialise();
            BindingContext = this;
            
        }

        private void initialise()
        {
            //InitializeComponent();
            var layout = new StackLayout();
            var label = new Label { Text = "Bienvenue" };

            
            this.Content = layout;
            layout.Children.Add(label);
            

            var classBttn = new Xamarin.Forms.Button
            {
                Text = "Go to classes",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            classBttn.Clicked += gotoClass;
            
            var studBttn = new Xamarin.Forms.Button
            {
                Text = "Students list",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            studBttn.Clicked += gotoStud;

            var evalBttn = new Xamarin.Forms.Button
            {
                Text = "Students list",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            evalBttn.Clicked += gotoEvals;

            layout.Children.Add(classBttn);

            layout.Children.Add(studBttn);

            layout.Children.Add(evalBttn);
        }

        async private void gotoClass(object sender, EventArgs e)
        {
            ClassesView pgn = new ClassesView();
            await Navigation.PushAsync(pgn);
        }

        async private void gotoStud(object sender, EventArgs e)
        {
            StudentList pgn = new StudentList();
            await Navigation.PushAsync(pgn);
        }
        async private void gotoEvals(object sender, EventArgs e)
        {
            EvaluationWorkshop pgn = new EvaluationWorkshop();
            await Navigation.PushAsync(pgn);
        }

    }
}