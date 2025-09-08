using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Components;
public partial class DateBadge
{
    [Parameter] public DateTime Start { get; set; }
    [Parameter] public DateTime End { get; set; }

    private string? badgeText;
    private string? badgeColor;

    protected override void OnInitialized()
    {
        var now = DateTime.Now;

        if (End < now)
        {
            badgeText = "Paserat";
            badgeColor = "neutral";
        }
        else if (Start < now && End > now)
        {
            badgeText = "Aktiv";
            badgeColor = "success";
        }
        else
        {
            var timeUntilStart = Start - now;
            var daysUntilStart = (int)Math.Ceiling(timeUntilStart.TotalDays);

            if (daysUntilStart <= 30)
            {
                badgeText = daysUntilStart == 0 ? "Startar idag" : $"Startar om {daysUntilStart} dagar";
                badgeColor = "warning";
            }
            else
            {
                var monthsUntilStart = (int)Math.Floor(daysUntilStart / 30.0);
                badgeText = monthsUntilStart == 1 ? "Startar nästa månad" : $"Startar om {monthsUntilStart} månader";
                badgeColor = "info";
            }
        }
    }
}