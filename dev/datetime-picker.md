# Date and Time Selection in the GRA

The GRA has the [Tempus Dominus](https://getdatepicker.com/) ("TD") date and time picker available for use along with some tag helpers to simplify use.

## Typical use

Assuming that the field "StartDate" is present in the page model and that the form controls are on the page upon page load (i.e. before the `scripts/gra.js` script is loaded), this pattern works:

```html
<div
  class="gra-picker-datetime"
  current-value="@Model.StartDate"
  datetimepicker-container
  id="startDatePicker"
>
  <input asp-for="StartDate" datetimepicker-input type="text" />
</div>
```

Notes:

- Container class `gra-picker-datetime` is automatically loaded as a TD picker with JavaScript in the site-wide `scripts/gra.js` file.
- The `current-value` attribute is parsed by the tag helper to move the current value into the JavaScript constructor of th date/time picker as the `defaultDate` parameter.
- Adding `datetimepicker-container` as a class activates the `GRA.Controllers.Helpers.DateTimeContainerTagHelper` tag helper.
- There must exist a unique ID on the container so that it can bind to the `input` field that it contains. Either provide a _unique on the page_ ID or provide no ID and the code will use a generated GUID.
- Inside the `datetimepicker-container` tag you should have an `<input>` tag to hold the actual value.
- This input can use the ASP.NET Form tag helper via `asp-for` or can have traditional `id` or `name` properties.
- Including `datetimepicker-input` activates the `GRA.Controllers.Helpers.DateTimeInputTagHelper` tag helper.
- Though the data submitted can be a `DateTime` in the page model, manually specify `type="text"` on this field control as that's what the date/time picker expects.
- For a **date-only** picker, use `gra-picker-date` and add a `value=""` to the `<input>` tag - by default ASP.NET/Razor will populate the input with the value from the model which will be a `DateTime` and formatted as such, specifying an empty value leaves the field empty until the TD object is instantiated which moves thte `current-value` into the content of the field. 
- For a **time-only** picker, use `gra-picker-time`.

## Event binding

If it is necessary to bind to picker events a reference to the TD object will need to be mantained. In this case, _exclude_ the `gra-picker-*` reference (this is what the side-wide `gra.js` file uses to automatically create the TD object for relevant page tags) and in the page's JavaScript include something similar to:

```js
var datePicker = graInitalizePickerTime(document.getElementById("startDatePicker"));
datePicker.subscribe(tempusDominus.Namespace.events.change, (e) => {
  if (e.date != null) {
    // take action based on the selected date
  } else {
    // probably not a valid date, etc.
  }
});
```
