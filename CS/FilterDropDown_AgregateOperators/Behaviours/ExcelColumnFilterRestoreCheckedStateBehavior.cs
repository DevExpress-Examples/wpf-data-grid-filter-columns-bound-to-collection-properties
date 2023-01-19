using DevExpress.Data.Filtering;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilterDropDown_AgregateOperators.Behaviours
{
    public class ExcelColumnFilterRestoreCheckedStateBehavior : Behavior<TreeListView> {

        public string DataItemId
        {
            get { return (string)GetValue(DataItemIdProperty); }
            set { SetValue(DataItemIdProperty, value); }
        }

        public static readonly DependencyProperty DataItemIdProperty =
            DependencyProperty.Register("DataItemId", typeof(string), typeof(ExcelColumnFilterRestoreCheckedStateBehavior), new PropertyMetadata(String.Empty));

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }
        void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            var info = AssociatedObject.DataContext as ExcelColumnFilterInfo;
            if (info == null)
                return;
            UpdateSelectionState(info.FilterItems, info.Column.View.DataControl.FilterCriteria);
        }
        void UpdateSelectionState(ExcelColumnFilterValuesListBase items, CriteriaOperator filterCriteria) {
            var aggregateOp = filterCriteria as AggregateOperand;
            if (ReferenceEquals(aggregateOp, null))
                return;
            var op = aggregateOp.Condition as InOperator;
            if (ReferenceEquals(op, null))
                return;
            var values = op.Operands.OfType<OperandValue>().Select(x => x.Value).ToList();
            foreach (var item in items) {
                if (item.EditValue != null)
                {
                    var value = item.EditValue.GetType().GetProperty(DataItemId).GetValue(item.EditValue);
                    if (value == null)
                        continue;
                    if (values.Contains(value))
                        item.IsChecked = true;
                }
            }
        }
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetaching();
        }
    }
}
