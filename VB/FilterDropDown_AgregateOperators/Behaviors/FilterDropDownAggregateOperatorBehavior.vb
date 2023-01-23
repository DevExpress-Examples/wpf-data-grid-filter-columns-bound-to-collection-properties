Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Grid
Imports System.Collections.Generic
Imports System.Windows

Namespace FilterDropDown_AgregateOperators.Behaviors

    Friend Class FilterDropDownAggregateOperatorBehavior
        Inherits Behavior(Of GridControl)

        Public Property CollectionColumnFieldName As String
            Get
                Return CStr(GetValue(CollectionColumnFieldNameProperty))
            End Get

            Set(ByVal value As String)
                SetValue(CollectionColumnFieldNameProperty, value)
            End Set
        End Property

        Public Shared ReadOnly CollectionColumnFieldNameProperty As DependencyProperty = DependencyProperty.Register("CollectionColumnFieldName", GetType(String), GetType(FilterDropDownAggregateOperatorBehavior), New PropertyMetadata(String.Empty))

        Public Property DataItemId As String
            Get
                Return CStr(GetValue(DataItemIdProperty))
            End Get

            Set(ByVal value As String)
                SetValue(DataItemIdProperty, value)
            End Set
        End Property

        Public Shared ReadOnly DataItemIdProperty As DependencyProperty = DependencyProperty.Register("DataItemId", GetType(String), GetType(FilterDropDownAggregateOperatorBehavior), New PropertyMetadata(String.Empty))

        Public Property ColumnItemsSource As IList(Of Object)
            Get
                Return CType(GetValue(ColumnItemsSourceProperty), IList(Of Object))
            End Get

            Set(ByVal value As IList(Of Object))
                SetValue(ColumnItemsSourceProperty, value)
            End Set
        End Property

        Public Shared ReadOnly ColumnItemsSourceProperty As DependencyProperty = DependencyProperty.Register("ColumnItemsSource", GetType(IList(Of Object)), GetType(FilterDropDownAggregateOperatorBehavior), New PropertyMetadata(CType(Nothing, PropertyChangedCallback)))

        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            AddHandler AssociatedObject.Loaded, AddressOf AssociatedObject_Loaded
        End Sub

        Private Sub AssociatedObject_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            RemoveHandler AssociatedObject.Loaded, AddressOf AssociatedObject_Loaded
            AddHandler AssociatedObject.SubstituteFilter, AddressOf AssociatedObject_SubstituteFilter
            AddHandler AssociatedObject.View.ShowFilterPopup, AddressOf View_ShowFilterPopup
        End Sub

        Private Sub View_ShowFilterPopup(ByVal sender As Object, ByVal e As FilterPopupEventArgs)
            If Equals(e.Column.FieldName, CollectionColumnFieldName) Then
                e.ExcelColumnFilterSettings.AllowedFilterTypes = ExcelColumnFilterType.FilterValues
                e.ExcelColumnFilterSettings.FilterItems = ColumnItemsSource
            End If
        End Sub

        Private Sub AssociatedObject_SubstituteFilter(ByVal sender As Object, ByVal e As DevExpress.Data.SubstituteFilterEventArgs)
            InToAggregatePatcher.FieldName = CollectionColumnFieldName
            InToAggregatePatcher.DataItemId = DataItemId
            e.Filter = InToAggregatePatcher.Patch(e.Filter)
        End Sub

        Protected Overrides Sub OnDetaching()
            RemoveHandler AssociatedObject.SubstituteFilter, AddressOf AssociatedObject_SubstituteFilter
            RemoveHandler AssociatedObject.View.ShowFilterPopup, AddressOf View_ShowFilterPopup
            MyBase.OnDetaching()
        End Sub
    End Class
End Namespace
