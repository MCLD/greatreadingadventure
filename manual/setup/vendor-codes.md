# Vendor codes

## About vendor codes

Vendor codes are generated in the GRA software and can then be provided to outside vendors
for redemption. The typical vendor code approach would look something like this:

1. At the start of the program, staff generate vendor codes.
2. Codes are sent to an outside vendor.
3. The vendor enters those codes into their system as valid for redemption.
4. Staff configure a trigger with the vendor code selected in "Award vendor code".
5. When a participant activates the trigger they are given their unique vendor code.
6. The participant can redeem that vendor code through the outside vendor.
7. Optionally, the vendor can send back status reports which can be uploaded into the software.

In addition, the software can be configured to offer participants options when they've activated
the trigger associated with the vendor code:

- Participants can choose to "donate" their prize - this will cause the code to not reveal to them
  so that the program administrators can redirect their prize as a donation
- Participants can opt for an email award which allows the prize to be delivered to them via email
  rather than through the vendor code approach

### Accepting participant codes

Codes default to the format of three sets of five alphanumeric
characters separated by hyphens (e.g. `X123X-456YY-ZZ789`). The party
administering the GRA software generates a set of codes prior to the
start of the program and exports the codes to a text file to provide
to the vendor.

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

The GRA is configured to accept invoice updates. The format for reporting order and ship dates is
an Excel `.xls` file. There can be as many columns as needed in the spreadsheet, the software looks
for the following:

- "Coupon" (required) - used to map to a participant (this is referred to as "code" above,
  e.g. `X123X-456YY-ZZ789`)
- "Branch Id" (optional) - the branch ID in the GRA software that the shipment is associated with
- "Order Date" (optional) - the date that the outside item was placed in an "ordered" status
- "Ship Date" (optional) - the date that the outside item was placed in a "shipped" status
- "Details" (optional) - details about the item that was selected
- "Pickpack Number" (optional) - a packing slip number for the shipment of the item
- "UPS Tracking Number" (optional) - comma-separated tracking numbers with a shipper

Those titles should appear exactly as shown here in the first row of the
spreadsheet.

If all fields except coupon are blank there's no reason to include the coupon code in the
sheet.

An administrator of the reading program can import these spreadsheets
periodically as needed so that participants can see their order status.

## Vendor code setup

Vendor code configuration can be found in Mission Control: under the Setup menu you'll find
"Vendor code management".

### Vendor code configuration

Required fields:

- **Description** - the kind of vendor code, will be shown to staff and participants
- **Mail subject** - the subject of the in-software mail to send when a participant is
  assigned a code
- **Mail** - the text of the in-software mail to send when a participant is assigned a code, the
  text `{Code}` will be replaced with their code (e.g. "Your code is: {Code}")

Optional fields:

- **Url** - a URL template to redeem a prize, `{Code}` will be replaced with the participant's
  code (e.g. "https://vendor/?Code={Code}")
- **Expiration date** - a cut-off date after which the codes shouldn't be assigned
- **Award prize on ship date** - add a prize to the participant's profile when an Excel file is
  imported with the "Ship Date" column set for the code
- **Award prize on received packing slip** - add a prize to the participant's profile when:
  1. An Excel file is imported with a "Packing Slip" number set
  2. Also, the packing slip is entered as received through the packing slip interface

### Additional configuration options

If you wish to configure options for awarding prizes you may do that in the additional
configuration fields. Configure the first two fields to activate the option functionality
(i.e. when they activate the trigger that awards the vendor code they are routed into the option
selection route rather than just provided with a vendor code).

- **OptionSubject** - The subject of the in-software mail sent letting the participant know they
  need to choose an option for the code
- **OptionMail** - The text of the in-software mail sent letting the participant know they
  need to choose an option for the code

Once you've activated the option functionality you should enable either donations or email rewards:

- **DonationMessage** - A short message shown on the users profile letting them know the
  reward has been donated.
- **DonationSubject** - The subject of the in-software mail sent letting the participant know
  their reward has been donated.
- **DonationMail** - The text of the in-software mail sent letting the participant know their
  reward has been donated.

- **EmailAwardMessage** - A short message shown on the users profile letting the participant
  know their reward will arrive via email.
- **EmailAwardSubject** - The subject of the in-software mail sent letting the participant know
  their reward will arrive via email.
- **EmailAwardMail** - The text of the in-software mail sent letting the participant know
  their reward will arrive via email.

Addtionally for email delivery a **VendorCodeTypeText** record for each
language needs to be added. The **EmailAwardInstructions** field is a
message displayed to the participant when they select the email delivery
option letting them know how the reward will arrive and what steps are
needed to redeem it.

Vendor codes that require an option be selected can have an
**ExpirationDate** set. If set the date the reward expires will be shown
to participants and after the date has passed the buttons to select a
redemption option will be hidden.

### Generating vendor codes

Once you've configured how vendor codes should operate, you must then generate vendor codes. Ensure
that you generate enough codes to last for your entire program!

### Download vendor codes

You must send vendor codes to your outside vendor. To download a list of all valid, generated
vendor codes, under the "Configuration" drop-down, choose "Dowanlod Codes".

**Note** that if you generate more codes during the program you will have to download them and
ensure your vendor has them imported into their system!
