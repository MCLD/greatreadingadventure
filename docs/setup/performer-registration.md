# Performer Registration

Added in version 4.1.2, the performer registration system provides management of:

- Performers register themselves with performer and program information
- Staff can review available programs during a preview period
- At a specific date and time, staff can make selections as to which performers they'd like to schedule at what time
- Administrators verify that performers are available and that there are no scheduling conflicts
- Finalized scheduling information can be provided to performers and staff

## Enable performer registration

To start using the performer registration functionality, an administrator must do the initial setup by accessing the **Performer management** menu item in the wrench menu of Mission Control.

### Initial Settings

On the initial page the following must be configured:

- A contact email for performers or staff who have questions
- The number of program selections available for each branch
- Registration open and close dates and times for performers to register with the software
- A scheduling preview date and time when staff can review the available performers
- Scheduling open and close dates and times for staff to make their selection(s)
- The schedule posted date and time when staff will be able to review if their selections were successful and performer appointments have been verified
- The Schedule Start and Schedule End Dates which represent the start and end dates for when programs can be scheduled (there are typically the start and end date of the reading program but could be different if necessary).

Once these items are entered, they can be updated in the **Settings** tab (which will be visible once you initially configure them).

### Age Groups

One or more age groups must be configured under the **Age Groups** tab in order for performers to register. Age groups can be used so that multiple programs may be scheduled with a limitation of only one program per age group per branch. Age groups must have a name and a color (in hexadecimal, Web color format) associated with them.

### Blackout Dates

If there are dates for which you do not want to allow program scheduling, add those dates in the **Blackout Dates** area. For example, you may wish to disallow programs on July 4th during a summer reading program if your branches are closed on Independence Day.

### Excluded Branches

If there are any branches in the system that you wish to exclude from selection for a program, select them here.

## Configure a performer role

Performers must have a role granting them access to the performer registration functionality. When an administrator selects the wrench icon, they can choose **Role management** in order to create the role. Ensure the role has the **AccessPerformerRegistration** permission.

The easiest method for conferring this role to the performers is to create an **Authorization Code** to add it to their account. This would be a multiple-use code granting the role that was created above. Performers can then be guided to the url `/Join/AuthorizationCode/` on the site to register as a performer, even if the registration date configured in the software has not happened yet.
