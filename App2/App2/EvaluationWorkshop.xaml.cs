using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EvaluationWorkshop : ContentPage
    {
        private Context context;
        List<Evaluation> evaluations = new List<Evaluation> { };
        FirebaseClient firebase;
        ObservableCollection<Evaluation> bdeval = new ObservableCollection<Evaluation>();
        public EvaluationWorkshop()
        {
            firebase = new FirebaseClient("https://i-dratherusemysql.firebaseio.com/");
            BindingContext = this;
            InitializeComponent();
        }

        private void initialise()
        {
            var layout = new StackLayout();
            var label = new Label { Text = "Evaluations" };

            ListView gradesList = new ListView
            {
                ItemsSource = bdeval,

                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label gradeLabel = new Label();
                    gradeLabel.SetBinding(Label.TextProperty, "weight");

                    Label descriptionLabel = new Label();
                    descriptionLabel.SetBinding(Label.TextProperty, "description");

                    Label completeLabel = new Label { Text = String.Concat(descriptionLabel.Text, ": ", gradeLabel.Text) };

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            completeLabel
                                        }
                                    }
                                }
                        }
                    };
                })
            };

            synclist();

            gradesList.ItemSelected += OnSelection;

            this.Content = layout;
            layout.Children.Add(label);
            layout.Children.Add(gradesList);


            var addEvalBttn = new Xamarin.Forms.Button
            {
                Text = "New evaluation",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            addEvalBttn.Clicked += addEval;

            layout.Children.Add(addEvalBttn);
        }

        async private void addEval(object sender, EventArgs e)
        {
            string info = await InputBox(this.Navigation);
            string[] nlnd = info.Split(',');
            Evaluation ne = new Evaluation(nlnd[0], Convert.ToDouble(nlnd[1]));
            await firebase.Child("evaluations").PostAsync(ne);
            initialise();
        }

        async private void synclist()
        {
            await getList();
        }

        public static Task<string> InputBox(INavigation navigation)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            var lblTitle = new Label { Text = "New Evaluation", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var lblName = new Label { Text = "Enter description:" };
            var descriptionInput = new Entry { Text = "" };
            var lblWeight = new Label { Text = "Enter weight:" };
            var weightInput = new Entry { Text = "" };

            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = String.Concat(descriptionInput.Text, ",", weightInput.Text);
                await navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, lblName, descriptionInput, lblWeight, weightInput, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            // txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }

        public async Task<int> getList()
        {
            var list = (await firebase.Child("evaluations").OnceAsync<Classes>());
            bdeval.Clear();
            foreach (var item in list)
            {
                Evaluation e = item.Object as Evaluation;
                e.description = item.Key;
                bdeval.Add(e);
            }

            return 0;
        }

        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            Page1 pgn = new Page1((Evaluation)e.SelectedItem);
            await Navigation.PushAsync(pgn);
            //e.SelectedItem = null;
            //((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
        }
    }
}