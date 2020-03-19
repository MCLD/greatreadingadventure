# Vendor codes

Vendor codes are generated in the GRA software and can then be provided
to outside vendors for redemption.

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

There can be as many columns as needed in the spreadsheet so long as the
following three columns are present:

- "Coupon" (this is referred to as "code" above, e.g.
`X123X-456YY-ZZ789`)
- "Order Date"
- "Ship Date"

Those titles should appear exactly as shown here in the first row of the
spreadsheet.

In the sheet either the Order Date or Ship Date column can be left empty
if the date isn't known. If both are blank there's no reason to include
the coupon code in the sheet.

An administrator of the reading program can import these spreadsheets
periodically as needed so that participants can see their order status.
