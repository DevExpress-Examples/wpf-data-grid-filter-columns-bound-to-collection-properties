using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterDropDown_AgregateOperators.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel() 
        {
            Genres = GridData.GetGenres();
            Items = GridData.GetDataItems(Genres);
        }


        protected ObservableCollection<ClassName> _Items;
        public ObservableCollection<ClassName> Items
        {
            get { return this._Items; }
            set { this.SetProperty(ref this._Items, value, "Items"); }
        }

        protected ObservableCollection<Genre> _Genres;
        public ObservableCollection<Genre> Genres
        {
            get { return this._Genres; }
            set { this.SetProperty(ref this._Genres, value, "Genres"); }
        }

    }

    public class GridData
    {
        public static ObservableCollection<Genre> GetGenres()
        {
            return new ObservableCollection<Genre>(Enumerable.Range(0, 10).Select(c => new Genre() { Value = c, Name = "Genre #" + c }));
        }
        public static ObservableCollection<ClassName> GetDataItems(IList<Genre> genres)
        {
            ObservableCollection<ClassName> data = new ObservableCollection<ClassName>();
            Random r = new Random();
            int i = -1;
            while (++i < 150)
            {
                ClassName value = new ClassName();
                value.Name = string.Format("Name #{0}", r.Next(100));
                value.Age = r.Next(40) + 20;
                var index1 = r.Next(1, genres.Count);
                var index2 = r.Next(0, index1);
                value.Genres = new List<object>() { genres[index1], genres[index2] };
                value.DateTime = DateTime.Today.AddDays(r.Next(30) - 15);
                value.IsSelected = r.Next(2) > 0;
                data.Add(value);
            }
            return data;
        }
    }
}
