using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    .Where(_ => _.Name == "class")
                    .FirstOrDefault();
            string availability = "";
            if (CurrentDate == true)
            {
                availability = PsScheduleDateStatus.Current.ToString();
            }
            else if (Schedule == null && (Booking == false || BookedDate == default(DateTime)))
            {
                availability = PsScheduleDateStatus.Available.ToString();
            }
            else if (Schedule?.StartTime.HasValue == true || (Booking == true && BookedDate != default(DateTime)))
            {
                availability = PsScheduleDateStatus.Time.ToString();
                if (Schedule?.StartTime.HasValue == true)
                {
                    var timeString = JsonConvert.SerializeObject(new List<string>()
                {
                    Schedule.StartTime.Value.ToString("hh:mm tt"),
                    Schedule.EndTime.Value.ToString("hh:mm tt")
                });
                    output.Attributes.Add("data-time", timeString);
                }
            }
            else if (Schedule != null && Schedule.StartTime.HasValue == false)
            {
                availability = PsScheduleDateStatus.Unavailable.ToString();
            }

            if (Booking == false)
            {
                output.Attributes.Add("data-availability", availability);
            }
            if (existingClasses.Value.ToString().Contains("unselectable") == false
                || (Booking && availability == PsScheduleDateStatus.Unavailable.ToString()))
            {
                output.Attributes.Remove(existingClasses);
                var attribute = new TagHelperAttribute("class", $"{existingClasses.Value} " +
                    $"{availability.ToLower()}");

                output.Attributes.Add(attribute);
            }
        }
    }
}
