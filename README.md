<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/590928153/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1141282)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# Data Grid for WPF - Filter Columns Bound to Collection Properties

The **Genres** column contains elements of the `List<object>` type. This example assigns collection editors to the column's cells and allows users to filter [GridControl](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl) data against column values:

![image](https://user-images.githubusercontent.com/65009440/214008045-941bfb71-671e-445f-90fe-cf225dc764c0.png)


## Implementation Details


### Display and Edit Collection Values

The DevExpress [WPF Editors](https://docs.devexpress.com/WPF/6190/controls-and-libraries/data-editors) library includes editors that allow your users to edit collections. In this example, the [CheckedTokenComboBox](https://docs.devexpress.com/WPF/DevExpress.Xpf.Editors.CheckedTokenComboBoxStyleSettings) editor is assigned to the **Genres** column:

![image](https://user-images.githubusercontent.com/65009440/214015138-2aed7692-661a-4599-8e72-95185323c2a4.png)

```xaml
<dxg:GridColumn FieldName="Genres"
                FilterPopupMode="Excel">
    <dxg:GridColumn.EditSettings>
        <dxe:ComboBoxEditSettings ItemsSource="{Binding Genres}"
                                  DisplayMember="Name">
            <dxe:ComboBoxEditSettings.StyleSettings>
                <dxe:CheckedTokenComboBoxStyleSettings FilterOutSelectedTokens="False"/>
            </dxe:ComboBoxEditSettings.StyleSettings>
        </dxe:ComboBoxEditSettings>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

Set the column's [FilterPopupMode](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.ColumnBase.FilterPopupMode) property to `Excel` to use the customizable Excel-style drop-down filter.

Refer to the following help topic for more information: [Implement multi-select in DevExpress WPF Data Editors](https://supportcenter.devexpress.com/ticket/details/t889444/how-to-implement-multi-select-when-using-devexpress-wpf-data-editors-comboboxedit).


### Populate the Column's Drop-down Filter with Collection Values

Perform the following actions in the [DataViewBase.ShowFilterPopup](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.DataViewBase.ShowFilterPopup) event handler:

* Assign the column's `ItemsSource` collection to the [ExcelColumnFilterSettings.FilterItems](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.ExcelColumnFilterSettings.FilterItems) property. The `GridControl` displays this collection in the column's [Drop-down Filter](https://docs.devexpress.com/WPF/6133/controls-and-libraries/data-grid/filtering-and-searching/drop-down-filter).
* Set the [ExcelColumnFilterSettings.AllowedFilterTypes](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.ExcelColumnFilterSettings.AllowedFilterTypes) property to `FilterValues` to display only column values in the filter popup:

```xaml
<dxg:GridControl ...>
    <dxmvvm:Interaction.Behaviors>
        <behaviors:FilterDropDownAggregateOperatorBehavior
            ColumnItemsSource="{Binding Genres}"/>
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


### Implement Filter Operations

Use the [GridControl.SubstituteFilter](https://docs.devexpress.com/WPF/DevExpress.Xpf.Grid.GridControl.SubstituteFilter) event to update the grid filter:

```xaml
<dxg:GridControl ...>
    <dxmvvm:Interaction.Behaviors>
        <behaviors:FilterDropDownAggregateOperatorBehavior 
            CollectionColumnFieldName="Genres" 
            DataItemId="Value" 
            ColumnItemsSource="{Binding Genres}"/>
    </dxmvvm:Interaction.Behaviors>
</dxg:GridControl>
```

```cs
internal class FilterDropDownAggregateOperatorBehavior : Behavior<GridControl> {
    // ...
    private void AssociatedObject_SubstituteFilter(object sender, DevExpress.Data.SubstituteFilterEventArgs e) {
        InToAggregatePatcher.FieldName = CollectionColumnFieldName;
        InToAggregatePatcher.DataItemId = DataItemId;
        e.Filter = InToAggregatePatcher.Patch(e.Filter);
    }
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

Refer to the following help topic for more information: [Traverse through and modify the CriteriaOperator instances](https://supportcenter.devexpress.com/ticket/details/t320172/how-to-traverse-through-and-modify-the-criteriaoperator-instances).


### Customize the Drop-down Filter

* The `SearchControlContainerStyle.Visibility` property allows you to hide the search box from the popup.
* The `ServiceValueTemplate` property is used to specify the text displayed in the **Select All** checkbox.

![image](https://user-images.githubusercontent.com/65009440/214020374-6fdf8d6a-41a5-4bc2-a9bb-3daf4e8ae1ea.png)

```xaml
<Style x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=SearchControlContainerStyle, IsThemeIndependent=True}" 
       TargetType="{x:Type Grid}">
    <Setter Property="Visibility" Value="Collapsed"/>
</Style>
<Style x:Key="{dxgt:ExcelColumnFilterPopupThemeKey ResourceKey=ValueColumnStyle, IsThemeIndependent=True}" 
       TargetType="{x:Type dxg:TreeListColumn}">
    <Setter Property="CellTemplateSelector">
        <Setter.Value>
            <dxg:ExcelColumnFilterCellTemplateSelector>
                <dxg:ExcelColumnFilterCellTemplateSelector.ServiceValueTemplate>
                    <DataTemplate>
                        <dxe:TextEdit EditValue="{Binding RowData.Row.DisplayValue, Mode=OneWay}"
                                      ShowBorder="False"/>
                    </DataTemplate>
                </dxg:ExcelColumnFilterCellTemplateSelector.ServiceValueTemplate>
            </dxg:ExcelColumnFilterCellTemplateSelector>
        </Setter.Value>
    </Setter>
</Style>
```

Refer to the following help topic for more information: [Modify Theme Resources](https://docs.devexpress.com/WPF/403598/common-concepts/themes/customize-devexpress-theme-resources).


## Files to Review

* [MainWindow.xaml](./CS/FilterDropDown_AgregateOperators/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/FilterDropDown_AgregateOperators/MainWindow.xaml))
* [InToAggregatePatcher.cs](./CS/FilterDropDown_AgregateOperators/Behaviors/InToAggregatePatcher.cs) (VB: [InToAggregatePatcher.vb](./VB/FilterDropDown_AgregateOperators/Behaviors/InToAggregatePatcher.vb))
* [FilterDropDownAggregateOperatorBehavior.cs](./CS/FilterDropDown_AgregateOperators/Behaviors/FilterDropDownAggregateOperatorBehavior.cs) (VB: [FilterDropDownAggregateOperatorBehavior.vb](./VB/FilterDropDown_AgregateOperators/Behaviors/FilterDropDownAggregateOperatorBehavior.vb))


## Documentation

- [How to traverse through and modify the CriteriaOperator instances](https://supportcenter.devexpress.com/ticket/details/t320172/how-to-traverse-through-and-modify-the-criteriaoperator-instances)
- [Modify Theme Resources](https://docs.devexpress.com/WPF/403598/common-concepts/themes/customize-devexpress-theme-resources)
- [Implement multi-select in DevExpress WPF Data Editors](https://supportcenter.devexpress.com/ticket/details/t889444/how-to-implement-multi-select-when-using-devexpress-wpf-data-editors-comboboxedit)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-data-grid-filter-columns-bound-to-collection-properties&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-data-grid-filter-columns-bound-to-collection-properties&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
