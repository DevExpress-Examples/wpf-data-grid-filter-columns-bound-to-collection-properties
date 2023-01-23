using DevExpress.Mvvm;
using System;

namespace FilterDropDown_AgregateOperators {
    public class ClassName : BindableBase {
        protected string _Name;
        public string Name {
            get { return this._Name; }
            set { this.SetProperty(ref this._Name, value, "Name"); }
        }

        protected int _Age;
        public int Age {
            get { return this._Age; }
            set { this.SetProperty(ref this._Age, value, "Age"); }
        }

        protected object _Genres;
        public object Genres {
            get { return this._Genres; }
            set { this.SetProperty(ref this._Genres, value, "Genres"); }
        }

        protected DateTime _DateTime;
        public DateTime DateTime {
            get { return this._DateTime; }
            set { this.SetProperty(ref this._DateTime, value, "DateTime"); }
        }

        protected bool _IsSelected;
        public bool IsSelected {
            get { return this._IsSelected; }
            set { this.SetProperty(ref this._IsSelected, value, "IsSelected"); }
        }
    }
}
