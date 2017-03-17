using PlaceStatusOnRaspberry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PlaceStatusOnRaspberry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            PeopleInside = (int)(restService.GetTotalPeopleInsideByPlaceId(selectedPlaceId) ?? 0);
        }

        private IIndoorPlaceInformationAPI2 restService;
        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        private int peopleInside = 0;
        public int PeopleInside {
            get => peopleInside;
            set {
                TextBlockTotalPeopleInside.Text = value.ToString();
                peopleInside = value;
            }
        }

        private Guid selectedPlaceId;

        public List<Place> Places;
        
        public Place SelectedPlace
        {
            set
            {
                PeopleInside = (int)(value.TotalPeopleInside ?? 0);
                selectedPlaceId = value.PlaceId ?? Guid.Empty;
            }
        }        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            restService = new IndoorPlaceInformationAPI2();
            
            Places = restService.GetAllPlaces().ToList();
                                    
            Places.ForEach(p => ListBoxPlaces.Items.Add(p));

            ListBoxPlaces.SelectedIndex = 0;

            if (selectedPlaceId != null)
            {
                timer.Start();
            }
        }

        private void ListBoxPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPlace = (Place)e.AddedItems.FirstOrDefault();
        }
    }
}
