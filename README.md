<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/590928153/22.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1141282)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Data Grid for WPF - Filter Columns of the Collection Type

This example allows users to filter [GridControl](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl) data against columns bound to a collection.

![image](https://user-images.githubusercontent.com/65009440/214008045-941bfb71-671e-445f-90fe-cf225dc764c0.png)


## Implementation Details


### Display and Edit Collection Values

The DevExpress [WPF Editors](https://docs.devexpress.com/WPF/6190/controls-and-libraries/data-editors) library includes editors that allow your users to edit collections. In this example, the [CheckedTokenComboBox](https://docs.devexpress.com/WPF/DevExpress.Xpf.Editors.CheckedTokenComboBoxStyleSettings) editor is assigned to the **Genres** column:

![image](https://user-images.githubusercontent.com/65009440/214015138-2aed7692-661a-4599-8e72-95185323c2a4.png)

```xaml
<dxg:GridColumn FieldName="Genres"
                FilterPopupMode="Excel"
                ImmediateUpdateColumnFilter="True">
    <dxg:GridColumn.EditSettings>
        <dxe:ComboBoxEditSettings ItemsSource="{Binding Genres}"
                                  DisplayMember="Name">
            <dxe:ComboBoxEditSettings.StyleSettings>
                <dxe:CheckedTokenComboBoxStyleSettings FilterOutSelectedTokens="False" />
            </dxe:ComboBoxEditSettings.StyleSettings>
        </dxe:ComboBoxEditSettings>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```


### Populate the Column's Drop-down Filter with Collection Values

The [DataViewBase.ShowFilterPopup](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.DataViewBase.ShowFilterPopup) event handler passes the column's collection values to the [Drop-down Filter](https://docs.devexpress.com/WPF/6133/controls-and-libraries/data-grid/filtering-and-searching/drop-down-filter):

```xaml
<dxg:GridControl ...>
    <dxmvvm:Interaction.Behaviors>
        <behaviors:FilterDropDownAggregateOperatorBehavior 
            CollectionColumnFieldName="Genres" 
            DataItemId="Value" 
            ColumnItemsSource="{Binding Genres, Converter={local:ListObjectConverter}}"/>
    </dxmvvm:Interaction.Behaviors>
</dxg:GridControl>
```

```cs
internal class FilterDropDownAggregateOperatorBehavior : Behavior<GridControl> {
    // ...
    private void View_ShowFilterPopup(object sender, FilterPopupEventArgs e) {
        if (e.Column.FieldName == CollectionColumnFieldName) {
            e.ExcelColumnFilterSettings.AllowedFilterTypes = DevExpress.Xpf.Grid.ExcelColumnFilterType.FilterValues;
            e.ExcelColumnFilterSettings.FilterItems = ColumnItemsSource;
        }
    }
}
```


### Customize the Drop-down Filter

Remove unsupported functionalities and set up the column's **Drop-down Filter** according to this implementation:

![image](https://user-images.githubusercontent.com/65009440/214020374-6fdf8d6a-41a5-4bc2-a9bb-3daf4e8ae1ea.png)

```xaml
<Style x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=SearchControlContainerStyle, IsThemeIndependent=True}" TargetType="{x:Type Grid}">
    <Setter Property="Visibility" Value="Collapsed"/>
</Style>

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


### Implement Filter Operations

Use the [GridControl.SubstituteFilter](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl.SubstituteFilter) event to update the grid filter:

```cs
private void AssociatedObject_SubstituteFilter(object sender, DevExpress.Data.SubstituteFilterEventArgs e) {
    InToAggregatePatcher.FieldName = CollectionColumnFieldName;
    InToAggregatePatcher.DataItemId = DataItemId;
    e.Filter = InToAggregatePatcher.Patch(e.Filter);
}
```

The `ClientCriteriaLazyPatcherBase.AggregatesCommonProcessingBase` descendant creates a new `In` filter operator. This operator allows users to filter the grid by the column's collection values:

```cs
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


### Process Existing Filters

To apply filters created outside of the column's **Drop-down Filter** (for example, created in the [Filter Editor](https://docs.devexpress.com/WPF/7788/controls-and-libraries/data-grid/filtering-and-searching/filter-editor) or code), create a behavior that parses this filter and checks items in the **Drop-down Filter** accordingly:

```xaml
<DataTemplate x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=TreeListBehaviorTemplate, IsThemeIndependent=True}">
    <ItemsControl>
        <dxg:ExcelColumnFilterListBehavior/>
        <dxg:ExcelColumnFilterMouseClickBehavior/>
        <behaviors:ExcelColumnFilterRestoreCheckedStateBehavior DataItemId="Value"/>
    </ItemsControl>
</DataTemplate>
```

```cs
public class ExcelColumnFilterRestoreCheckedStateBehavior : Behavior<TreeListView> {
    // ...
    void UpdateSelectionState(ExcelColumnFilterValuesListBase items, CriteriaOperator filterCriteria) {
        var aggregateOp = filterCriteria as AggregateOperand;
        if (ReferenceEquals(aggregateOp, null))
            return;
        var op = aggregateOp.Condition as InOperator;
        if (ReferenceEquals(op, null))
            return;
        var values = op.Operands.OfType<OperandValue>().Select(x => x.Value).ToList();
        foreach (var item in items) {
            if (item.EditValue != null) {
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


## Files to Review

* [MainWindow.xaml](./CS/FilterDropDown_AgregateOperators/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/FilterDropDown_AgregateOperators/MainWindow.xaml))
* [InToAggregatePatcher.cs](./CS/FilterDropDown_AgregateOperators/Behaviours/InToAggregatePatcher.cs) (VB: [InToAggregatePatcher.vb](./VB/FilterDropDown_AgregateOperators/Behaviours/InToAggregatePatcher.vb))
* [FilterDropDownAggregateOperatorBehavior.cs](./CS/FilterDropDown_AgregateOperators/Behaviours/FilterDropDownAggregateOperatorBehavior.cs) (VB: [FilterDropDownAggregateOperatorBehavior.vb](./VB/FilterDropDown_AgregateOperators/Behaviours/FilterDropDownAggregateOperatorBehavior.vb))
* [ExcelColumnFilterRestoreCheckedStateBehavior.cs](./CS/FilterDropDown_AgregateOperators/Behaviours/ExcelColumnFilterRestoreCheckedStateBehavior.cs) (VB: [ExcelColumnFilterRestoreCheckedStateBehavior.vb](./VB/FilterDropDown_AgregateOperators/Behaviours/ExcelColumnFilterRestoreCheckedStateBehavior.vb))


## Documentation

- [How to traverse through and modify the CriteriaOperator instances](https://supportcenter.devexpress.com/ticket/details/t320172/how-to-traverse-through-and-modify-the-criteriaoperator-instances)
- [Modify Theme Resources](https://docs.devexpress.com/WPF/403598/common-concepts/themes/customize-devexpress-theme-resources)

