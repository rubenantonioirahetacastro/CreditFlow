using CreditFlow.Web.Core.UI.Components.StatusColor;

namespace CreditFlow.Web.Shared.Credit.Presentation;

public static class CreditStatusPalette
{
    private static readonly CdsStatusAppearance Default =
        new("var(--cds-status-default-content)", "var(--cds-status-default-container)");

    private static readonly IReadOnlyDictionary<int, CdsStatusAppearance> ByStatusId =
        new Dictionary<int, CdsStatusAppearance>
        {
            [1] = new("var(--cds-status-requested-content)", "var(--cds-status-requested-container)"),
            [2] = new("var(--cds-status-analysis-content)", "var(--cds-status-analysis-container)"),
            [3] = new("var(--cds-status-approved-content)", "var(--cds-status-approved-container)"),
            [4] = new("var(--cds-status-disbursed-content)", "var(--cds-status-disbursed-container)"),
            [5] = new("var(--cds-status-verified-content)", "var(--cds-status-verified-container)"),
            [30] = new("var(--cds-status-current-content)", "var(--cds-status-current-container)"),
            [50] = new("var(--cds-status-cancelled-content)", "var(--cds-status-cancelled-container)")
        };

    public static CdsStatusAppearance Resolve(int statusId) =>
        ByStatusId.GetValueOrDefault(statusId, Default);
}
