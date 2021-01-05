# Vendor codes

Vendor codes are generated in the GRA software and can then be provided to outside vendors
for redemption. The typical vendor code approach would look something like this:

1. At the start of the program, staff generate vendor codes.
2. Codes are sent to an outside vendor.
3. The vendor enters those codes into their system as valid for redemption.
4. Staff configure a trigger with the vendor code selected in "Award vendor code".
5. When a participant activates the trigger they are given their unique vendor code.
6. The participant can redeem that vendor code through the outside vendor.
7. Optionally, the vendor can send back status reports containing an Order Date, a Ship
   Date, and an item description which can be uploaded into the software.

## Vendor interface

### Accepting participant codes

Codes default to the format of three sets of five alphanumeric
characters separated by hyphens (e.g. `X123X-456YY-ZZ789`). The party
administering the GRA software generates a set of codes prior to the
start of the program and then provides them to the vendor in a standard
format such as an Excel spreadsheet or CSV (comma-separated value)
document.

When participants unlock the vendor code, the software informs them in
an in-software mail message that includes a link. The GRA can include
the code in the link provided to the customer. For example, a template
URL of:

```
https://vendor/?Code={Code}
```

Would expand (using the sample code from above) to:

```
https://vendor/?Code=X123X-456YY-ZZ789
```

This is a convenience for participants. Ideally the vendor Web site will
capture any query string code when the customer arrives on the site and
save it until the vendor gets to the shopping cart/check-out area so
that it can be prepopulated. Note that it's ideal to allow the customer
to enter a code as well in case the code does not come through the link
properly.

### Reporting status

The GRA is configured to accept invoice updates including an order date
as well as a ship date. The order date represents the date that the
vendor received the item request, the ship date represents the date that
the vendor dispatched the item via a shipping service.

The format for reporting order and ship dates is an Excel file with the
following format:

There can be as many columns as needed in the spreadsheet, the software looks for the
following:

- "Coupon" (required) - used to map to a participant (this is referred to as "code" above,
  e.g. `X123X-456YY-ZZ789`)
- "Order Date" (optional) - the date that the outside item was placed in an "ordered" status
- "Ship Date" (optional) - the date that the outside item was placed in a "shipped" status
- "Details" (optional) - details about the item that was selected

Those titles should appear exactly as shown here in the first row of the
spreadsheet.

If all fields except coupon are blank there's no reason to include the coupon code in the
sheet.

An administrator of the reading program can import these spreadsheets
periodically as needed so that participants can see their order status.

### Additional redemption options

The GRA can be configured to allow for the reward to be donated or
delivered via email.

To enable either of these options the following fields on VendorCodeType must be set:

- **OptionSubject** - The subject of the in-game mail sent letting the participant know they
  need to choose an option for the code
- **OptionMail** - The message of the in-game mail sent letting the participant know they
  need to choose an option for the code

To enable donations the additional fields on VendorCodeType need to be set:

- **DonationSubject** - The subject of the in-game mail sent letting the participant know
  their reward has been donated.
- **DonationMail** - The message of the in-game mail sent letting the participant know their
  reward has been donated.
- **DonationMessage** - A short message shown on the users profile letting them know the
  reward has been donated.

To enable email delivery the additional fields on VendorCodeType need to be set:

- **EmailAwardSubject** - The subject of the in-game mail sent letting the participant know
  their reward will arrive via email.
- **EmailAwardMail** - The message of the in-game mail sent letting the participant know
  their reward will arrive via email.
- **EmailAwardMessage** - A short message shown on the users profile letting the participant
  know their reward will arrive via email.

Addtionally for email delivery a **VendorCodeTypeText** record for each
language needs to be added. The **EmailAwardInstructions** field is a
message displayed to the participant when they select the email delivery
option letting them know how the reward will arrive and what steps are
needed to redeem it.

Vendor codes that require an option be selected can have an
**ExpirationDate** set. If set the date the reward expires will be shown
to participants and after the date has passed the buttons to select a
redemption option will be hidden.
