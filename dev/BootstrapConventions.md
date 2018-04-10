## Alerts and glyphicons

General guidelines:

* Bad user entry results in an `alert-danger` with `glyphicon-remove`.
* System unable to find something or valid input that just "didn't work" is in an `alert-warning` with a `glyphicon-exclamation-sign`.
* A wrong guess in an adventure should be shown in an `alert-warning` with a `glyphicon-question-sign`.
* Informational messages are in an `alert-info` with a `glyphicon-info-sign`.

For info on sending an alert message to the user via code, please review the [ASP.NET conventions](ASPNETConventions.md).

| Glyphicon      | Use
| -------------- | ---
|barcode         |code redeemed (in `alert-success`)
|certificate     |completed challenge, badge awarded
|check           |saved (challenge progress, profile changes)
|exclamation-sign|validation summary (in `alert-danger`), code invalid, already redeemed, attempt to submit too much reading, items not found (in `alert-warning`)
|flag            |completed adventure
|info-sign       |info message in an `alert-info`
|lock            |change password
|ok              |password has been reset
|pencil          |write a mail
|plus-sign       |add family member
|question-sign   |wrong guess in an Adventure
|remove          |bad entry in form field (with `has-feedback`), delete mail, invalid form entry (in `alert-danger`)
|save            |save changes
|send            |send an entered mail message
|star            |completed challenge
|thumbs-up       |reading activity logged, registered successfully, correct guess in an Adventure
|th-list         |return to a list (mail messages, family members)
