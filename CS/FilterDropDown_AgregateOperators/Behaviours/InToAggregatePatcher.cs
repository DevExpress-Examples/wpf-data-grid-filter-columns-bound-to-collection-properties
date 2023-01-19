using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using System.Linq;

namespace FilterDropDown_AgregateOperators.Behaviours {
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
}
