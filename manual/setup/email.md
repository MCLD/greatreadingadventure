# Email

## Setup

Ensure that the **From Email Name**, **From Email Address**, and **Outgoing Mail Host** are configured in [Site Settings](site-settings.md). You can verify the configuration is working by using the "Send test email" field and button.

### Base Templates

In the **Email Management** area of Mission Control you can create Base Templates. The system comes with a default template that is functional but it can be customized. The recommended basis for a Base Template is to use [MJML](https://mjml.io/) to generate HTML that is compatible with the widest number of email clients. The Base Template configuration provides a field where you can save your MJML to make it easier to find and customize further later but the software does not use the MJML that is saved. The following macro replacements can be used in a Base Template:

- `{{BodyHtml}}`
- `{{BodyText}}`
- `{{Footer}}`
- `{{Preview}}`
- `{{Title}}`

Base Templates can be imported and exported using the buttons on the bottom of the edit screen.

### Templates

Actual emails that are sent are configured as an Email Template. The system comes with the following built-in templates (which can be customized):

- Mission Control news post
- Password recovery
- System Configuration Test Message
- Username recovery

The system sends these email templates at the appropriate times.

Templates can be added for sending to lists (if you have lists configured with the **Collect emails after access has closed** and **Collect emails before registration has opened** in [Site Settings](site-settings.md)) or for configuring a welcome email.

## Welcome email

A Template can be configured to be sent as a welcome email to participants once they sign up. Here are the details:

- Email system must be configured and functional.
- Job tasks must be configured using the `GraJobSleepSeconds` [Application Setting](../technical/appsettings.md).
- The program must be in the **Program Open** stage as configured in scheduling.
- In the **Email** section of [Site Settings](site-settings.md), the setting **Welcome email delay hours** can be set if you'd like the software to wait a period of time after the participant signs up before sending the email (i.e. if all above criteria is met, the system can still delay a number of hours before emailing new participants).

Once the setup above is complete you can create a template containing the text for the welcome email. From the **Email Management** screen in Mission Control select teh **Set Special Emails** button. In the list of emails that is displayed, click the **Set as Welcome Email** button next to the email you wish to be used as the welcome email.
