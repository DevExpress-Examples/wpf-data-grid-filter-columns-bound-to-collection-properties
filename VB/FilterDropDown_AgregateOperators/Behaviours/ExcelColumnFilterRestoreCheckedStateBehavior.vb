Imports DevExpress.Data.Filtering
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Grid
Imports System.ComponentModel
Imports System.Linq
Imports System.Windows

Namespace FilterDropDown_AgregateOperators.Behaviours

    Public Class ExcelColumnFilterRestoreCheckedStateBehavior
        Inherits Behavior(Of TreeListView)

        Public Property DataItemId As String
            Get
                Return CStr(GetValue(DataItemIdProperty))
            End Get

            Set(ByVal value As String)
                SetValue(DataItemIdProperty, value)
            End Set
        End Property

        Public Shared ReadOnly DataItemIdProperty As DependencyProperty = DependencyProperty.Register("DataItemId", GetType(String), GetType(ExcelColumnFilterRestoreCheckedStateBehavior), New PropertyMetadata(String.Empty))

        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            AddHandler AssociatedObject.Loaded, AddressOf AssociatedObject_Loaded
        End Sub

        Private Sub AssociatedObject_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            RemoveHandler AssociatedObject.Loaded, AddressOf AssociatedObject_Loaded
            Dim info = TryCast(AssociatedObject.DataContext, ExcelColumnFilterInfo)
            If info Is Nothing Then Return
            UpdateSelectionState(info.FilterItems, info.Column.View.DataControl.FilterCriteria)
        End Sub

        Private Sub UpdateSelectionState(ByVal items As ExcelColumnFilterValuesListBase, ByVal filterCriteria As CriteriaOperator)
            Dim aggregateOp = TryCast(filterCriteria, AggregateOperand)
            If ReferenceEquals(aggregateOp, Nothing) Then Return
            Dim op = TryCast(aggregateOp.Condition, InOperator)
            If ReferenceEquals(op, Nothing) Then Return
            Dim values = op.Operands.OfType(Of OperandValue)().[Select](Function(x) x.Value).ToList()
            For Each item In items
                If item.EditValue IsNot Nothing Then
                    Dim value = item.EditValue.GetType().GetProperty(DataItemId).GetValue(item.EditValue)
                    If value Is Nothing Then Continue For
                    If values.Contains(value) Then item.IsChecked = True
                End If
            Next
        End Sub

        Protected Overrides Sub OnDetaching()
            RemoveHandler AssociatedObject.Loaded, AddressOf AssociatedObject_Loaded
            MyBase.OnDetaching()
        End Sub
    End Class
End Namespace
