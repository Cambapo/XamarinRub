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
	public partial class StudentList : ContentPage
	{
        private Context context;
        List<Student> evaluations = new List<Student> { };
        FirebaseClient firebase;
        ObservableCollection<Student> bdstudents = new ObservableCollection<Student>();
        public StudentList()
		{
            firebase = new FirebaseClient("https://i-dratherusemysql.firebaseio.com/");
            BindingContext = this;
            InitializeComponent();
        }
        private void initialise()
        {
            var layout = new StackLayout();
            var label = new Label { Text = "Students" };

            ListView studentsList = new ListView
            {
                ItemsSource = bdstudents,

                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "name");

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
                                            nameLabel
                                        }
                                    }
                                }
                        }
                    };
                })
            };

            synclist();

            studentsList.ItemSelected += OnSelection;

            this.Content = layout;
            layout.Children.Add(label);
            layout.Children.Add(studentsList);


            var addStudentBttn = new Xamarin.Forms.Button
            {
                Text = "New student",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            addStudentBttn.Clicked += addStudent;

            layout.Children.Add(addStudentBttn);
        }

        async private void addStudent(object sender, EventArgs e)
        {
            string info = await InputBox(this.Navigation);
            Student ns = new Student(info);
            await firebase.Child("studentws").PostAsync(ns);
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

            var lblTitle = new Label { Text = "New Student", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var lblName = new Label { Text = "Enter name:" };
            var nameInput = new Entry { Text = "" };
            
            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = nameInput.Text;
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
                Children = { lblTitle, lblName, nameInput, slButtons },
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
            bdstudents.Clear();
            foreach (var item in list)
            {
                Student s = item.Object as Student;
                s.name = item.Key;
                bdstudents.Add(s);
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