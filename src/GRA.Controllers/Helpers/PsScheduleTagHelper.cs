using System;
using System.Collections.Generic;
using System.Linq;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(Attributes = "asp-schedule")]
    public class PsScheduleTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-schedule")]
        public PsPerformerSchedule Schedule { get; set; }

        [HtmlAttributeName("asp-booked")]
        public DateTime BookedDate { get; set; }

        [HtmlAttributeName("asp-booking")]
        public bool Booking { get; set; }

        [HtmlAttributeName("asp-currentDate")]
        public bool CurrentDate { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var existingClasses = output.Attributes
                    .FirstOrDefault(_ => _.Name == "class");
            string availability = "";
            if (CurrentDate)
            {
                availability = nameof(PsScheduleDateStatus.Current);
            }
            else if (Schedule == null && (!Booking || BookedDate == default(DateTime)))
            {
                availability = nameof(PsScheduleDateStatus.Available);
            }
            else if (Schedule?.StartTime.HasValue == true || (Booking && BookedDate != default(DateTime)))
            {
                availability = nameof(PsScheduleDateStatus.Time);
                if (Schedule?.StartTime.HasValue == true)
                {
                    var timeString = JsonConvert.SerializeObject(new List<string>
                    {
                        Schedule.StartTime.Value.ToString("hh:mm tt"),
                        Schedule.EndTime.Value.ToString("hh:mm tt")
                    });
                    output.Attributes.Add("data-time", timeString);
                }
            }
            else if (Schedule?.StartTime.HasValue == false)
            {
                availability = nameof(PsScheduleDateStatus.Unavailable);
            }

            if (!Booking)
            {
                output.Attributes.Add("data-availability", availability);
            }
            if (!existingClasses.Value.ToString().Contains("unselectable")
                || (Booking && availability == nameof(PsScheduleDateStatus.Unavailable)))
            {
                output.Attributes.Remove(existingClasses);
                var attribute = new TagHelperAttribute("class",
                    $"{existingClasses.Value} {availability.ToLower()}");

                output.Attributes.Add(attribute);
            }
        }
    }
}
