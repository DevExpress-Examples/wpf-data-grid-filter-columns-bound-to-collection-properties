using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilterDropDown_AgregateOperators.Behaviours
{
    internal class FilterDropDownAggregateOperatorBehavior : Behavior<GridControl>
    {


        public string CollectionColumnFieldName
        {
            get { return (string)GetValue(CollectionColumnFieldNameProperty); }
            set { SetValue(CollectionColumnFieldNameProperty, value); }
        }

        
        public static readonly DependencyProperty CollectionColumnFieldNameProperty =
            DependencyProperty.Register("CollectionColumnFieldName", typeof(string), typeof(FilterDropDownAggregateOperatorBehavior), new PropertyMetadata(String.Empty));

        public string DataItemId
        {
            get { return (string)GetValue(DataItemIdProperty); }
            set { SetValue(DataItemIdProperty, value); }
        }

        
        public static readonly DependencyProperty DataItemIdProperty =
            DependencyProperty.Register("DataItemId", typeof(string), typeof(FilterDropDownAggregateOperatorBehavior), new PropertyMetadata(String.Empty));


        public IList<Object> ColumnItemsSource
        {
            get { return (IList<Object>)GetValue(ColumnItemsSourceProperty); }
            set { SetValue(ColumnItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ColumnItemsSourceProperty =
            DependencyProperty.Register("ColumnItemsSource", typeof(IList<Object>), typeof(FilterDropDownAggregateOperatorBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;

        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.SubstituteFilter += AssociatedObject_SubstituteFilter;
            AssociatedObject.View.ShowFilterPopup += View_ShowFilterPopup;
        }

        private void View_ShowFilterPopup(object sender, FilterPopupEventArgs e)
        {
            if (e.Column.FieldName == CollectionColumnFieldName)
            {
                e.ExcelColumnFilterSettings.AllowedFilterTypes = DevExpress.Xpf.Grid.ExcelColumnFilterType.FilterValues;
                e.ExcelColumnFilterSettings.FilterItems = ColumnItemsSource;
            }
        }


        private void AssociatedObject_SubstituteFilter(object sender, DevExpress.Data.SubstituteFilterEventArgs e)
        {
            InToAggregatePatcher.FieldName = CollectionColumnFieldName;
            InToAggregatePatcher.DataItemId = DataItemId;
            e.Filter = InToAggregatePatcher.Patch(e.Filter);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SubstituteFilter -= AssociatedObject_SubstituteFilter;
            AssociatedObject.View.ShowFilterPopup -= View_ShowFilterPopup;
            base.OnDetaching();
        }


    }
}
