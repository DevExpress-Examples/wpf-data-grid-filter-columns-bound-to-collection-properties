<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1141282)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Data Grid for WPF - How to implement aggregate operators in Filter Drop-Down

DevExpress GirdControl does not support aggeragate operators in filter drop-down. This example illustartes how to implement aggeragate operators by substituting filters at runtime.

The filters are substituted using the [SubstituteFilter](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl.SubstituteFilter) event. To parse and patch CriteriaOperators a descendand of `ClientCriteriaLazyPatcherBase.AggregatesCommonProcessingBase` is used.

```cs
private void AssociatedObject_SubstituteFilter(object sender, DevExpress.Data.SubstituteFilterEventArgs e)
{
    InToAggregatePatcher.FieldName = CollectionColumnFieldName;
    InToAggregatePatcher.DataItemId = DataItemId;
    e.Filter = InToAggregatePatcher.Patch(e.Filter);
}

public class InToAggregatePatcher : ClientCriteriaLazyPatcherBase.AggregatesCommonProcessingBase {

    public static string FieldName;

    public static string DataItemId;

    public static CriteriaOperator Patch(CriteriaOperator source) {
        return new InToAggregatePatcher().Process(source);
    }

    public override CriteriaOperator Visit(InOperator theOperator) {
        var result = (InOperator)base.Visit(theOperator);
        var property = result.LeftOperand as OperandProperty;
        if (property?.PropertyName == FieldName && result.Operands.All(c => c is OperandValue)) {
            var items = result.Operands.Cast<OperandValue>().Select(c => c.Value);
            var ItemValues = items.Select(item => new OperandValue(item.GetType().GetProperty(DataItemId).GetValue(item)));
            var inOperator = new InOperator(new OperandProperty(DataItemId), ItemValues);
            var newOperator = CriteriaOperator.Parse(FieldName + "[" + inOperator.ToString() + "]");
            return newOperator;
        }

        return result;
    }
}
```

To correctly set up Filter Drop-Down for this usage scenario it is neccesary to pass filter items to it and change its EditSettings. To pass the filter items the [ShowFilterPopup](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.DataViewBase.ShowFilterPopup) is used. To change the EditSettings in Filter Drop-Down it is neccesary to override its theme template. The required template can be found in theme resources with the `dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=SearchControlContainerStyle` key.

```XAML
<Style x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=ValueColumnStyle, IsThemeIndependent=True}" TargetType="{x:Type dxg:TreeListColumn}">
    <Setter Property="Width" Value="*"/>
    <Setter Property="EditSettings" Value="{x:Null}"/> <!--Default value == {Binding ValueColumnEditSettings}-->
    <Setter Property="ColumnFilterMode" Value="DisplayText"/>
    <Setter Property="CellTemplateSelector">
        <Setter.Value>
            <dxg:ExcelColumnFilterCellTemplateSelector>
                <dxg:ExcelColumnFilterCellTemplateSelector.ServiceValueTemplate>
                    <DataTemplate>
                        <dxe:TextEdit EditMode="InplaceInactive" EditValue="{Binding RowData.Row.DisplayValue, Mode=OneWay}"/>
                    </DataTemplate>
                </dxg:ExcelColumnFilterCellTemplateSelector.ServiceValueTemplate>
            </dxg:ExcelColumnFilterCellTemplateSelector>
        </Setter.Value>
    </Setter>
</Style>
```

The above steps will allow Filter Drop-Down to create aggregate filters, but this will not allow it to process already existing aggregate filters. To implement this a custom a custom behavior is created that manually checks required filter items.

```cs
public class ExcelColumnFilterRestoreCheckedStateBehavior : Behavior<TreeListView> {

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
}
```

```XAML
<DataTemplate x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=TreeListBehaviorTemplate, IsThemeIndependent=True}">
    <ItemsControl>
        <dxg:ExcelColumnFilterListBehavior/>
        <dxg:ExcelColumnFilterMouseClickBehavior/>
        <behaviours:ExcelColumnFilterRestoreCheckedStateBehavior DataItemId="Value"/>
    </ItemsControl>
</DataTemplate>
```

With these setting the seraching functionality of Filter Drop-Down doesn't work. You can disable it as follows:
```XAML
<Style x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=SearchControlContainerStyle, IsThemeIndependent=True}" TargetType="{x:Type Grid}">
    <Setter Property="Visibility" Value="Collapsed"/>
</Style>
```

## Files to Review

* [Window1.xaml](./CS/FilterDropDown_AgregateOperators/MainWindow.xaml) (VB: [Window1.xaml](./VB/FilterDropDown_AgregateOperators/MainWindow.xaml))
* [InToAggregatePatcher.cs](./CS/FilterDropDown_AgregateOperators/Behaviours/InToAggregatePatcher.cs) (VB: [InToAggregatePatcher.vb](./VB/FilterDropDown_AgregateOperators/Behaviours/InToAggregatePatcher.vb))
* [FilterDropDownAggregateOperatorBehavior.cs](./CS/FilterDropDown_AgregateOperators/Behaviours/FilterDropDownAggregateOperatorBehavior.cs) (VB: [FilterDropDownAggregateOperatorBehavior.vb](./VB/FilterDropDown_AgregateOperators/Behaviours/FilterDropDownAggregateOperatorBehavior.vb))
* [ExcelColumnFilterRestoreCheckedStateBehavior.cs](./CS/FilterDropDown_AgregateOperators/Behaviours/ExcelColumnFilterRestoreCheckedStateBehavior.cs) (VB: [ExcelColumnFilterRestoreCheckedStateBehavior.vb](./VB/FilterDropDown_AgregateOperators/Behaviours/ExcelColumnFilterRestoreCheckedStateBehavior.vb))

## Documentation

- [How to traverse through and modify the CriteriaOperator instances](https://supportcenter.devexpress.com/internal/ticket/details/T320172)
- [Modify Theme Resources](https://docs.devexpress.com/WPF/403598/common-concepts/themes/customize-devexpress-theme-resources)
