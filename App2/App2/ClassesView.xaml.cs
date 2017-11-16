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
	public partial class ClassesView : ContentPage
	{
        private Context context;
        List<Classes> classes = new List<Classes> { };
        FirebaseClient firebase;
        ObservableCollection<Classes> bdclasses = new ObservableCollection<Classes>();
        public ClassesView ()
		{
            firebase = new FirebaseClient("https://i-dratherusemysql.firebaseio.com/");
            BindingContext = this;
            InitializeComponent();
        }

        private void initialise()
        {
            var layout = new StackLayout();
            var label = new Label { Text = "Classes" };

        ListView classesList = new ListView
        {
            ItemsSource = bdclasses,

            ItemTemplate = new DataTemplate(() =>
            {
                // Create views with bindings for displaying each property.
                Label subjectLabel = new Label();
                subjectLabel.SetBinding(Label.TextProperty, "Subject");

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
                                            subjectLabel
                                        }
                                    }
                                }
                    }
                };
            })
        };

            synclist();

            classesList.ItemSelected += OnSelection;

            this.Content = layout;
            layout.Children.Add(label);
            layout.Children.Add(classesList);
            

            var addClassBttn = new Xamarin.Forms.Button
            {
                Text = "New class",
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            addClassBttn.Clicked += addClass;
            
            layout.Children.Add(addClassBttn);
        }

    async private void addClass(object sender, EventArgs e)
    {
        string subject = await InputBox(this.Navigation);
        Classes nc = new Classes(subject);
        await firebase.Child("classes").PostAsync(nc);
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

        var lblTitle = new Label { Text = "New Class", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
        var lblName = new Label { Text = "Enter subject:" };
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
        var list = (await firebase.Child("classes").OnceAsync<Classes>());
        bdclasses.Clear();
        foreach (var item in list)
        {
            Classes c = item.Object as Classes;
            c.subject = item.Key;
            bdclasses.Add(c);
        }

        return 0;
    }

    async void OnSelection(object sender, SelectedItemChangedEventArgs e)
    {
        Page1 pgn = new Page1((Classes)e.SelectedItem);
        await Navigation.PushAsync(pgn);
        //e.SelectedItem = null;
        //((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
    }
    }
}